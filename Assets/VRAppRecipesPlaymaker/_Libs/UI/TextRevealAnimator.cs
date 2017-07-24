using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ZefirVR {

	public class TextRevealAnimator : MonoBehaviour {

		public bool animateOnStart = true;
		public float animationTime = 0.5f;
		public bool fade = true;
		public REVEAL_TYPE animationType;

		private Text myText;

		public enum REVEAL_TYPE {
			Up,
			Down,
			Left,
			Right
		}

		void Awake()
		{
			myText = GetComponentInChildren<Text> ();
			if (myText==null) Debug.LogWarning("Text component not found");
		}

		void OnEnable () {
			if (animateOnStart) RevealAnimation ();
		}

		void RevealAnimation()
		{
			if (fade) {
				Color myColor = myText.color;
				myColor.a = 0f;
				MiniTween.Tween.Value (animationTime).From (0).To (1.0f).OnUpdate ((value) => {
					myColor.a = value;
					myText.color = myColor;
				}).Start ();
			}

			Vector2 originalPosition, startPosition;

			switch(animationType) {
			case REVEAL_TYPE.Up:
				originalPosition = myText.rectTransform.anchoredPosition;
				startPosition = originalPosition;
				startPosition.y -= myText.rectTransform.rect.height;
				MiniTween.Tween.Value (animationTime).From (startPosition.y).To (originalPosition.y).OnUpdate ( (v) => {
					startPosition.y = v;
					myText.rectTransform.anchoredPosition = startPosition;
				}).Start ();
				break;
			case REVEAL_TYPE.Down:
				originalPosition = myText.rectTransform.anchoredPosition;
				startPosition = originalPosition;
				startPosition.y += myText.rectTransform.rect.height;
				MiniTween.Tween.Value (animationTime).From (startPosition.y).To (originalPosition.y).OnUpdate ( (v) => {
					startPosition.y = v;
					myText.rectTransform.anchoredPosition = startPosition;
				}).Start ();
				break;
			case REVEAL_TYPE.Right:
				originalPosition = myText.rectTransform.anchoredPosition;
				startPosition = originalPosition;
				startPosition.x -= myText.rectTransform.rect.width;
				MiniTween.Tween.Value (animationTime).From (startPosition.x).To (originalPosition.x).OnUpdate ( (v) => {
					startPosition.x = v;
					myText.rectTransform.anchoredPosition = startPosition;
				}).Start ();
				break;
			case REVEAL_TYPE.Left:
				originalPosition = myText.rectTransform.anchoredPosition;
				startPosition = originalPosition;
				startPosition.x += myText.rectTransform.rect.width;
				MiniTween.Tween.Value (animationTime).From (startPosition.x).To (originalPosition.x).OnUpdate ( (v) => {
					startPosition.x = v;
					myText.rectTransform.anchoredPosition = startPosition;
				}).Start ();
				break;
			}
		}

	}

}