using UnityEngine;
using System.Collections;

public class LoadLevelWithFade : MonoBehaviour {
    public float delayBeforeLoad = 2.0f;
    public string sceneToLoad = string.Empty;
	private SceneFader sceneFader;
	public AudioSource clickSound;
   // public GameObject materialChanger;

	private bool isFading = false;
	
	void Awake()
	{
		sceneFader = FindObjectOfType(typeof(SceneFader)) as SceneFader;
	}

    public void LoadLevelNum()
	{
		if (isFading) return;
		isFading = true;
		if (clickSound!=null) clickSound.Play ();
		StartCoroutine(DelayedSceneLoad());
		if (sceneFader!=null) sceneFader.FadeOut();
    }

	IEnumerator DelayedSceneLoad()
	{
		// delay one frame to make sure everything has initialized
		yield return 0;
		
		// this is *ONLY* here for example as our 'main scene' will load too fast
		// remove this for production builds or set the time to 0.0f
		yield return new WaitForSeconds(delayBeforeLoad);
		
		Debug.Log( "[LoadLevel] " + sceneToLoad + " Time: " + Time.time );
		
		float startTime = Time.realtimeSinceStartup;
		#if !UNITY_PRO_LICENSE
		Application.LoadLevel(sceneToLoad);
		#else
		//*************************
		// load the scene asynchronously.
		// this will allow the player to 
		// continue looking around in your loading screen
		//*************************
		AsyncOperation async = Application.LoadLevelAsync(sceneToLoad);
		yield return async;
		#endif
		Debug.Log( "[SceneLoad] Completed: " + (Time.realtimeSinceStartup - startTime).ToString("F2") + " sec(s)");
	}	
}
