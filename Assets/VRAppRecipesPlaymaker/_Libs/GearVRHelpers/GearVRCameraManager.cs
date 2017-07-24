using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GearVRCameraManager : MonoBehaviour {

	public bool enableGazeClick = false;

	void Start () {
		OVRInputModule inputModule = GetComponentInChildren<OVRInputModule> ();
		if (inputModule==null) Debug.Log("OVRInputModule not found");

		GazeInteractionInputModule gazeInputModule = GetComponentInChildren<GazeInteractionInputModule> ();
		if (gazeInputModule==null) Debug.Log("GazeInteractionInputModule not found");

		inputModule.enabled = !enableGazeClick;
		gazeInputModule.enabled = enableGazeClick;
	}
	
}
