using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsMeta : Actor
{
    //custom values
    public float groundSpeed, jumpSpeed;
    public enum MsMetaPerks
    {
        doubleJump, unknown
    }
    public MsMetaPerks perk;
    public KeyCode JumpKey, PunchKey, KickKey;
    public AudioClip JumpSound;

    //unique
    private Damager punch;
    private Damager kick;
    private Damager jumpKick;

    private readonly int punchHash = Animator.StringToHash("Punch");
    private readonly int kickHash = Animator.StringToHash("Kick");
    private readonly int jumpKickHash = Animator.StringToHash("JumpKick");
    private readonly int doubleJumpHash = Animator.StringToHash("DoubleJump");

    void Start()
    {
        InitializeActor();
        punch = damagers[0];
        kick = damagers[1];
        jumpKick = damagers[2];
    }

    void Update()
    {
        UpdateStatus();

        if (status.IsNot(Status.constrained))
        {
            HandleInputs();
        }

        UpdateAnimatorParam();
    }

    void HandleInputs()
    {
        //Axis
        Vector2 inputVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * groundSpeed;
        Vector3 velocity = rb.velocity;
        velocity.Set(inputVelocity.x, rb.velocity.y, inputVelocity.y);
        AdjustFacing(inputVelocity);

        //Keys
        if (Input.GetKeyDown(JumpKey) && status.IsNot(Status.rooted))
        {
            if (status.Is(Status.canJump))
            {
                status &= ~Status.canJump;
                velocity.Set(rb.velocity.x, jumpSpeed, rb.velocity.z);
                SoundManager.PlayOnce(JumpSound);
            } else if (status.Is(Status.canDoubleJump))
            {
                status &= ~Status.canDoubleJump;
                velocity.Set(rb.velocity.x, jumpSpeed, rb.velocity.z);
                anim.SetTrigger(doubleJumpHash);
                SoundManager.PlayOnce(JumpSound); ;
            }
        } else if (Input.GetKeyDown(PunchKey) && status.IsNot(Status.inAir))
        {
            anim.SetTrigger(punchHash);
        } else if (Input.GetKeyDown(KickKey))
        {
            if (status.Is(Status.inAir))
            {
                status &= ~Status.canDoubleJump;
                anim.SetTrigger(jumpKickHash);
            }
            else
            {
                anim.SetTrigger(kickHash);
            }
        }
        AdjustVelocity(velocity);
    }

    void UpdateAnimatorParam()
    {
        anim.SetFloat("XSpeed", rb.velocity.x);
        anim.SetFloat("ZSpeed", rb.velocity.z);
        anim.SetFloat("YSpeed", rb.velocity.y);
        anim.SetBool("InAir", status.Is(Status.inAir));
    }

    public override void ToggleDamager(int i, bool state)
    {
        if (i == punchHash)
        {
            punch.active = state;
        } else if (i == kickHash)
        {
            kick.active = state;
        } else if (i == jumpKickHash)
        {
            jumpKick.active = state;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Vector3.Distance(collision.GetContact(0).point, transform.position) < 1f)
        {
            status |= Status.canJump;
            if (perk == MsMetaPerks.doubleJump)
            {
                status |= Status.canDoubleJump;
            }
        }
    }
}
