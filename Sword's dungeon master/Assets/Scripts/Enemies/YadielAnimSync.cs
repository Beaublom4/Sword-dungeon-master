using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YadielAnimSync : MonoBehaviour
{
    [SerializeField] Yadiel yadiel;

    public void PreShoot()
    {
        yadiel.PreShoot();
    }
    public void Shoot()
    {
        yadiel.Shoot();
    }
}
