using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

[ActionCategory("VR")]
[Tooltip("Fading Out complete scene in 360")]
public class SceneFadeOut : FsmStateAction
{
	[RequiredField]
	[Tooltip("Fader material")]
	public FsmMaterial faderMaterial;
	
	[RequiredField]
	[Tooltip("Color to fade from. E.g., Fade up from black.")]
	public FsmColor color;

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
		color = Color.black;
		time = 1.0f;
		finishEvent = null;
	}

	private float startTime;
	private float currentTime;
	private Color colorLerp;

	public override void OnEnter()
	{
		startTime = FsmTime.RealtimeSinceStartup;
		currentTime = 0f;
		colorLerp = color.Value;
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

		colorLerp = Color.Lerp(Color.clear, color.Value, currentTime/time.Value);
		faderMaterial.Value.color = colorLerp;

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
