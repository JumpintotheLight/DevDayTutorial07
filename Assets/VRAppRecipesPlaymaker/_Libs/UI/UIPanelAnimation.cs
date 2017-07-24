using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZefirVR;
using System;
using HutongGames.PlayMaker;

public class UIPanelAnimation : MonoBehaviour {

	public float animationTime = 0.3f;
	public PlayMakerFSM fsm;
	public bool fade = false;

	private RectTransform myRect;
	private CanvasGroup canvasGroup;
	private Vector2 originalPos;
	private float originalAlpha;

	void Awake () {
		myRect = gameObject.GetComponent<RectTransform> ();
		canvasGroup = gameObject.GetComponent<CanvasGroup> ();
		originalPos = myRect.anchoredPosition;
		if (canvasGroup != null) originalAlpha = canvasGroup.alpha;
	}

	public void Reset()
	{
		myRect.anchoredPosition = originalPos;
		if (canvasGroup != null) canvasGroup.alpha = originalAlpha;
	}

	public void HideToLeftPM(string eventName) {
		HideToLeft (()=>{
			SendEvent (eventName);
		});
	}

	// Hide rect transform to the left
	public void HideToLeft (Action OnComplete) {
		Vector2 pos = myRect.anchoredPosition;
		Vector2 originalPos = pos;

		Tween.Value (animationTime).From (pos.x).To(pos.x - myRect.rect.width).OnUpdate ((v)=>{
			pos.x = v;
		}).Ease (Tween.EaseType.easeOutExpo).Start().OnComplete (()=>{
			if (OnComplete!=null) OnComplete();
		});

		if (fade) {
			Tween.Value (animationTime).From (1.0f).To(0).OnUpdate ((v)=>{
				canvasGroup.alpha = v;
			}).Ease (Tween.EaseType.easeOutExpo).Start().OnComplete (()=>{
				myRect.anchoredPosition = originalPos;
			});
		}
	}

	public void ShowFromLeftPM(string eventName) {
		ShowFromLeft (()=>{
			SendEvent (eventName);
		});
	}

	// Hide rect transform to the left
	public void ShowFromLeft(Action OnComplete) {
		Vector2 pos = myRect.anchoredPosition;

		Tween.Value (animationTime).From (pos.x - myRect.rect.width).To(pos.x).OnUpdate ((v)=>{
			pos.x = v;
			myRect.anchoredPosition = pos;
		}).Ease (Tween.EaseType.easeOutExpo).Start().OnComplete (()=>{
			if (OnComplete!=null) OnComplete();
		});

		if (fade) {
			Tween.Value (animationTime).From (0f).To(1.0f).OnUpdate ((v)=>{
				canvasGroup.alpha = v;
			}).Ease (Tween.EaseType.easeOutExpo).Start();
		}
	}

	// Show rect transform from right
	public void ShowFromRightPM(string eventName) {
		ShowFromRight (()=>{
			if (!string.IsNullOrEmpty (eventName)) {
				SendEvent (eventName);
			}
		});
	}

	public void ShowFromRight (Action OnComplete) {
		Vector2 pos = myRect.anchoredPosition;
		Tween.Value (animationTime).From (pos.x + myRect.rect.width).To(pos.x).OnUpdate ((v)=>{
			pos.x = v;
			myRect.anchoredPosition = pos;
		}).Ease (Tween.EaseType.easeOutExpo).Start().OnComplete (()=>{
			if (OnComplete!=null) OnComplete();
		});

		if (fade) {
			Tween.Value (animationTime).From (0f).To(1.0f).OnUpdate ((v)=>{
				canvasGroup.alpha = v;
			}).Ease (Tween.EaseType.easeOutExpo).Start();
		}
	}

	// Show rect transform from right
	public void HideToRightPM(string eventName) {
		HideToRight (()=>{
			if (!string.IsNullOrEmpty (eventName)) {
				SendEvent (eventName);
			}
		});
	}

	public void HideToRight(Action OnComplete) {
		Vector2 pos = myRect.anchoredPosition;
		Vector2 originalPos = pos;

		Tween.Value (animationTime).From (pos.x).To(pos.x  + myRect.rect.width).OnUpdate ((v)=>{
			pos.x = v;
			myRect.anchoredPosition = pos;
		}).Ease (Tween.EaseType.easeOutExpo).Start().OnComplete (()=>{
			if (OnComplete!=null) OnComplete();
		});

		if (fade) {
			Tween.Value (animationTime).From (1.0f).To(0f).OnUpdate ((v)=>{
				canvasGroup.alpha = v;
			}).Ease (Tween.EaseType.easeOutExpo).Start().OnComplete (()=>{
				myRect.anchoredPosition = originalPos;
			});
		}
	}

	// Show rect transform from right
	public void ShowFromTopPM(string eventName) {
		ShowFromTop (()=>{
			if (!string.IsNullOrEmpty (eventName)) {
				SendEvent (eventName);
			}
		});
	}

	public void ShowFromTop (Action OnComplete) {
		Vector2 pos = myRect.anchoredPosition;
		Tween.Value (animationTime).From (pos.y).To(pos.y - myRect.rect.height).OnUpdate ((v)=>{
			pos.y = v;
			myRect.anchoredPosition = pos;
		}).Ease (Tween.EaseType.easeOutExpo).Start().OnComplete (()=>{
			if (OnComplete!=null) OnComplete();
		});

		if (fade) {
			Tween.Value (animationTime).From (0f).To(1.0f).OnUpdate ((v)=>{
				canvasGroup.alpha = v;
			}).Ease (Tween.EaseType.easeOutExpo).Start();
		}
	}

	// Show rect transform from right
	public void HideToTopPM(string eventName) {
		HideToTop (()=>{
			if (!string.IsNullOrEmpty (eventName)) {
				SendEvent (eventName);
			}
		});
	}

	// Hide rect transform to the left
	public void HideToTop (Action OnComplete) {
		Vector2 pos = myRect.anchoredPosition;
		Vector2 originalPost = pos;

		Tween.Value (animationTime).From (pos.y).To(pos.y + myRect.rect.height).OnUpdate ((v)=>{
			pos.y = v;
			myRect.anchoredPosition = pos;
		}).Ease (Tween.EaseType.easeOutExpo).Start().OnComplete (()=>{
			if (OnComplete!=null) OnComplete();
		});

		if (fade) {
			Tween.Value (animationTime).From (1.0f).To(0).OnUpdate ((v)=>{
				canvasGroup.alpha = v;
			}).Ease (Tween.EaseType.easeOutExpo).Start().OnComplete (()=>{
				myRect.anchoredPosition = originalPos;
			});
		}
	}

	void SendEvent(string eventName)
	{
		if (!string.IsNullOrEmpty (eventName)) {
			if (fsm!=null) fsm.SendEvent (eventName);
			else PlayMakerFSM.BroadcastEvent (eventName);
		}
	}

}
