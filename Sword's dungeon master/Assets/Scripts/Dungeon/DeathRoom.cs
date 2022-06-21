using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRoom : MonoBehaviour
{
    [SerializeField] int enemieSpawnCount;
    [SerializeField] GameObject[] enemyTypes;
    [SerializeField] int enemiesAlive = 100;

    [SerializeField] Transform spawnPoint;
    [SerializeField] ParticleSystem spawnParticle;

    [SerializeField] float spawnerDelay;
    float timer;

    bool canCollide = true, isTriggered;

    [SerializeField] GameObject portal;

    private void OnTriggerEnter(Collider other)
    {
        if (!canCollide)
            return;

        if(other.tag == "Player")
        {
            canCollide = false;
            isTriggered = true;
        }
    }
    private void Update()
    {
        if (isTriggered)
        {
            if (enemieSpawnCount > 0)
            {
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    spawnParticle.Play();
                    Invoke("SpawnEnemy", 1);
                    timer = spawnerDelay;
                }
            }
            else
            {
                isTriggered = false;
                enemiesAlive -= 100;
            }
        }
        if(enemiesAlive <= 0)
        {
            portal.SetActive(true);
            Destroy(gameObject);
        }
    }
    void SpawnEnemy()
    {
        int randomSpawn = Random.Range(0, enemyTypes.Length);
        GameObject newEnemy = Instantiate(enemyTypes[randomSpawn], spawnPoint);
        newEnemy.GetComponent<DeathRoomEnemy>().deathRoom = this;
        enemiesAlive++;
        enemieSpawnCount--;
    }
    public void EnemyDeath()
    {
        enemiesAlive--;
    }
}
