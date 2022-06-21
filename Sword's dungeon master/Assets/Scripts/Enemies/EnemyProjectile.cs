using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Transform enemy;
    [SerializeField] float timeAlive;
    [SerializeField] float damage;
    [SerializeField] bool heatSeak;
    [SerializeField] float rotSpeed;
    [SerializeField] Transform rotator;
    [SerializeField] float moveSpeed;

    private void Start()
    {
        Destroy(gameObject, timeAlive);
    }
    private void Update()
    {
        if (heatSeak)
        {
            rotator.LookAt(new Vector3(enemy.position.x, enemy.position.y + 1, enemy.position.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rotator.rotation, rotSpeed * Time.deltaTime);
            transform.Translate(0, 0, moveSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        if(other.tag == "Player")
        {
            other.GetComponentInParent<PlayerHealth>().GetHit(damage);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
