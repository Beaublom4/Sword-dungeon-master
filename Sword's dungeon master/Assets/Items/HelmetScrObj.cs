using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Armor/Helmet")]
public class HelmetScrObj : ScriptableObject
{
    public int id;
    public GameObject prefab;
    public int slot;
    public float cost;

    public float addHealth;
    public float regeneration;
    public float defence;
    public float mana;
}
