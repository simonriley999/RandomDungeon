using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerArea : MonoBehaviour
{
    public DoorController doorController;
    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            DoorEventSystem.instance.DoorOpen(doorController.doorId,doorController.belongedRoom);
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.tag == "Player")
        {
            DoorEventSystem.instance.DoorClose(doorController.doorId,doorController.belongedRoom);
        }
    }
}
