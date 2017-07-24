// Detect swipe gestures on GearVR 
// Emulate with keyboard keys for Editor testing

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace ZefirVR {
	public class SimpleSwipeDetector : MonoBehaviour {

		public PlayMakerFSM fsm;

		public string leftSwipeEvent = "SwipeBack"; 
		public string rightSwipeEvent = "SwipeForward";
		public string upSwipeEvent = "SwipeUp";
		public string downSwipeEvent = "SwipeDown";
		public string singleTouchEvent = "SingleTouch";

		public KeyCode leftSwipeTestKey = KeyCode.LeftArrow;
		public KeyCode rightSwipeTestKey = KeyCode.RightArrow;
		public KeyCode touchTestKey = KeyCode.Space;

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
				if (fsm!=null) fsm.SendEvent (leftSwipeEvent);
				else PlayMakerFSM.BroadcastEvent (leftSwipeEvent);
			}
			if (Input.GetKeyDown (rightSwipeTestKey)) {
				if (fsm!=null) fsm.SendEvent (rightSwipeEvent);
				else PlayMakerFSM.BroadcastEvent (rightSwipeEvent);
			}
			if (Input.GetKeyDown (touchTestKey)) {
				if (fsm!=null) fsm.SendEvent (singleTouchEvent);
				else PlayMakerFSM.BroadcastEvent (singleTouchEvent);
			}

			if (OVRManager.instance==null && Input.GetMouseButtonDown(0)) {
				// Cardboard - go to next when button pressed
				if (fsm!=null) {
					fsm.SendEvent (rightSwipeEvent);
					fsm.SendEvent (singleTouchEvent);
				} else {
					PlayMakerFSM.BroadcastEvent (rightSwipeEvent);
					PlayMakerFSM.BroadcastEvent (singleTouchEvent);
				}
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
							Input.ResetInputAxes ();
							if (fsm!=null) fsm.SendEvent (leftSwipeEvent);
							else PlayMakerFSM.BroadcastEvent (leftSwipeEvent);
						} else {
							if (startPosX - newX > stepX) {
								// Swipe back
								print ("swipe forward");
								touchStarted = false;
								Input.ResetInputAxes ();
								if (fsm!=null) fsm.SendEvent (rightSwipeEvent);
								else PlayMakerFSM.BroadcastEvent (rightSwipeEvent);
							}
						}

						// Vertical swipes
						if (newY - startPosY > stepY) {
							// Swipe up
							print ("swipe up");
							touchStarted = false;
							Input.ResetInputAxes ();
							if (fsm!=null) fsm.SendEvent (upSwipeEvent);
							else PlayMakerFSM.BroadcastEvent (upSwipeEvent);
						} else {
							if (startPosY - newY > stepY) {
								// Swipe down
								print ("swipe down");
								touchStarted = false;
								Input.ResetInputAxes ();
								if (fsm!=null) fsm.SendEvent (downSwipeEvent);
								else PlayMakerFSM.BroadcastEvent (downSwipeEvent);
							}
						}
					} else {
						touchStarted = true;
						startPosX = newX;
						startPosY = newY;
						if (fsm!=null) {
							fsm.SendEvent (singleTouchEvent);
						} else {
							PlayMakerFSM.BroadcastEvent (singleTouchEvent);
						}
					}
				} else touchStarted = false;		
			}
		}
	}	
}