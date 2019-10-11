using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DepthSpriteRenderer : MonoBehaviour {

    public int additionalPriority;
    private SpriteRenderer sprite;

	void Start() {
		sprite = GetComponent<SpriteRenderer>();
	}
	void Update() {
		sprite.sortingOrder = 30 - (int)transform.position.z + additionalPriority;
	}
}
