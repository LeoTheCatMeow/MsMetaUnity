using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSMB : StateMachineBehaviour
{
    [Tooltip("Usually for damage reaction such as flinch etc..")]
    public bool invulnerableThroughout;

    [Tooltip("Motion can't be affected by further inputs until finished")]
    public bool constrainInputs;

    [Tooltip("Set velocity to 0 on start")]
    public bool resetVelocity;

    [Tooltip("Automatically toggle damagers off when the state exits")]
    public bool disableDamagerOnExit;

    [Tooltip("Durations at which damage is toggled on and off, normalized, -0.1 if not apply")]
    [Range(-0.1f, 1)]
    public float enableDamageAt;
    [Range(-0.1f, 1)]
    public float disableDamageAt;

    public delegate void SimpleDelegate();
    public SimpleDelegate BeginInvulnarability;
    public SimpleDelegate EndInvulnarability;
    public SimpleDelegate BeginConstrain;
    public SimpleDelegate EndConstrain;
    public SimpleDelegate ResetVelocity;

    public delegate void ParamDelegate(int i, bool state);
    public ParamDelegate ToggleDamager;

    private bool damagersOn;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (constrainInputs)
        {
            BeginConstrain();
        }
        if (invulnerableThroughout)
        {
            BeginInvulnarability();
        }
        if (resetVelocity)
        {
            ResetVelocity();
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enableDamageAt >= 0 && stateInfo.normalizedTime >= enableDamageAt && !damagersOn) {
            ToggleDamager(stateInfo.shortNameHash, true);
            damagersOn = true;
        }
        if (disableDamageAt >= 0 && stateInfo.normalizedTime >= disableDamageAt && damagersOn)
        {
            ToggleDamager(stateInfo.shortNameHash, false);
            damagersOn = false;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (constrainInputs)
        {
            EndConstrain();
        }
        if (invulnerableThroughout)
        {
            EndInvulnarability();
        }
        if (disableDamagerOnExit)
        {
            ToggleDamager(stateInfo.shortNameHash, false);
            damagersOn = false;
        }
    }
}
