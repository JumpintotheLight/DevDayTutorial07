using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchScrollController : MonoBehaviour {

	Vector2 firstPressPos, secondPressPos, currentSwipe;
	Vector2 touchPos;
	bool dragStarted = false;
	float stepX, stepY;

	Vector3 lastRotation;
	Touch t;

	public Transform target;
	public float speed = 1.0f;

	void Start()
	{
		stepX = (float)Screen.width * 0.05f;
		stepY = (float)Screen.height * 0.05f;
	}

	void Update () {
		bool aTouch = false;

		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) {
			if (hit.collider.gameObject == this.gameObject) {
				//Debug.Log("Hit " + this.gameObject.name);

				if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer) {
					aTouch = Input.GetMouseButton (0);
					touchPos = Input.mousePosition;
				} else {
					aTouch = Input.touches.Length > 0;
					if (aTouch) {
						t = Input.GetTouch (0);
						touchPos = Input.touches [0].position;
					}
				}

				if (aTouch) {
					if (dragStarted) {
						float rotX = (touchPos.x - firstPressPos.x) / stepX * speed;
						float rotY = -(touchPos.y - firstPressPos.y) / stepY * speed;
						var rot = new Vector3 (lastRotation.x + rotY, lastRotation.y + rotX, lastRotation.z);
						target.localRotation = Quaternion.Euler (rot);
					} else {
						dragStarted = true;
						firstPressPos = touchPos;
						lastRotation = target.localEulerAngles;
					}
				} else dragStarted = false;
			}
		}
	}
}
