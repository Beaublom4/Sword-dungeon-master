using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Guard")]
public class GuardScrObj : ScriptableObject
{
    public int id;
    public GameObject prefab;
    public int slot;
    public float cost;

    public float staggerTime = .2f;
    public float manaDiscount;
}
