using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShadow : MonoBehaviour
{
    private float y;

    void Start()
    {
        y = transform.position.y;
    }
    
    void Update()
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
