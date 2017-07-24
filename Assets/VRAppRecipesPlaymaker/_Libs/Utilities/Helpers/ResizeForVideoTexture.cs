using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using ZefirVR;

// Resize video screen when video is ready
public class ResizeForVideoTexture : MonoBehaviour {

	public SimpleVideoPlayer player;
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

	void Start()
	{
		if (player!=null) {
			player.videoPlayer.prepareCompleted += VideoReady;
		}
	}

	// resize texture when video is ready and we know it's dimensions
	void VideoReady(VideoPlayer source)
	{
		Resize (source.texture.width, source.texture.height);
	}

	public void Resize (int width, int height) 
	{
		Debug.Log ("Resize for : " + width + " / " + height);
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
