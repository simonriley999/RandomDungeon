using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DoorEventSystem : MonoBehaviour
{
    public static DoorEventSystem instance;

    public event Action<int> onDoorOpen;
    public event Action<int> onDoorClose;
    public event Action<int> onSpawnEnemy;

    public RoomGenerator roomGenerator;

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

    public void DoorOpen(int _id,int _roomNumber)
    {
        if (onDoorOpen != null && !roomGenerator.IsRoomLocked(_roomNumber))
        {
            onDoorOpen(_id);//只开启相对应的门
        }
    }

    public void DoorClose(int _id,int _roomNumber)
    {
        if (onDoorClose != null)
        {
            onDoorClose(_id);//只关闭相对应的门
            if (onSpawnEnemy != null && !roomGenerator.IsRoomClear(_roomNumber) && !roomGenerator.IsRoomLocked(_roomNumber) && roomGenerator.IsPlayerInRoom(_roomNumber))
            {
                roomGenerator.SetRoomLock(_roomNumber);
                onSpawnEnemy(_roomNumber);
            }
        }
    }

}
