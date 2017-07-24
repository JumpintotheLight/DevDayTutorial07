using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Extend transform.Clear() to destroy all children of a transform
public static class TransformEx {
	public static Transform Clear(this Transform transform)
	{
		foreach (Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}
		return transform;
	}
}
