using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("VR")]
	[Tooltip("Fading In Canvas Group")]
	public class FadeCanvasIn : FsmStateAction
	{
		[RequiredField]
		[Tooltip("CanvasGroup")]
		public CanvasGroup canvasGroup;
		
		[RequiredField]
		[HasFloatSlider(0,10)]
		[Tooltip("Fade in time in seconds.")]
		public FsmFloat time;

		[Tooltip("Event to send when finished.")]
		public FsmEvent finishEvent;

		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		public override void Reset()
		{
			time = 1.0f;
			finishEvent = null;
		}

		private float startTime;
		private float currentTime;
		private float alphaValue;

		public override void OnEnter()
		{
			startTime = FsmTime.RealtimeSinceStartup;
			currentTime = 0f;
			alphaValue = 0f;
		}

		public override void OnUpdate()
		{
			if (realTime)
			{
				currentTime = FsmTime.RealtimeSinceStartup - startTime;
			}
			else
			{
				currentTime += Time.deltaTime;
			}

			alphaValue = Mathf.Lerp (0f, 1.0f, currentTime / time.Value);
			canvasGroup.alpha = alphaValue;

			if (currentTime > time.Value)
			{
				if (finishEvent != null)
				{
					Fsm.Event(finishEvent);
				}

				Finish();
			}
		}
	}
}
