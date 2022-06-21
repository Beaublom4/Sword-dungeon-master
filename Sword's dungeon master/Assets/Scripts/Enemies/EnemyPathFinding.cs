using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFinding : MonoBehaviour
{
    [HideInInspector] public Transform destination;
    public float moveSpeed;

    [HideInInspector] public NavMeshAgent agent;
    Vector3 lastPos;
    [HideInInspector] public Animator anim;

    [HideInInspector] public EnemyHealth health;

    [Header("Staggering")]
    bool staggered;

    [Header("Rotation")]
    [SerializeField] Transform rotator;
    [SerializeField] float rotationSpeed;

    [Header("Triggering")]
    public TriggerRadius triggerRadius;
    [HideInInspector] public bool triggered;

    [Header("Attacking")]
    [SerializeField] AttackRadius attackRadius;
    public bool canAttack = true;
    bool attacking;
    EnemyHitBox hitBox;

    public virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        anim = GetComponent<Animator>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();
        hitBox = GetComponentInChildren<EnemyHitBox>();
        health = GetComponent<EnemyHealth>();
    }
    public virtual void Update()
    {
        if (health.death)
            return;

        if (health.staggered && !staggered)
        {
            staggered = true;
            agent.speed = 0;
            anim.speed = 0;
        }
        else if (!health.staggered && staggered)
        {
            staggered = false;
            agent.speed = moveSpeed;
            anim.speed = 1;
        }

        TriggerRadiusChecks();
        DamageRadiusCheck();

        if (destination != null)
        {
            if (attacking)
            {
                rotator.LookAt(new Vector3(destination.position.x, transform.position.y, destination.position.z));
                transform.rotation = Quaternion.Lerp(transform.rotation, rotator.rotation, rotationSpeed * Time.deltaTime);
            }

            agent.SetDestination(destination.position);
        }

        if (agent.velocity.magnitude > .2f)
        {
            anim.SetBool("Moving", true);
        }
        else
        {
            anim.SetBool("Moving", false);
        }
        lastPos = transform.position;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    public virtual void TriggerRadiusChecks()
    {
        if (triggerRadius.inRange && !triggered)
        {
            triggered = true;
            destination = triggerRadius.player;
            agent.isStopped = false;
        }
        else if (!triggerRadius.inRange && triggered)
        {
            triggered = false;
            destination = null;
            agent.isStopped = true;
        }
    }
    void DamageRadiusCheck()
    {
        if (canAttack)
        {
            if (attackRadius.inRange && !attacking)
            {
                attacking = true;
                agent.isStopped = true;
                anim.SetBool("Attacking", true);
            }
            else if (!attackRadius.inRange && attacking)
            {
                attacking = false;
                agent.isStopped = false;
                anim.SetBool("Attacking", false);
            }
        }
    }
    public void StartHit()
    {
        hitBox.canHit = true;
    }
    public void EndHit()
    {
        hitBox.canHit = false;
        hitBox.hits.Clear();
    }
    public void WalkingState(bool b)
    {
        if (b)
        {
            agent.speed = moveSpeed;
        }
        else
        {
            agent.speed = 0;
        }
    }
}
