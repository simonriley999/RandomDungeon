using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    public int belongedRoom;//所属room的roomHolder的子物体下标
    public bool isClear;//当前房间是否已经安全
    [SerializeField] public int doorId;//用于判断开哪扇门的id
    [SerializeField]public float moveTime;
    public GameObject Door;
    public GameObject triggerArea;
    // Start is called before the first frame update
    void Start()
    {
        DoorEventSystem.instance.onDoorOpen += OpenDoor;
        DoorEventSystem.instance.onDoorClose += CloseDoor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor(int _id)
    {
        if (_id == doorId)
        {
            LeanTween.moveLocalY(Door,-1f,moveTime).setEaseInSine();
        }
    }

    public void CloseDoor(int _id)
    {
        if (_id == doorId)
        {
            LeanTween.moveLocalY(Door,0f,moveTime).setEaseInSine();
        }
    }

    public void PrepareToGenerateEnemy()
    {
        
    }
}
