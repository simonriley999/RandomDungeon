using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class RoomGenerator : MonoBehaviour
{
    public static RoomGenerator instance;
    private void Awake() 
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;

        // DontDestroyOnLoad(gameObject);

        //单例模式
    }
    public MapGenerator mapGenerator;
    public event Action onVictory;
    [Header("Generate Tile Room")]
    public GameObject tilePrefab;
    // public Transform roomHolder;
    public Vector2 roomSize;
    public Vector3 roomPosition;
    [Range(0,1)]public float outlinePercent;
    public GameObject player;
    public Texture2D cursor;
    private List<RoomStats> roomsStats;

    [Header("Generate Random Obstacle")]
    public GameObject obsPrefab;
    public GameObject doorPrefab;
    public List<List<Coord>> allRoomsTilesCoord;
    public Coord roomCenter;//房间中心生成人物，不能生成障碍物
    List<bool[,]> allRoomObstacles;//表示当前点是否已经有障碍物

    // public float minHeight;
    // public float maxHeight;
    [SerializeField]public float height;

    [Range(0,1)]public float obsPercent;
    // [Header("Paint Colorful")]
    // public Color foregroundColor,backgroundColor;
    [Header("NavMesh Ganerate")]
    public GameObject navMesh;

    [System.Serializable]
    public struct Coord
    {
        public int x,y;
        public Coord(int x,int y)
        {
            this.x=x;
            this.y=y;
        }

        public static bool operator == (Coord _coord1,Coord _coord2)
        {
            return (_coord1.x==_coord2.x) && (_coord1.y == _coord2.y);
        } 

        public static bool operator != (Coord _coord1,Coord _coord2)
        {
            return !(_coord1 == _coord2);
        }
    };

    // Start is called before the first frame update
    void Start()
    {
        mapGenerator.onReadyToGenerateRoom += StartToGenerateRoom;
        allRoomsTilesCoord = new List<List<Coord>>();
        allRoomObstacles = new List<bool[,]>();
        roomsStats = new List<RoomStats>();
        //Generateroom();
        // GenerateNavMesh();//将在大地图生成时在统一进行烘焙
        //Init();
    }

    public void InitPlayer()
    {
        Vector2Int p = roomsStats[0].GetRoomCenter();
        roomsStats[0].isClear = true;
        roomsStats[0].isLock = false;
        mapGenerator.clearRoomCount++;
        // player = Instantiate(player,new Vector3(p.x + 0.5f, 1.5f, p.y + 0.5f),Quaternion.identity);
        player.transform.position = new Vector3(p.x + 0.5f, 0.5f, p.y + 0.5f);
        Cursor.SetCursor(cursor,new Vector2(32,32),CursorMode.Auto);
    }

    bool roomIsFullAccessible(bool[,] _roomObstacles,int _currentObscont)
    {
        int width = _roomObstacles.GetLength(0);
        int height = _roomObstacles.GetLength(1);
        bool[,] roomflags = new bool[width,height];//用于存储当前障碍是否已经检测
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(roomCenter);
        roomflags[roomCenter.x,roomCenter.y] = true;//房间中心为起点，设定为一定可通行
        int accessibleCount = 1;
        while (queue.Count>0)
        {
            Coord current = queue.Dequeue();
            for (int i=-1;i<=1;i++)
            {
                for (int j=-1;j<=1;j++)
                {
                    int neighborX = current.x + i;
                    int neighborY = current.y + j;
                    if (i==0||j==0)//四向检索
                    {
                        if (neighborX<width && neighborX>=0 && neighborY<height && neighborY>=0//不超出房间边界,除去墙壁
                        && !roomflags[neighborX,neighborY] && !_roomObstacles[neighborX,neighborY])//没有访问过且没有障碍物
                        {
                            roomflags[neighborX,neighborY] = true;
                            // Debug.Log("neighborX = " + neighborX + " , neighborY = " + neighborY);
                            accessibleCount++;
                            queue.Enqueue(new Coord(neighborX,neighborY));
                        }
                    }
                }
            }
            
        }
        queue.Clear();
        // Debug.Log("accessibleCount = " + accessibleCount);
        // Debug.Log("(int)((width) * (height) - _currentObscont) = " + (int)((width) * (height) - _currentObscont));
        return (accessibleCount == (int)((width) * (height) - _currentObscont));
    }

    void StartToGenerateRoom(Grid2D<MapGenerator.CellType> _grid,List<Room> _roomList)
    {
        StartCoroutine(GenerateRoom(_grid,_roomList));

        GenerateNavMesh();

        InitPlayer();
    }

    IEnumerator GenerateRoom(Grid2D<MapGenerator.CellType> _grid,List<Room> _roomList)
    {
        Room temp;
        for (int i=0;i<mapGenerator.mapSize.x;i++)
        {
            for (int j=0;j<mapGenerator.mapSize.y;j++)
            {
                if (_grid[new Vector2Int(i,j)] == MapGenerator.CellType.Hallway)
                {
                    GameObject spawnHallwayTile = Instantiate(tilePrefab,new Vector3(i + 0.5f, 0, j + 0.5f) + roomPosition ,Quaternion.Euler(90,0,0));
                    spawnHallwayTile.transform.parent = mapGenerator.hallwaysHolder.transform;
                    spawnHallwayTile.transform.localScale *= 1f-outlinePercent;
                    spawnHallwayTile.name = "hallway";
                }
                else if (_grid[new Vector2Int(i,j)] == MapGenerator.CellType.HallwayWall)
                {
                    GameObject obstacle = Instantiate(obsPrefab,new Vector3(i + 0.5f, 0.5f, j + 0.5f) + roomPosition ,Quaternion.identity);
                    obstacle.transform.parent = mapGenerator.hallwaysHolder.transform;
                    obstacle.transform.localScale = new Vector3(1-outlinePercent,height,1-outlinePercent);
                    obstacle.name = "hallwayWall";
                }
            }
        }

        #region 
        int index = 0;
        int doorId = 0;//用于给每个一个唯一标识
        for (int k=0;k<_roomList.Count;k++)
        {
            temp = _roomList[k];
            List<Coord> allTilesCoord = new List<Coord>();
            if (temp.isMain)
            {
                roomsStats.Add(new RoomStats(temp));
                for (int i=temp.roomPosition.x;i<temp.roomPosition.x+temp.roomSize.x;i++)
                {
                    for (int j=temp.roomPosition.y;j<temp.roomPosition.y+temp.roomSize.y;j++)
                    {
                        switch(_grid[new Vector2Int(i,j)])
                        {
                            case MapGenerator.CellType.Room:
                                GameObject spawnTile = Instantiate(tilePrefab,new Vector3(i + 0.5f, 0, j + 0.5f) + roomPosition ,Quaternion.Euler(90,0,0));
                                spawnTile.transform.parent = temp.parent;
                                spawnTile.transform.localScale *= 1f-outlinePercent;
                                spawnTile.name = "floor";
                                allTilesCoord.Add(new Coord(i-temp.roomPosition.x-1,j-temp.roomPosition.y-1));
                                break;
                            case MapGenerator.CellType.RoomWall:
                            case MapGenerator.CellType.Obstacle:
                                GameObject obstacle = Instantiate(obsPrefab,new Vector3(i + 0.5f, 0.5f, j + 0.5f) + roomPosition ,Quaternion.identity);
                                obstacle.transform.parent = temp.parent;
                                obstacle.transform.localScale = new Vector3(1-outlinePercent,height,1-outlinePercent);
                                obstacle.name = "obstacle";
                                break;
                            case MapGenerator.CellType.Door:
                                GameObject door = Instantiate(doorPrefab,new Vector3(i + 0.5f, 0.5f,+ j + 0.5f) + roomPosition ,Quaternion.identity);
                                door.transform.parent = temp.parent;
                                door.transform.localScale *= 1f-outlinePercent;
                                door.GetComponent<DoorController>().belongedRoom = index;//将门所属于的房间号存储起来，以便于之后单个房间的敌人生成
                                door.GetComponent<DoorController>().doorId = doorId++;
                                door.name = "door";
                                break;
                            default:
                            break;
                        }
                    }
                }
                index++;

                yield return new WaitForEndOfFrame();
                
                roomSize = temp.roomSize;
                List<Coord> coords = new List<Coord>(Utilities.ShuffleCoords(allTilesCoord.ToArray()));
                int obsCount = (int)((temp.roomSize.x-2) * (temp.roomSize.y-2) * obsPercent);
                roomCenter = new Coord((int)temp.roomSize.x/2,(int)temp.roomSize.y/2);
                bool[,] roomObstacles = new bool[(int)temp.roomSize.x,(int)temp.roomSize.y];
                int currentObscount = 0;

                for (int i=0;i<roomSize.x;i++)
                {
                    for (int j=0;j<roomSize.y;j++)
                    {
                        if (i == 0 || j == 0 || i == roomSize.x -1 || j == roomSize.y - 1)
                        {
                            if (_grid[new Vector2Int(i + temp.roomPosition.x,j + temp.roomPosition.y)] == MapGenerator.CellType.RoomWall)
                            {
                                roomObstacles[i,j] = true;
                                currentObscount++;
                            }
                        }
                    }
                }

                // yield return new WaitForSeconds(0.5f);

                for (int i=0;i<obsCount;i++)
                {
                    roomObstacles[coords[i].x + 1,coords[i].y + 1] = true;
                    currentObscount++;
                    if (new Coord(coords[i].x + 1,coords[i].y + 1) != roomCenter && roomIsFullAccessible(roomObstacles,currentObscount))
                    {
                        GameObject obstacle = Instantiate(obsPrefab,new Vector3(temp.roomPosition.x+coords[i].x + 1 + 0.5f,height/2,temp.roomPosition.y+coords[i].y + 1 + 0.5f) + roomPosition ,Quaternion.identity);
                        obstacle.transform.parent = temp.parent;
                        obstacle.transform.localScale = new Vector3(1-outlinePercent,height,1-outlinePercent);
                        obstacle.name = "obstacle";
                    }
                    else
                    {
                        roomObstacles[coords[i].x + 1,coords[i].y + 1] = false;
                        currentObscount--;
                    }
                    // yield return new WaitForSeconds(0.5f);
                }
                allRoomObstacles.Add(roomObstacles);
                allRoomsTilesCoord.Add(allTilesCoord);
            }

            // yield return new WaitForSeconds(0.5f);

        }
        yield return null;
        #endregion
    }

    void GenerateNavMesh()
    {
        navMesh.transform.position = new Vector3(mapGenerator.mapSize.x/2,0,mapGenerator.mapSize.y/2);
        navMesh.transform.localScale = new Vector3(mapGenerator.mapSize.x, 0 ,mapGenerator.mapSize.y);
        navMesh.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    public Transform GetRandomOpenTile(int roomIndex)//门关闭时，生成敌人调用
    {
        Coord openTile;
        int index;
        int height = roomsStats[roomIndex].roomSize.y;
        do
        {
            index = UnityEngine.Random.Range(0,allRoomsTilesCoord[roomIndex].Count);
            openTile = new Coord(allRoomsTilesCoord[roomIndex][index].x,allRoomsTilesCoord[roomIndex][index].y);
        } while (allRoomObstacles[roomIndex][openTile.x + 1,openTile.y + 1]);
        return mapGenerator.transform.GetChild(roomIndex+2).GetChild((openTile.x+1)*height + openTile.y+1);//Mapgenerator中原有两个子物体，roomHolder下标从2开始，竖向摆放
        // return new Vector3(roomsStats[roomIndex].roomPosition.x + openTile.x + 1 ,0.5f ,roomsStats[roomIndex].roomPosition.y + openTile.y + 1);//直接返回世界坐标系中的位置
    }

    public bool IsPlayerInRoom(int _roomNumber)
    {
        Vector3 playerPos = player.transform.position;
        RoomStats thisRoom = roomsStats[_roomNumber];
        if (playerPos.x >= thisRoom.roomPosition.x + 0.5f && playerPos.x <= thisRoom.roomPosition.x + thisRoom.roomSize.x - 0.5f
        && playerPos.z >= thisRoom.roomPosition.y + 0.5f && playerPos.z <= thisRoom.roomPosition.y + thisRoom.roomSize.y - 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsRoomLocked(int _roomNumber)
    {
        return roomsStats[_roomNumber].isLock;
    }
    public void SetRoomUnlock(int _roomNumber)
    {
        roomsStats[_roomNumber].isLock = false;
    }

    public void SetRoomLock(int _roomNumber)
    {
        roomsStats[_roomNumber].isLock = true;
    }
    public bool IsRoomClear(int _roomNumber)
    {
        return roomsStats[_roomNumber].isClear;
    }

    public void SetRoomClear(int _roomNumber)
    {
        roomsStats[_roomNumber].isClear = true;
        mapGenerator.clearRoomCount++;
        if (mapGenerator.clearRoomCount >= roomsStats.Count)
        {
            onVictory?.Invoke();
        }
    }

    public void SetRoomUnclear(int _roomNumber)
    {
        roomsStats[_roomNumber].isClear =false;
        mapGenerator.clearRoomCount--;
    }

    public Vector2Int GetRoomCenter(int _roomNumber)
    {
        Room temp = roomsStats[_roomNumber];
        return new Vector2Int((int)(temp.roomPosition.x + temp.roomSize.x/2),(int)(temp.roomPosition.y + temp.roomSize.y/2));
    }

    public Transform GetRoomCenterTile(int _roomNumber)
    {
        Room temp = roomsStats[_roomNumber];
        int height = temp.roomSize.y;
        return mapGenerator.transform.GetChild(_roomNumber+2).GetChild((int)(temp.roomSize.x / 2)*height + (int)(temp.roomSize.y / 2));
    }

    public bool IsTheLastRoom()
    {
        return mapGenerator.clearRoomCount + 1 == roomsStats.Count;
    }
}
