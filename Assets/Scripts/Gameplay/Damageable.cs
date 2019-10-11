using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public int maxHitPoints;
    public int currentHealth { get; private set; }
    public bool invulnerableAfterDamage = true;
    public float invulnerabilityDuration = 1f;
    [Tooltip("The angle from the which that damageable is hitable. Always in the world XZ plane, with the forward being rotate by hitForwardRoation")]
    [Range(0.0f, 360.0f)]
    public float hitAngle = 360.0f;
    [Tooltip("Allow to rotate the world forward vector of the damageable used to define the hitAngle zone")]
    [Range(0.0f, 360.0f)]
    public float hitForwardRotation = 360.0f;
    public int flinchThreshold = 1;
    public int knockBackThreshold = 3;
    public int knockOutThreshold = 3;
    public int knockOutLimit = 6;

    private Actor actor;
    private Rigidbody rb;

    private bool m_Invulnerable;
    private float m_InulnerabilityTimer;
    private int knockOutBuildUp;

    public UnityEvent OnSpawn, OnDeath, OnReceiveDamage;

    System.Action schedule;

    void Start()
    {
        currentHealth = maxHitPoints;
        actor = GetComponent<Actor>();
        rb = GetComponent<Rigidbody>();
        OnSpawn.Invoke();
    }

    public void TakeHit(Damager damager)
    {
        //for hit angle and stuff
        Vector3 forward = transform.forward;
        forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

        //we project the direction to damager to the plane formed by the direction of damage
        Vector3 positionToDamager = damager.transform.position - transform.position;
        positionToDamager -= transform.up * Vector3.Dot(transform.up, positionToDamager);

        if (Vector3.Angle(forward, positionToDamager) > hitAngle * 0.5f)
        {
            return;
        }

        //check invulnerability
        if (m_Invulnerable)
        {
            return;
        }

        //damage
        currentHealth -= damager.damage;

        if (currentHealth > 0)
        {
            actor.Damaged(damager);
            OnReceiveDamage.Invoke();

            //status
            if (damager.appliedStatus > 0)
            {
                actor.ApplyStatus(damager.appliedStatus, damager.statusDuration);
            }

            //physical impact 
            knockOutBuildUp += damager.hitPower;
            if (knockOutLimit > 0 && knockOutThreshold > 0 && knockOutBuildUp >= knockOutLimit && damager.hitPower >= knockOutThreshold)
            {
                knockOutBuildUp = 0;
                actor.ApplyImpact(damager, "KnockOut");
            }
            else if (knockBackThreshold > 0 && damager.hitPower >= knockBackThreshold)
            {
                actor.ApplyImpact(damager, "KnockBack");
            }
            else if (flinchThreshold > 0 && damager.hitPower >= flinchThreshold)
            {
                actor.ApplyImpact(damager, "Flinch");
            }

            if (invulnerableAfterDamage)
            {
                StartCoroutine(AfterHitInvulnerability(invulnerabilityDuration));
            }
        }
        else
        {
            schedule += OnDeath.Invoke;
        }
    }

    void LateUpdate()
    {
        if (schedule != null)
        {
            schedule();
            schedule = null;
        }
    }

    public void ModifyHealth(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHitPoints);
    }

    public void EnableInvulnerability()
    {
        m_Invulnerable = true;

    }

    public void DisableInvulnerability()
    {
        m_Invulnerable = false;
    }

    private IEnumerator AfterHitInvulnerability(float duration)
    {
        EnableInvulnerability();
        yield return new WaitForSeconds(duration);
        DisableInvulnerability();
    }

    private IEnumerator KnockBack(Damager damager)
    {
        actor.ConstrainControl();
        rb.velocity = damager.transform.right * 10f * rb.mass;
        yield return new WaitForSeconds(0.5f);
        actor.ReleaseControl();
    }
}
