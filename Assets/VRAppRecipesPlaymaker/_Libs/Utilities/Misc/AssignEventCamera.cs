using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class AssignEventCamera : MonoBehaviour {

	void Awake () {
		Canvas myCanvas = GetComponent<Canvas> ();
		if (myCanvas != null)
			myCanvas.worldCamera = Camera.main;
	}
	
}
