using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectile;
    public float launchVelocity;
    public float mimumInterval;

    public float range { get; private set; }
    public bool ready { get; private set; }

    private float intervalTimer;

    void Start()
    {
        intervalTimer = mimumInterval;

        range = projectile.GetComponent<Damager>().maximumRange;
    }

    void Update()
    {
        if (intervalTimer < mimumInterval)
        {
            ready = false;
            intervalTimer += Time.deltaTime;
        } else
        {
            ready = true;
        }
    }

    public void Fire()
    {
        intervalTimer = 0f;
        GameObject instance = Instantiate(projectile, transform.position, transform.rotation);
        instance.GetComponent<Rigidbody>().velocity = transform.right * launchVelocity;
    }

    public void Fire(Vector3 targetPoint)
    {
        intervalTimer = 0f;
        GameObject instance = Instantiate(projectile, transform.position, transform.rotation);
        instance.transform.rotation = Quaternion.FromToRotation(instance.transform.right, targetPoint - transform.position);
        instance.transform.eulerAngles = new Vector3(instance.transform.eulerAngles.x, 0f, instance.transform.eulerAngles.z);
        instance.GetComponent<Rigidbody>().velocity = (targetPoint - transform.position).normalized * launchVelocity;
    }
}
