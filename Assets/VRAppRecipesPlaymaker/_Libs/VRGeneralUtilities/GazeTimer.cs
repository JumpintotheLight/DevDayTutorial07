using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

public class GazeTimer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public PlayMakerFSM eventTarget;

	public string onFinishedEvent = "UGUI / ON GAZE FINISHED";

	public float timeToGaze = 3.0f;

	bool isGazing = false;
	float gazingTime = 0f;

	void Update () {
		if (isGazing) {
			gazingTime += Time.deltaTime;
			if (gazingTime >= timeToGaze) {
				print ("gaze timer finished");
				isGazing = false;
				eventTarget.SendEvent (onFinishedEvent);
				//onFinishedEvent.SendEvent(PlayMakerUGuiSceneProxy.fsm,eventTarget);
			}
		}
	}

	public void OnPointerEnter (PointerEventData data) {
		//print ("OnPointerEnter");
		isGazing = true;
		gazingTime = 0.0f;
		//GetLastPointerDataInfo.lastPointeEventData = data;
	}

	public void OnPointerExit (PointerEventData data) {
		//print ("OnPointerExit");
		isGazing = false;
		//GetLastPointerDataInfo.lastPointeEventData = data;
	}
}
