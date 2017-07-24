using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class SetGearVRQualitySettings : MonoBehaviour {

	public int antiAliasing = 2;
	public float renderScale = 1.5f;

	void Start () {
		QualitySettings.antiAliasing = antiAliasing;
		VRSettings.renderScale = renderScale;
	}

}
