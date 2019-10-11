using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform target;
    [Tooltip("Extra space given to the direction the target's X axis is facing")]
    public float offset;
    [Tooltip("Right limit of camera motion")]
	public float maxX;
    [Tooltip("Left limit of camera motion")]
	public float minX;

    private float initialY;

    void Start()
    {
        initialY = transform.position.y;
    }

    void Update() {
        float x = Mathf.MoveTowards(transform.position.x, target.position.x + target.right.x * offset, 0.1f);
        float y = Mathf.MoveTowards(transform.position.y - initialY, target.position.y, 0.1f);
        x = Mathf.Clamp(x, minX, maxX);
        transform.position = new Vector3(x, y + initialY, transform.position.z );
	}
}
