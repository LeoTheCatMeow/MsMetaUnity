using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backdraft : Actor
{
    //reference
    private GameObject target;
    private ProjectileLauncher fireBall;

    void Start()
    {
        InitializeActor();
        fireBall = projectileLaunchers[0];
        target = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(target.name);
    }

    void Update()
    {
        if (fireBall.ready && status.IsNot(Status.constrained))
        {
            CastFireball();
        }
    }

    public void CastFireball()
    {
        status |= Status.constrained;
        anim.SetTrigger("CastFireball");
    }

    //animation event
    public void ReleaseFireball()
    {
        fireBall.Fire(target.transform.position + new Vector3(0f, 8f, 0f));
    }

    public void FinishFireball()
    {
        status &= ~Status.constrained;
    }
}
