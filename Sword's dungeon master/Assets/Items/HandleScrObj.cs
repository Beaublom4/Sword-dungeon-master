using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Handle")]
public class HandleScrObj : ScriptableObject
{
    public int id;
    public GameObject prefab;
    public int slot;
    public float cost;

    public float attackSpeed = 1;
    public float manaRegen;
    [Tooltip("0: swing, 1: stab")]
    public int attackType;
}
