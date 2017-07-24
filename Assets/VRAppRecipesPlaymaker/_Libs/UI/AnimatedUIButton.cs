using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ZefirVR {

	public class AnimatedUIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler {

		public UnityEvent triggerEvent;

		[Tooltip("When button is clicked, scale it to this factor, 0 to disable")]
		public float clickScaleFactor = 1.3f;

		[Tooltip("How long to animate")]
		public float animationTime = 0.3f;

		[Tooltip("Select elements that will be visible by default")]
		public GameObject[] defaultElements;

		[Tooltip("Select elements that will be visible when button is hovered")]
		public GameObject[] selectedElements;

		[Tooltip("Select elements that will be scaled when hovered")]
		public Transform[] scaleElements;
		public float scaleElementFactor = 1.0f;

		[Tooltip("Select elements that will be rotated when hovered")]
		public Transform[] rotateElements;
		public Vector3 rotateVector;

		private Vector3[] startRotations;
		private Vector3[] startElementScale;
		private Vector3 startScale;
		private Vector3 selectedScale;
		private float lastClickTime = 0f;
		private bool clickStarted = false;
		private bool entering = false;

		void Start()
		{
			// show default elements, hide selected ones
			ShowHideObjects (defaultElements, true);
			ShowHideObjects (selectedElements, false);

			// save original scale and rotation
			startScale = transform.localScale;
			selectedScale = startScale * clickScaleFactor;

			startElementScale = new Vector3[scaleElements.Length];
			startRotations = new Vector3[rotateElements.Length];

			// save rotation and scale of button elements
			if (rotateElements!=null) for(int i=0; i<rotateElements.Length; i++) if (startRotations[i]!=null) startRotations[i] = rotateElements[i].localRotation.eulerAngles;
			if (scaleElements!=null)  for(int i=0; i<scaleElements.Length; i++) if (startElementScale[i]!=null) startElementScale[i] = scaleElements[i].localScale;
		}

		public void OnPointerEnter(PointerEventData e)
		{
			// make sure we don't have multiple entering anmations
			if (entering || clickStarted) return;
			entering = true;
			print ("On Pointer Enter");

			SimpleSoundManager.Instance.PlaySound ("hover");
			ShowHideObjects (defaultElements, false);
			ShowHideObjects (selectedElements, true);

			// Scale elements
			for(int i=0; i<scaleElements.Length; i++) {
				if (scaleElements [i]!=null) {
					MiniTween.Tween.Scale (scaleElements [i], animationTime).From (scaleElements [i].localScale).To (scaleElements [i].localScale * scaleElementFactor).Tags("scale").Start ();
				}
			}

			// Rotate elements
			for(int i=0; i<rotateElements.Length; i++) {
				if (rotateElements[i]!=null) {
					Vector3 startRotation = rotateElements [i].localRotation.eulerAngles;
					Vector3 targetRotaton = rotateElements [i].localRotation.eulerAngles + rotateVector;
					MiniTween.Tween.Rotate(rotateElements[i], animationTime).From (startRotation).To (targetRotaton).Tags("rotate").Start ();
				}
			}
		}

		public void OnPointerExit(PointerEventData e)
		{
			if (!entering || clickStarted) return;
			entering = false;

			//print ("On Pointer Exit");

			ShowHideObjects (defaultElements, true);
			ShowHideObjects (selectedElements, false);
			clickStarted = false;

			// Scale back elements
			// MiniTween.Tween.Stop ();
			for(int i=0; i<scaleElements.Length; i++) {
				if (scaleElements [i]!=null) {
					MiniTween.Tween.Scale (scaleElements [i], animationTime).From (scaleElements [i].localScale).To (startElementScale[i]).Start ();
				}
			}

			// Rotate back elements
			for(int i=0; i<rotateElements.Length; i++) {
				if (rotateElements[i]!=null) {
					Vector3 startRotation = rotateElements [i].localRotation.eulerAngles;
					MiniTween.Tween.Rotate(rotateElements[i], animationTime).From (startRotation).To (startRotations[i]).Tags("rotate").Start ();
				}
			}
		}

		public void OnPointerClick(PointerEventData e)
		{
			if (clickStarted) {
				//print ("OnPointer Click");
				lastClickTime = Time.timeSinceLevelLoad;
				triggerEvent.Invoke ();
				SimpleSoundManager.Instance.PlaySound ("click");
				//MiniTween.Tween.Stop ();
				if (clickScaleFactor>0) MiniTween.Tween.Scale (transform, animationTime).From (transform.localScale).To (startScale).Start ();
				clickStarted = false;
			}
		}

		public void OnPointerDown(PointerEventData e)
		{
			if (lastClickTime==0 || Time.timeSinceLevelLoad - lastClickTime > 1.0f) {
				//print ("OnPointer Down");
				lastClickTime = Time.timeSinceLevelLoad;
				clickStarted = true;
				if (clickScaleFactor>0) MiniTween.Tween.Scale (transform, animationTime).From (startScale).To (selectedScale).Tags("down-anim").Start ();
			}		
		}

		void ShowHideObjects(GameObject[] array, bool show)
		{
			foreach(var obj in array) {
				obj.SetActive (show);
			}
		}

	}

}