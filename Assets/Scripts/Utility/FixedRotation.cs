using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedRotation : MonoBehaviour
{

    void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
