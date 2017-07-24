using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonLoadLevelWithFade : MonoBehaviour {

	public string loadLevelName;
	public float fadeTime = 1.0f;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			print ("back button");
			SceneFader.Instance.FadeOut ();
			StartCoroutine (waitAndLoadLevel ());
		}
	}

	IEnumerator waitAndLoadLevel()
	{
		yield return new WaitForSeconds (fadeTime);
		SceneManager.LoadScene (loadLevelName);
	}
}