using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float health;
    [HideInInspector] public bool death;
    [SerializeField] GameObject[] rigidBodies;
    [SerializeField] GameObject[] radias;

    [Header("Boss health bar")]
    [SerializeField] bool isBoss;
    [SerializeField] string bossName;
    bool hasBar;
    float startHealth;

    [Header("Drops")]
    [SerializeField] int minCoinsDropped;
    [SerializeField] int maxCoinsDropped;
    [System.Serializable]
    public class Drop
    {
        public Object item;
        public float dropChance;
    }
    [SerializeField] Drop[] drops;

    [Header("Bleeding")]
    [SerializeField] GameObject dmgShowPrefab;
    float bleedDmg;
    float bleedTimer;

    [Header("HitFlash")]
    [SerializeField] MeshRenderer[] hitShowRenderers;
    [SerializeField] Material hitMat;
    IEnumerator hitCoroutine;

    [Header("Staggering")]
    [SerializeField] bool cantBeStaggered;
    public bool staggered;
    IEnumerator staggerCoroutine;

    private void Start()
    {
        InvokeRepeating("BleedInterfall", 0, 1);
    }
    public void ActivateBossBar()
    {
        hasBar = true;
        startHealth = health;
        HudManager.Instance.EnableBossBar(true);
        HudManager.Instance.SetBossHealthBar(health, startHealth, bossName);
        HudManager.Instance.SplashScreen(bossName);
    }
    void BleedInterfall()
    {
        if (death)
            return;

        if (bleedTimer > 0)
        {
            GetHit(bleedDmg, true, 0, 0);
            DamageDisplay dmg = Instantiate(dmgShowPrefab, transform.position, Quaternion.identity, null).GetComponent<DamageDisplay>();
            dmg.dmg.text = (bleedDmg).ToString();
            bleedTimer -= 1;
        }
    }
    public void GetHit(float damage, bool hitFlash, float bleedTime, float staggerTime)
    {
        if (death)
            return;

        health -= damage;

        if(hasBar)
            HudManager.Instance.SetBossHealthBar(health, startHealth, bossName);

        if (bleedTime > 0)
        {
            bleedDmg = damage / 10;
            bleedTimer += bleedTime;
            if (bleedTimer > 9)
                bleedTimer = 9;
        }

        if (hitFlash)
        {
            if (hitCoroutine != null)
                StopCoroutine(hitCoroutine);
            hitCoroutine = HitFlash();
            StartCoroutine(hitCoroutine);
        }

        if (cantBeStaggered)
        {
            if (staggerCoroutine != null)
                StopCoroutine(staggerCoroutine);
            staggerCoroutine = Stagger(staggerTime);
            StartCoroutine(staggerCoroutine);
        }
       
        if (health <= 0)
        {
            death = true;

            Animator anim = GetComponent<Animator>();
            if (anim == null)
                anim = GetComponentInChildren<Animator>();

            if(!isBoss)
                anim.enabled = false;

            if (GetComponent<NavMeshAgent>()) GetComponent<NavMeshAgent>().isStopped = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            CancelInvoke();

            foreach (GameObject g in radias)
            {
                g.SetActive(false);
            }

            foreach(GameObject g in rigidBodies)
            {
                if(g.GetComponent<Collider>())
                    g.GetComponent<Collider>().isTrigger = false;
                g.GetComponent<Rigidbody>().useGravity = true;
                g.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                g.GetComponent<Rigidbody>().isKinematic = false;
            }

            GetComponentInParent<DungeonRoom>()?.CheckIfCleared();
            DropItem();
            if(!isBoss)
                Destroy(gameObject, 3);
        }
    }
    void DropItem()
    {
        int coins = Random.Range(minCoinsDropped, maxCoinsDropped);
        Inventory.coins += coins;
        HudManager.Instance.ShowCoins();

        float randomDrop = Random.Range(0, 100);
        float latestDrop = 0;
        foreach (Drop drop in drops)
        {
            latestDrop += drop.dropChance;
            if (randomDrop <= latestDrop)
            {
                if(drop.item != null)
                    Inventory.AddItem(drop.item);
                return;
            }
        }
    }
    IEnumerator Stagger(float staggerTime)
    {
        staggered = true;
        yield return new WaitForSeconds(staggerTime);
        staggered = false;
    }
    IEnumerator HitFlash()
    {
        foreach (MeshRenderer mr in hitShowRenderers)
        {
            mr.material = hitMat;
        }
        yield return new WaitForSeconds(.2f);
        foreach (MeshRenderer mr in hitShowRenderers)
        {
            if(mr.GetComponent<HitColor>())
                mr.GetComponent<HitColor>().NormalMat();
        }
    }
}
