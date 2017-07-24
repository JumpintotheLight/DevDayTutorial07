using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class VRSwipeDetector : MonoBehaviour {

	public PlayMakerFSM fsm;

	public KeyCode leftSwipeTestKey = KeyCode.LeftArrow;
	public KeyCode rightSwipeTestKey = KeyCode.RightArrow;

	bool touchStarted = false;
	float startPosX, startPosY;

	float stepX = 30.7f;
	float stepY = 30.7f;

	void Start () {
		touchStarted = false;
	}
	
	void Update () {
		// Test keys for PC and Editor
		if (Input.GetKeyDown (leftSwipeTestKey)) {
			if (fsm!=null) fsm.SendEvent ("Prev");
			if (fsm!=null) fsm.SendEvent ("SwipeBack");
		}
		if (Input.GetKeyDown (rightSwipeTestKey)) {
			if (fsm!=null) fsm.SendEvent ("Next");
			if (fsm!=null) fsm.SendEvent ("SwipeForward");
		}

		if (OVRManager.instance==null && Input.GetMouseButtonDown(0)) {
			// Cardboard - go to next when button pressed
			if (fsm!=null) fsm.SendEvent ("Next");
			if (fsm!=null) fsm.SendEvent ("SwipeForward");
		} else {
			// Detect swipes
			if (Input.GetMouseButton (0)) {
				var newX = Input.GetAxis ("Mouse X");
				var newY = Input.GetAxis ("Mouse Y");
				if (touchStarted) {
					// Horizontal swipes
					if (newX - startPosX > stepX) {
						// Swipe forward
						print ("swipe back");
						touchStarted = false;
						if (fsm!=null) fsm.SendEvent ("Prev");
						if (fsm!=null) fsm.SendEvent ("SwipeBack");
					} else {
						if (startPosX - newX > stepX) {
							// Swipe back
							print ("swipe forward");
							touchStarted = false;
							if (fsm!=null) fsm.SendEvent ("Next");
							if (fsm!=null) fsm.SendEvent ("SwipeForward");
						}
					}

					// Vertical swipes
					if (newY - startPosY > stepY) {
						// Swipe up
						print ("swipe up");
						touchStarted = false;
						if (fsm!=null) fsm.SendEvent ("SwipeUp");
					} else {
						if (startPosY - newY > stepY) {
							// Swipe down
							print ("swipe down");
							touchStarted = false;
							if (fsm!=null) fsm.SendEvent ("SwipeDown");
						}
					}
				} else {
					touchStarted = true;
					startPosX = newX;
					startPosY = newY;
				}
			} else touchStarted = false;		
		}
	}
}
