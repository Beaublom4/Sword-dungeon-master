using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DungeonRoom : MonoBehaviour
{
    public bool forward, back, right, left;
    public bool hasEnemies;
    [SerializeField] bool deathRoom;

    EnemyHealth[] enemies;
    Secret[] secrets;

    public Vector2 gridLoc;

    public RoomLinkedCheck linkScript;
    public bool linked;

    public GameObject door;

    public Transform miniBossSpawnLoc;

    private void Start()
    {
        secrets = GetComponentsInChildren<Secret>();
        if (hasEnemies)
        {
            DungeonManager.Instance.totalRoomsToClear++;
            enemies = GetComponentsInChildren<EnemyHealth>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ShowCollectedSecrets();
        }
    }
    public void ShowCollectedSecrets()
    {
        int secretsFound = 0;
        foreach (Secret s in secrets)
        {
            if (s == null)
                secretsFound++;
        }
        HudManager.Instance.ShowSecrets(secrets.Length, secretsFound);
    }
    public void CheckIfCleared()
    {
        if (!hasEnemies)
            return;

        foreach (EnemyHealth health in enemies)
        {
            bool isDead = false;
            if (health == null)
            {
                isDead = true;
            }
            else if (health.death)
            {
                isDead = true;
            }

            Debug.Log(gameObject.name + " is dead: " + isDead);
            if (!isDead)
            {
                return;
            }
        }
        DungeonManager.Instance.RoomCleared();
    }
}
