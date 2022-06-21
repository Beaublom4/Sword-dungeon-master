using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secret : MonoBehaviour
{
    public float damageBuff;
    public float healthBuff;

    [Header("Mins_Maxs")]
    [SerializeField] float minDmgBuff;
    [SerializeField] float maxDmgBuff;
    [SerializeField] float minHealthBuff;
    [SerializeField] float maxHealthBuff;

    private void Awake()
    {
        damageBuff = Random.Range(minDmgBuff, maxDmgBuff);
        healthBuff = Random.Range(minHealthBuff, maxHealthBuff);
    }
}
