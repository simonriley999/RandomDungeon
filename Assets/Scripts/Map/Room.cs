using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int roomPosition;
    public Vector2Int roomSize;//x == width,y == height

    public Transform parent;

    public bool isMain;//是否是删选后的房间

    public Room()
    {
        isMain = true;
    }
    public bool CollidesWith(Room room)
    {
        bool value = true;
        if (room.roomPosition.x >= this.roomPosition.x + this.roomSize.x + 1 ||
            room.roomPosition.y >= this.roomPosition.y + this.roomSize.y + 1 ||
            room.roomPosition.x + room.roomSize.x + 1 <= this.roomPosition.x ||
            room.roomPosition.y + room.roomSize.y + 1 <= this.roomPosition.y)//+1是为了给走廊预留位置
        {
            value = false;
        }
        return value;
    }
    public void Shift(int shiftX, int shiftY)
    {
        roomPosition.x += shiftX;
        roomPosition.y += shiftY;
    }

    public Vector2Int GetRoomCenter()
    {
        return new Vector2Int((int)(roomSize.x / 2 + roomPosition.x),(int)(roomSize.y / 2 + roomPosition.y));
    }
}
