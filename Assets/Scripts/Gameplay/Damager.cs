using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    public int damage = 1;
    public float minimumInterval = 0f;
    public bool damageOnAwake = false;
    public bool continuous = false;
    public bool projectile = false;
    public float maximumRange = 0f;
    public int hitPower = 1;
    public LayerMask hittableLayers;
    [Tooltip("Immediat effect upon hit, spawned at a random location within the damager's collider bound, with a random z rotation")]
    public GameObject onHitEffect;
    [Tooltip("Effect spawned at the origin of the target, no randomization of position or rotation")]
    public GameObject bestowedEffect;
    public Actor.Status appliedStatus;
    public float statusDuration;

    [System.NonSerialized]
    public List<Collider> targetsInRange = new List<Collider>();

    public bool active { get; set; }
    public bool suppressed { get; set; }

    private float intervalTimer;
    private Vector3 lastPosition;
    private float distanceTraveled;

    void Start()
    {
        if (damageOnAwake)
        {
            active = true;
        }

        intervalTimer = minimumInterval;

        if (projectile)
        {
            lastPosition = transform.position;
        }
    }

    void Update()
    {
        if (intervalTimer < minimumInterval)
        {
            intervalTimer += Time.deltaTime;
        }

        if (projectile)
        {
            distanceTraveled += (transform.position - lastPosition).magnitude;
            lastPosition = transform.position;
            if (distanceTraveled >= maximumRange)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        //include in targets
        if (!targetsInRange.Contains(other))
        {
            targetsInRange.Add(other);
        }

        //Check if active or suppressed
        if (!active || suppressed)
        {
            return;
        }

        //Check if enough time has passed to damage again
        if (intervalTimer >= minimumInterval)
        {
            intervalTimer = 0f;
        }
        else
        {
            return;
        }

        //Check if hit object is hittable
        if (hittableLayers == (hittableLayers | (1 << other.gameObject.layer)))
        {
            if (onHitEffect != null)
            {
                Bounds effectSpawnRegion = GetComponent<Collider>().bounds;
                Vector3 randomSpawnPoint = effectSpawnRegion.center + 
                new Vector3(Random.Range(-effectSpawnRegion.extents.x, effectSpawnRegion.extents.x), 
                            Random.Range(-effectSpawnRegion.extents.y, effectSpawnRegion.extents.y), 0);
                Vector3 randomRotation = new Vector3(0f, 0f, Random.Range(0f, 360f));
                Instantiate(onHitEffect, randomSpawnPoint, Quaternion.Euler(randomRotation));
            }

            if (bestowedEffect != null)
            {
                Instantiate(bestowedEffect, other.transform);
            }

            //Check if the other object can receive damage
            Damageable damageable = other.GetComponent<Damageable>();
            if (damageable != null)
            {
                damageable.TakeHit(this);
            }

            if (!continuous)
            {
                active = false;

                if (projectile)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        targetsInRange.Remove(other);
    }
}
