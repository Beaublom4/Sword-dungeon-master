using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLinkedCheck : MonoBehaviour
{
    public DungeonRoom[] rooms;
    DungeonRoom yourRoom;
    private void OnEnable()
    {
        yourRoom = GetComponent<DungeonRoom>();
    }
    void Update()
    {
        foreach(DungeonRoom room in rooms)
        {
            if(room != null)
            {
                if (room.linked)
                {
                    yourRoom.linked = true;
                    this.enabled = false;
                }
            }
        }
    }
}
