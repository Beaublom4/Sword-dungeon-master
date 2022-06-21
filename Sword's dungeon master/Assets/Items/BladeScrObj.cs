using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Weapon/Blade")]
public class BladeScrObj : ScriptableObject
{
    public int id;
    public GameObject prefab;
    public int slot;
    public float cost;

    public float damage;
    public float critDamage;
    public float critChance;
    public float reinforceMagic;
    public bool bleeding;
    public float bleedTime;
    public float lifeStealPercentage;
}
