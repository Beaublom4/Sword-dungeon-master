using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName ="Weapon/Magic")]
public class MagicScrObj : ScriptableObject
{
    public int id;
    public Sprite sprite;
    public float damage;
    public float manaUsage;
    public float speed;
    public float castTime;
    public float timeAlive;
    public float magicType;

    public GameObject magicPrefab;
}
