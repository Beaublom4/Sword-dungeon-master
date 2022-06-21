using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public static Stats Instance;

    public float defaultHealth;
    public float defaultRegen;
    public float defaultDefence;
    public float defaultMana;
    public float defaultManaRegen;
    public float defaultProtection;
    public float defaultSpProt;
    public float defaultSpeed;

    [Header("DisplayStats")]
    [SerializeField] float currentHealth;
    [SerializeField] float currentRegen;
    [SerializeField] float currentDefence;
    [SerializeField] float currentMana;
    [SerializeField] float currentManaRegen;
    [SerializeField] float currentProtection;
    [SerializeField] float currentSpProt;
    [SerializeField] float currentSpeed;

    public static float health;
    public static float healthRegen;
    public static float defence;
    public static float mana;
    public static float manaRegen;
    public static float protection;
    public static float spProt;
    public static float speed;

    static bool changed;

    private void Awake()
    {
        changed = true;

        health = defaultHealth;
        healthRegen = defaultRegen;
        defence = defaultDefence;
        mana = defaultMana;
        protection = defaultProtection;
        spProt = defaultSpProt;
        speed = defaultSpeed;
        manaRegen = defaultManaRegen;
    }
    public static void ChangeStats(float addHealth, float addRegen, float addDefence, float addMana, float addManaRegen, float addProtection, float addSpProt, float addSpeed)
    {
        health += addHealth;
        healthRegen += addRegen;
        defence += addDefence;
        mana += addMana;
        manaRegen += addManaRegen;
        protection += addProtection;
        spProt += addSpProt;
        speed += addSpeed;

        changed = true;
    }
    private void Update()
    {
        if (changed)
        {
            changed = false;
            currentHealth = health;
            currentRegen = healthRegen;
            currentDefence = defence;
            currentMana = mana;
            currentManaRegen = manaRegen;
            currentProtection = protection;
            currentSpProt = spProt;
            currentSpeed = speed;
        }
    }
}
