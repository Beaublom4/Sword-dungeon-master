using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    public bool canHit;
    public List<Transform> hits = new List<Transform>();
    public float damage;
    private void OnTriggerEnter(Collider other)
    {
        if (!canHit)
            return;
        if(other.tag == "Player")
        {
            foreach(Transform t in hits)
            {
                if (t == other.transform.root)
                    return;
            }
            hits.Add(other.transform.root);
            other.transform.root.GetComponent<PlayerHealth>().GetHit(damage);
        }
    }
}
