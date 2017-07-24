using UnityEngine;
using System.Collections;

public class CrossFader : MonoBehaviour {
	[HideInInspector]
	public float FadeTime = 3f;

	private bool fading;
	private bool fadeInc;
	private float fadevalue = 0;
	private Material mat;

	// Use this for initialization
	void Start () {
		mat = GetComponent<MeshRenderer> ().material;
	}
	
	// Update is called once per frame
	void Update () {
		// Change blend param 
		if (fading) {
			if (fadeInc) {
				fadevalue += 1f/FadeTime * Time.deltaTime;
				if (fadevalue > 1f) {
					fadevalue = 1f;
					fading = false;
					Resources.UnloadUnusedAssets ();
				}
			} else {
				fadevalue -= 1f/FadeTime * Time.deltaTime;
				if (fadevalue < 0f) {
					fadevalue = 0f;
					fading = false;
					Resources.UnloadUnusedAssets ();
				}
			}
			mat.SetFloat ("_Blend",fadevalue);				
		}
	}

	public void BeginFade(bool inc) {
		fading=true;
		fadeInc = inc;
		if (fadeInc) {
			fadevalue = 0f;
		}
		else {
			fadevalue = 1f;			
		}
	}

}
