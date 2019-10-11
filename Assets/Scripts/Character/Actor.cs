using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Actor : MonoBehaviour
{
    //status
    [Flags]
    public enum Status : int
    {
        none = 0,
        inAir = 1 << 0,
        constrained = 1 << 1,
        rooted = 1 << 2,
        canJump = 1 << 3,
        canDoubleJump = 1 << 4,
        dead = 1 << 5
    }
    protected Status status = Status.none;
    protected float[] statusDurations = new float[Enum.GetNames(typeof(Status)).Length];

    //references
    protected Animator anim;
    protected Rigidbody rb;
    protected Collider cldr;
    protected Damageable db;
    protected Damager[] damagers;
    protected ProjectileLauncher[] projectileLaunchers;

    protected void InitializeActor()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        cldr = GetComponent<Collider>();
        db = GetComponent<Damageable>();
        damagers = GetComponentsInChildren<Damager>();
        projectileLaunchers = GetComponentsInChildren<ProjectileLauncher>();

        GenericSMB[] smbs = anim.GetBehaviours<GenericSMB>();
        foreach (GenericSMB smb in smbs)
        {
            smb.BeginInvulnarability = db.EnableInvulnerability;
            smb.EndInvulnarability = db.DisableInvulnerability;
            smb.BeginConstrain = ConstrainControl;
            smb.EndConstrain = ReleaseControl;
            smb.ResetVelocity = ResetVelocity;
            smb.ToggleDamager = ToggleDamager;
        }
    }

    protected void AdjustFacing (Vector3 reference)
    {
        if (reference.x > 0f)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
        }
        else if (reference.x < 0f)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180f, transform.eulerAngles.z);
        }
    }

    protected void AdjustVelocity (Vector3 velocity)
    {
        if (status.Is(Status.rooted))
        {
            velocity.Set(0f, Mathf.Min(velocity.y, 0f), 0f);
        }
        rb.velocity = velocity;
    }

    public virtual void UpdateStatus()
    {
        if (Mathf.Abs(rb.velocity.y) > 1f)
        {
            status |= Status.inAir;
        } else
        {
            status &= ~Status.inAir;
        }
         
        for (int i = 0; i < statusDurations.Length; i++)
        {
            if (statusDurations[i] == 0f)
            {
                continue;
            }
            statusDurations[i] -= Time.deltaTime;
            if (statusDurations[i] < 1f)
            {
                statusDurations[i] = 0f;
                status &= ~(Status)(1 << (i + 1));
            } 
        }
    }

    public virtual void ApplyStatus (Status s, float duration)
    {
        status |= s;
        statusDurations[(int)Mathf.Log((int)s, 2) - 1] = duration + 1;
    }
    
    public virtual void ApplyImpact(Damager damager, string type)
    {
        ConstrainControl();
        AdjustFacing(-damager.transform.right);
        anim.SetTrigger(type);
    }

    public virtual void Damaged(Damager damager)
    {
        anim.SetTrigger("Damaged");
    }

    public virtual void ToggleDamager(int i, bool state) { }

    public virtual void OnDeath()
    {
        status |= Status.dead;
        rb.isKinematic = true;
        cldr.enabled = false;
        foreach (Damager x in damagers)
        {
            x.gameObject.SetActive(false);
        }

        foreach (ProjectileLauncher x in projectileLaunchers) {
            x.gameObject.SetActive(false);
        }
        anim.SetTrigger("Die");
    }

    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }

    public void ConstrainControl()
    {
        status |= Status.constrained;
    }

    public void ReleaseControl()
    {
        status &= ~Status.constrained;
    }
}

static class EnumExtensions
{
    public static bool Is(this Actor.Status status, Actor.Status other)
    {
        return (status & other) != 0;
    }

    public static bool IsNot(this Actor.Status status, Actor.Status other)
    {
        return (status & other) == 0;
    }
}