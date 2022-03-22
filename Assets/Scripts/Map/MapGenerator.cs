using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using Graphs;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator instance;
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
    [HideInInspector]public enum CellType {
        None,
        Room,
        Hallway,
        Obstacle,
        RoomWall,
        Door,
        HallwayWall
    }
    System.Random random;
    public RoomGenerator roomGenerator;
    public GameObject hallwaysHolder;
    public GameObject roomHolder;
    [Header("Room Information")]
    [SerializeField]public int roomMinHeight;
    [SerializeField]public int roomMaxHeight;
    [SerializeField]public int roomMinWidth,roomMaxWidth;
    [SerializeField]public int roomCircleRadius;//生成房间的位置范围
    [SerializeField]public int roomCount;
    private float widthAvg,heightAvg;
    [SerializeField]public float mainRoomMeanCutoff;//保留的房间的阈值
    [SerializeField]public Vector2Int mapSize;
    [Header("Game Control")]
    public int clearRoomCount;//用于记录已清空的房间数量，也要用于判断最后的boss房
    List<Room> roomList;
    Grid2D<CellType> grid;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;
    public GameObject testCube;
    private Vector2Int playerPos;
    public event Action<Grid2D<CellType>,List<Room>> onReadyToGenerateRoom;

    bool isError;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();
        roomList = new List<Room>();
        grid = new Grid2D<CellType>(mapSize, Vector2Int.zero);
        if (roomGenerator == null)
        {
            roomGenerator = new RoomGenerator();
        }
        clearRoomCount = 0;

        MapGenerate();
    }

    void MapGenerate()
    {
        PlaceRoom();
        RoomSperate();
        PickMainRoom();
        Triangulate();
        if (isError)
        {
            return;
        }
        #region test
        // foreach (Vertex<Room> v in delaunay.Vertices)
        // {
        //     GameObject cube = Instantiate(testCube,new Vector3(v.Item.roomPosition.x + (float)v.Item.roomSize.x / 2,0.5f,v.Item.roomPosition.y + (float)v.Item.roomSize.y / 2),Quaternion.identity);
        //     cube.transform.localScale = new Vector3(v.Item.roomSize.x,1,v.Item.roomSize.y);
        // }
        #endregion
        CreateHallways();
        StartCoroutine(PathfindHallways());
        // CreateRoom();
        PlaceCube();
    }

    private void PlaceRoom()
    {
        Room currentRoom;
        for (int i=0;i<roomCount;i++)
        {
            currentRoom = new Room();
            currentRoom.roomSize = new Vector2Int(
                Mathf.RoundToInt(RandomFromDistribution.RandomRangeNormalDistribution(roomMinWidth, roomMaxWidth, RandomFromDistribution.ConfidenceLevel_e._80)) / 2 * 2,
                Mathf.RoundToInt(RandomFromDistribution.RandomRangeNormalDistribution(roomMinWidth, roomMaxWidth, RandomFromDistribution.ConfidenceLevel_e._80)) / 2 * 2);//化为一个偶数
            Vector2 pos = Utilities.GetRandomPointInCircle(roomCircleRadius);
            currentRoom.roomPosition = new Vector2Int(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.y));
            roomList.Add(currentRoom);
            widthAvg+=currentRoom.roomSize.x;
            heightAvg+=currentRoom.roomSize.y;
        }
        widthAvg/=roomCount;
        heightAvg/=roomCount;
    }

    void RoomSperate()
    {
        bool roomCollision = true;
        while (roomCollision)
        {
            roomCollision = false;

            //利用选择排序进行判断房间是否重叠
            for (int i = 0; i < roomList.Count; i++)
            {
                Room roomA = roomList[i];
                for (int j = i + 1; j < roomList.Count; j++)
                {
                    Room roomB = roomList[j];
                    if (roomA.CollidesWith(roomB))
                    {
                        roomCollision = true;

                        int roomBX = Mathf.RoundToInt((roomA.roomPosition.x + roomA.roomSize.x) - roomB.roomPosition.x) + 1;
                        int roomBY = Mathf.RoundToInt((roomA.roomPosition.y + roomA.roomSize.y) - roomB.roomPosition.y) + 1;

                        int roomAX = Mathf.RoundToInt((roomB.roomPosition.x + roomB.roomSize.x) - roomA.roomPosition.x) + 1;
                        int roomAY = Mathf.RoundToInt((roomB.roomPosition.y + roomB.roomSize.y) - roomA.roomPosition.y) + 1;

                        if (roomAX > roomBX)
                        {
                            if (roomAX > roomAY)
                                roomA.Shift(roomAX, 0);
                            else
                                roomA.Shift(0, roomAY);
                        }
                        else
                        {
                            if (roomBX > roomBY)
                                roomB.Shift(roomBX, 0);
                            else
                                roomB.Shift(0, roomBY);
                        }//移动最小距离    
                    }
                }
            }
        }
    }

    void PickMainRoom()//筛选合适房间
    {
        foreach (Room r in roomList)
        {
            if (r.roomSize.x * mainRoomMeanCutoff < widthAvg || r.roomSize.y * mainRoomMeanCutoff < heightAvg)
            {
                r.isMain = false;    
                //roomList.Remove(r);   
            }    
        }
    }

    void Triangulate() 
    {
        List<Vertex> vertices = new List<Vertex>();

        for (int k= 0;k<roomList.Count;k++) 
        {
            Room room = roomList[k];
            if (room.isMain)
            {
                vertices.Add(new Vertex<Room>((Vector2)room.roomPosition+ ((Vector2)room.roomSize) / 2, room));
                for (int i=room.roomPosition.x;i<room.roomPosition.x + room.roomSize.x;i++)
                {
                    for (int j=room.roomPosition.y;j<room.roomPosition.y + room.roomSize.y;j++)
                    {
                        if (i==room.roomPosition.x||i==room.roomPosition.x+room.roomSize.x-1 || j==room.roomPosition.y||j==room.roomPosition.y+room.roomSize.y-1)
                        {
                            try
                            {
                                grid[new Vector2Int(i,j)] = CellType.RoomWall;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
                                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
                                isError = true;
                                return;
                            }
                        }
                        else
                        {
                            try
                            {
                                grid[new Vector2Int(i,j)] = CellType.Room;//将生成房间的范围在地图数组上标记为房间，避免寻路时在房间内形成道路
                            }
                            catch (IndexOutOfRangeException)
                            {
                                SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
                                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
                                isError = true;
                                return;
                            }
                        }
                    }
                }
                GameObject rh = Instantiate(roomHolder,Vector3.zero,Quaternion.identity,transform);
                rh.name = "roomHolder";
                roomList[k].parent = rh.transform;
            }
            
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }

    void CreateHallways() {
        try
        {
            List<Prim.Edge> edges = new List<Prim.Edge>();

            foreach (var edge in delaunay.Edges) {
                edges.Add(new Prim.Edge(edge.U, edge.V));
            }
            List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);
            selectedEdges = new HashSet<Prim.Edge>(mst);
        }
        catch (ArgumentOutOfRangeException)
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
            SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            isError = true;
            return;
        }

    }

    IEnumerator PathfindHallways() {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(mapSize);

        foreach (var edge in selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = (Vector2)startRoom.roomPosition+ ((Vector2)startRoom.roomSize) / 2;
            var endPosf = (Vector2)endRoom.roomPosition+ ((Vector2)endRoom.roomSize) / 2;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            // Debug.Log(startPos);
            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();
                
                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (grid[b.Position] == CellType.Room || grid[b.Position] == CellType.RoomWall) {
                    pathCost.cost += 10;
                } else if (grid[b.Position] == CellType.None || grid[b.Position] == CellType.HallwayWall) {
                    pathCost.cost += 5;
                } else if (grid[b.Position] == CellType.Hallway) {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (grid[current] == CellType.None || grid[current] == CellType.HallwayWall) {
                        grid[current] = CellType.Hallway;
                    }

                    if (i > 0) {
                        var prev = path[i - 1];

                        var delta = current - prev;
                    }
                }

                foreach (var pos in path) {
                    if (grid[pos] == CellType.Hallway) {
                        PlaceHallwayWall(pos);
                    }
                    if (grid[pos] == CellType.RoomWall && SelectDoor(pos))
                    {
                        grid[pos] = CellType.Door;
                    }
                }//在RoomGenerator里生成
            }
        }
        // PlaceHallwayWall();
        yield return null;
    }

    private void PlaceHallwayWall(Vector2Int pos)
    {
        for (int i = -1;i <= 1;i++)
        {
            for (int j = -1;j <= 1;j++)
            {
                if (grid[new Vector2Int(pos.x+i,pos.y+j)]==CellType.None)
                {
                    grid[new Vector2Int(pos.x+i,pos.y+j)] = CellType.HallwayWall;
                }
            }
        }
    }

    private void PlaceHallwayWall()
    {
        Vector2Int pos;
        for (int x = 0;x < mapSize.x;x++)
        {
            for (int y = 0;y < mapSize.y;y++)
            {
                pos = new Vector2Int(x,y);
                if (grid[pos] == CellType.Hallway)
                {
                    for (int i = -1;i <= 1;i++)
                    {
                        for (int j = -1;j <= 1;j++)
                        {
                            if (grid[new Vector2Int(pos.x+i,pos.y+j)]==CellType.None)
                            {
                                grid[new Vector2Int(pos.x+i,pos.y+j)] = CellType.HallwayWall;
                            }
                        }
                    }
                }
            }
        }
    }
    private void PlaceCube()
    {
        #region test
        // GameObject go = Instantiate(testCube, new Vector3(pos.x + 0.5f, 0.5f, pos.y + 0.5f), Quaternion.identity);
        // go.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
        // go.GetComponent<MeshRenderer>().material.color = Color.green;
        #endregion

        if (onReadyToGenerateRoom != null)
        {
            onReadyToGenerateRoom(grid,roomList);
        }
    }

    bool SelectDoor(Vector2Int _pos)
    {
        int i = _pos.x;
        int j = _pos.y;
        if (i >= 0 && j>=0 && (grid[new Vector2Int(i+1,j)] == CellType.Hallway || grid[new Vector2Int(i-1,j)] == CellType.Hallway || grid[new Vector2Int(i,j+1)] == CellType.Hallway || grid[new Vector2Int(i,j-1)] == CellType.Hallway))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
