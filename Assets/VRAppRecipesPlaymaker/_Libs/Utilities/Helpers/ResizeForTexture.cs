using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeForTexture : MonoBehaviour {

	public bool stereoSBS = false;
	public bool stereoOU = false;

	Transform trans;
	float t_width, t_height, z;

	void Awake()
	{
		trans = this.transform;
		t_width = trans.localScale.x;
		t_height = trans.localScale.y;
		z = trans.localScale.z;
	}

	public void Resize (int width, int height) 
	{
		//Debug.Log ("Resize for : " + width + " / " + height);
		if (stereoSBS) width = width / 2;
		if (stereoOU) height = height / 2;
		if (width>height) {
			float y = t_width / ((float)width / (float)height);
			transform.localScale = new Vector3 (t_width, y, z);
			//Debug.Log ("resize: " + t_width + " / " + y);
		} else {
			float x = t_height * ((float)width / (float)height);
			transform.localScale = new Vector3 (x, t_height, z);
			//Debug.Log ("resize: " + x + " / " + t_height);
		}
	}
}
