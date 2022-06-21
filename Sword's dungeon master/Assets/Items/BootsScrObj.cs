using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Armor/Boots")]
public class BootsScrObj : ScriptableObject
{
    public int id;
    public GameObject prefab;
    public int slot;
    public float cost;

    public float addHealth;
    public float regeneration;
    public float defence;
    public float moveSpeedBoost;
}
