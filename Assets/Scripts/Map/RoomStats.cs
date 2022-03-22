using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomStats : Room
{
    //public int roomNumber;//房间序号用数组下标表示
    public bool isLock;//房间是否上锁
    public bool isClear;//房间的怪是否被清除
    public bool isBoss;//改房间是否被认为是boss房

    public RoomStats()
    {
        isLock = false;
        isClear = false;
        isBoss = false;
    }

    public RoomStats(Room _room)
    {
        this.roomPosition = _room.roomPosition;
        this.roomSize = _room.roomSize;
        this.parent = _room.parent;
        this.isMain = _room.isMain;
        isLock = false;
        isClear = false;
        isBoss = false;
    }
}
