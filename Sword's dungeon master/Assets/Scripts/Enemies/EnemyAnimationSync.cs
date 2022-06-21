using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationSync : MonoBehaviour
{
    [SerializeField] EnemyHitBox[] hitboxes;
    public void CanHit()
    {
        foreach(EnemyHitBox box in hitboxes)
        {
            box.canHit = true;
        }
    }
    public void StopHit()
    {
        foreach (EnemyHitBox box in hitboxes)
        {
            box.canHit = false;
            box.hits.Clear();
        }
    }
}
