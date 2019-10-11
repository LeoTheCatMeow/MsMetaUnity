using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderDrone : Actor
{
    public float groundSpeed;
    public bool hostile = true;
    public float aggroRadius;
    public bool allowPatrol = true;
    public float patrolRadius;
    public float deathRemovalDelay;
    public AudioClip webLaunchSound;

    private GameObject target;
    private Vector2 randomPatrolPoint; 

    //reference
    private Vector2 spawnOrigin;
    private ProjectileLauncher spiderWeb;

    //AI 
    private float AIUpTime = 3f;
    private float AIDownTime = 0.6f;
    private float AICurrentUpTime = 0f;
    private float AICurrentDownTime = 0f;
    private float AIPathStrategy;
    private float AIActionStrategy;
   
    void Start()
    {
        InitializeActor();
        spiderWeb = projectileLaunchers[0];
        target = GameObject.FindGameObjectWithTag("Player");
        spawnOrigin = new Vector2(transform.position.x, transform.position.z);
        AIRandomPatrolPoint();
    }

    void Update()
    {
        if (status.Is(Status.dead))
        {
            return;
        }

        AITimeKeeping();
        UpdateStatus();

        if (status.IsNot(Status.constrained))
        {
            AIAction();
        }    
    }

    private void AITimeKeeping()
    {
        if (AICurrentUpTime < AIUpTime)
        {
            AICurrentUpTime += Time.deltaTime;
        }
        else
        {
            AICurrentDownTime += Time.deltaTime;
        }
    }

    private void AIAction()
    {
        if (AICurrentUpTime < AIUpTime)
        {
            Vector3 targetVector = target.transform.position - transform.position;
            targetVector.Set(targetVector.x, 0f, targetVector.z);

            if (targetVector.magnitude <= aggroRadius && hostile)
            {
                Vector3 direction = targetVector.normalized;
                float distance = targetVector.magnitude;
                Bounds targetBounds = target.GetComponent<Collider>().bounds;

                //face target 
                AdjustFacing(targetVector);
               
                if (AIPathStrategy < 0.4f)
                {
                    //approach x
                    direction = new Vector3(direction.x, 0f, direction.z / 3f).normalized;
                } else if (AIPathStrategy <0.8f)
                {
                    //approach z
                    direction = new Vector3(direction.x / 3f, 0f, direction.z).normalized;
                } else 
                {
                    //align z but create distance for ranged attack
                    direction = new Vector3(-direction.x, 0f, direction.z);
                    if (distance > aggroRadius * 0.6f)
                    {
                        direction = Vector3.zero;
                    }
                }
                if (distance > targetBounds.extents.x + cldr.bounds.extents.x && direction != Vector3.zero)
                {
                    rb.velocity = direction * groundSpeed;
                }
                if (Mathf.Abs(targetVector.x) < spiderWeb.range && Mathf.Abs(targetVector.z) < targetBounds.extents.z && spiderWeb.ready && AIActionStrategy > 0.6f)
                {
                    spiderWeb.Fire();
                    SoundManager.PlayOnce(webLaunchSound);
                }
            }
            else
            {
                if (allowPatrol)
                {
                    Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);
                    Vector2 velocity2D = Vector2.zero;
                    if ((randomPatrolPoint - currentPos).magnitude > 0.5f)
                    {
                        velocity2D = (randomPatrolPoint - currentPos).normalized * groundSpeed;
                        rb.velocity = new Vector3(velocity2D.x, rb.velocity.y, velocity2D.y);
                    }
                    AdjustFacing(velocity2D);
                }   
            }
        } else
        {
            if (AICurrentDownTime >= AIDownTime)
            {
                AICurrentDownTime = 0f;
                AICurrentUpTime = 0f;
                AIPathStrategy = Random.value;
                AIActionStrategy = Random.value;
                AIRandomPatrolPoint();
            }
        }
    }

    private void AIRandomPatrolPoint()
    {
        Vector2 randomCircle = Random.insideUnitCircle;
        randomCircle.Set(randomCircle.x, randomCircle.y / 2f);
        randomPatrolPoint = spawnOrigin + randomCircle * patrolRadius;
    }

    public override void ApplyImpact(Damager damager, string type)
    {
        base.ApplyImpact(damager, type);
        if (type == "KnockBack" || type == "KnockOut")
        {
            rb.velocity = damager.transform.right * 15f;
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        GetComponent<AudioSource>().Stop();
        Destroy(gameObject, deathRemovalDelay);
    }
}
