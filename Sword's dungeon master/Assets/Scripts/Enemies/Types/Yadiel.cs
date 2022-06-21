using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yadiel : EnemyPathFinding
{
    bool started;

    [Header("Stages")]
    [SerializeField] int currentStage = 0;
    float startHealth;

    [Space]
    [SerializeField] float speedMultiplier;
    [SerializeField] float waitBetweenAttacksTimer;
    float phaseTimer;

    [Space]
    public MeshRenderer[] eyes;
    public Material redEyes;
    public ParticleSystem attackparticle;
    public GameObject shootPrefab;
    public Transform shootLoc;

    [Header("Phases")]
    [SerializeField] int currentPhase;

    [SerializeField] GameObject bigBoss;

    public override void Awake()
    {
        base.Awake();
        startHealth = health.health;
    }
    public override void Update()
    {
        base.Update();

        if (!started && triggered)
        {
            started = true;
            AttackPhase();
        }

        if (!started)
            return;

        float healthPercentage = (health.health / startHealth) * 100;
        if (health.health <= 0)
        {
            //Stage 3
            currentStage++;
            Invoke("Dead", 2.5f);
        }
        else if(currentStage < 1 && healthPercentage < 50)
        {
            //Stage 2
            currentStage++;
            foreach (MeshRenderer renderer in eyes)
            {
                renderer.material = redEyes;
            }
            moveSpeed *= speedMultiplier;
            agent.speed *= speedMultiplier;
            anim.SetFloat("SpeedIncrease", speedMultiplier);
            //more ranged attacks and faster
        }
    }
    public override void TriggerRadiusChecks()
    {
        if (!triggered)
            return;

        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();
        Transform closestPlayer = null;
        float closestFloat = 0;
        foreach(PlayerHealth player in players)
        {
            float newDist = Vector3.Distance(transform.position, player.transform.position);
            if(closestPlayer== null)
            {
                closestPlayer = player.transform;
                closestFloat = newDist;
            }
            else if(closestFloat > newDist)
            {
                closestPlayer = player.transform;
                closestFloat = newDist;
            }
        }

        destination = closestPlayer;
    }
    void Dead()
    {
        Destroy(gameObject);
        bigBoss.SetActive(true);
    }
    IEnumerator PhaseTimer()
    {
        yield return new WaitForSeconds(phaseTimer);
        anim.SetBool("Attacking", false);
        WalkingState(false);
        yield return new WaitForSeconds(waitBetweenAttacksTimer);
        NextPhase();
    }
    void NextPhase()
    {
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
                phaseTimer = 5;
                WalkingState(true);
                anim.SetInteger("AttackId", 0);
                break;
            case 1:
                phaseTimer = 5;
                WalkingState(true);
                anim.SetInteger("AttackId", 0);
                break;
            case 2:
                phaseTimer = 5;
                WalkingState(false);
                anim.SetInteger("AttackId", 1);
                anim.SetBool("Attacking", true);
                break;
        }
        StartCoroutine(PhaseTimer());
    }
    public void PreShoot()
    {
        attackparticle.Play();
    }
    public void Shoot()
    {
        GameObject newProj = Instantiate(shootPrefab, shootLoc.position, shootLoc.rotation, null);
        newProj.GetComponent<EnemyProjectile>().enemy = destination;
    }
}
