using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YadielBig : MonoBehaviour
{
    float phaseTimer;
    [SerializeField] float waitBetweenAttacksTimer;

    [SerializeField] float currentPhase;
    [SerializeReference] YadielsPillarManager pillars;

    [Header("Air attack")]
    [SerializeField] bool doAirAttack;
    [SerializeField] float airAttackAddTime;
    float airAttackTimer;
    [SerializeField] ParticleSystem beam, ball;
    [SerializeField] GameObject magicPrefab;
    [SerializeField] Transform airAttackSpawnPoint;

    [Header("Ground Attack")]
    [SerializeField] float goUpTime, goDownTime, upTime;
    [SerializeField] Transform groundAttackObj1, groundAttackObj2, groundAttackObj3;
    [SerializeField] Transform groundAttackObj1Loc, groundAttackObj2Loc, groundAttackObj3Loc;
    [SerializeField] Transform groundAttackObj1End, groundAttackObj2End, groundAttackObj3End;
    [SerializeField] ParticleSystem[] groundAttack1Particle, groundAttack2Particle, groundAttack3Particle;
    [SerializeField] ParticleSystem groundAttackParticle;

    [Header("Side attack")]
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject cubeMagic;

    List<Transform> spawnedCubes = new List<Transform>();
    [SerializeField] float cubeMoveSpeed;
    
    bool doSideAttack;
    [Header("SplashScreen")]
    [SerializeField] GameObject splashScreenCam;
    [SerializeField] GameObject camPrefab;

    Animator anim;

    [Header("Dead")]
    [SerializeReference] GameObject portalToChest;
    [SerializeField] GameObject[] disableObjs;
    bool dead;
    EnemyHealth healthScript;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        healthScript = GetComponent<EnemyHealth>();
    }
    public void SplashScreen()
    {
        Transform target = FindObjectOfType<PlayerMove>().camLoc;
        GameObject currentCam = Instantiate(camPrefab, target.position, target.rotation, null);
        currentCam.GetComponent<TitleCardCam>().target = splashScreenCam.transform;
        Destroy(currentCam, 3);
        GetComponent<EnemyHealth>().ActivateBossBar();
        pillars.PillarCooldown();
    }
    private void Update()
    {
        if (dead)
            return;
        if(healthScript.health <= 0 && !dead)
        {
            dead = true;
            anim.SetTrigger("Dead");
            DungeonManager.Instance.SpawnChests(false);
            StopAllCoroutines();
            foreach(GameObject g in disableObjs)
            {
                g.SetActive(false); 
            }
            return;
        }
        if (doAirAttack)
        {
            if(airAttackTimer > 0)
            {
                airAttackTimer -= Time.deltaTime;
            }
            else
            {
                airAttackTimer = airAttackAddTime;
                AirAttackSpawn();
            }
        }
        if (doSideAttack)
        {
            foreach(Transform t in spawnedCubes)
            {
                t.Translate(0, 0, cubeMoveSpeed * Time.deltaTime);
            }
        }
    }
    public void Dead()
    {
        portalToChest.SetActive(true);
    }
    private void Start()
    {
        Invoke("AttackPhase", 8);
    }
    IEnumerator PhaseTimer()
    {
        yield return new WaitForSeconds(phaseTimer);
        yield return new WaitForSeconds(waitBetweenAttacksTimer);
        NextPhase();
    }
    void NextPhase()
    {
        doAirAttack = false;
        anim.SetBool("Attacking", false);

        currentPhase++;
        if (currentPhase > 2)
            currentPhase = 0;

        AttackPhase();
    }
    void AttackPhase()
    {
        switch (currentPhase)
        {
            case 0:
                anim.SetInteger("AttackId", 0);
                StartCoroutine(GroundAttack());
                break;
            case 1:
                anim.SetInteger("AttackId", 1);
                anim.SetTrigger("Attack");
                phaseTimer = 10;
                StartCoroutine(PhaseTimer());
                break;
            case 2:
                anim.SetInteger("AttackId", 2);
                anim.SetTrigger("Attack");

                Invoke("SideAttack", 1);
                Invoke("SideAttack", 3);
                Invoke("SideAttackTimer", 9);
                phaseTimer = 9;
                StartCoroutine(PhaseTimer());
                break;
        }
    }
    #region Air attack
    public void StartBeam()
    {
        beam.Play();
        ball.Play();
        Invoke("WaitBeforeAirAttack", 3);
    }
    public void EndBeam()
    {
        beam.Stop();
    }
    void WaitBeforeAirAttack()
    {
        doAirAttack = true;
    }
    void AirAttackSpawn()
    {
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();

        EnemyProjectile proj = Instantiate(magicPrefab, airAttackSpawnPoint.position, airAttackSpawnPoint.rotation, null).GetComponent<EnemyProjectile>();
        proj.enemy = players[Random.Range(0, players.Length)].transform;
    }
    #endregion
    #region Ground attack
    IEnumerator GroundAttack()
    {
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
        foreach(ParticleSystem system in groundAttack1Particle)
        {
            system.Play();
        }
        yield return new WaitForSeconds(1);
        foreach (ParticleSystem system in groundAttack1Particle)
        {
            system.Stop();
        }
        float timeElapsed = 0;
        Vector3 startPos = groundAttackObj1.position;
        groundAttackObj1.GetComponent<EnemyHitBox>().canHit = true;
        while (timeElapsed < goUpTime)
        {
            groundAttackObj1.position = Vector3.Lerp(startPos, groundAttackObj1Loc.position, timeElapsed/goUpTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(upTime - goUpTime);
        timeElapsed = 0;
        startPos = groundAttackObj1.position;
        while (timeElapsed < goDownTime)
        {
            groundAttackObj1.position = Vector3.Lerp(startPos, groundAttackObj1End.position, timeElapsed / goDownTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        groundAttackObj1.GetComponent<EnemyHitBox>().canHit = false;
        yield return new WaitForSeconds(1);
        foreach (ParticleSystem system in groundAttack2Particle)
        {
            system.Play();
        }
        yield return new WaitForSeconds(1);
        foreach (ParticleSystem system in groundAttack2Particle)
        {
            system.Stop();
        }
        timeElapsed = 0;
        startPos = groundAttackObj2.position;
        groundAttackObj2.GetComponent<EnemyHitBox>().canHit = true;
        while (timeElapsed < goUpTime)
        {
            groundAttackObj2.position = Vector3.Lerp(startPos, groundAttackObj2Loc.position, timeElapsed / goUpTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(upTime - goUpTime);
        timeElapsed = 0;
        startPos = groundAttackObj2.position;
        while (timeElapsed < goDownTime)
        {
            groundAttackObj2.position = Vector3.Lerp(startPos, groundAttackObj2End.position, timeElapsed / goDownTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        groundAttackObj2.GetComponent<EnemyHitBox>().canHit = false;
        yield return new WaitForSeconds(1);
        foreach (ParticleSystem system in groundAttack3Particle)
        {
            system.Play();
        }
        yield return new WaitForSeconds(1);
        foreach (ParticleSystem system in groundAttack3Particle)
        {
            system.Stop();
        }
        timeElapsed = 0;
        startPos = groundAttackObj3.position;
        groundAttackObj3.GetComponent<EnemyHitBox>().canHit = true;
        while (timeElapsed < goUpTime)
        {
            groundAttackObj3.position = Vector3.Lerp(startPos, groundAttackObj3Loc.position, timeElapsed / goUpTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(upTime);
        timeElapsed = 0;
        startPos = groundAttackObj3.position;
        while (timeElapsed < goDownTime)
        {
            groundAttackObj3.position = Vector3.Lerp(startPos, groundAttackObj3End.position, timeElapsed / goDownTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        groundAttackObj3.GetComponent<EnemyHitBox>().canHit = false;
        phaseTimer = 1;
        StartCoroutine(PhaseTimer());
    }
    public void GroundAttackParticle()
    {
        groundAttackParticle.Play();
    }
    #endregion
    #region attack
    void SideAttack()
    {
        doSideAttack = true;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnedCubes.Add(Instantiate(cubeMagic, spawnPoints[i].position, spawnPoints[i].rotation, null).transform);
        }
    }
    void SideAttackTimer()
    {
        doSideAttack = false;
        foreach (Transform t in spawnedCubes)
        {
            Destroy(t.gameObject);
        }
        spawnedCubes.Clear();
    }
    #endregion
}
