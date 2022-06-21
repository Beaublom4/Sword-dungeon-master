using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRoomEnemy : MonoBehaviour
{
    public DeathRoom deathRoom;
    private void OnDestroy()
    {
        deathRoom.EnemyDeath();
    }
}
