using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

// Load and play audio file from streaming assets or a folder
public class TextFileLoader : MonoBehaviour
{
	public Action<string> OnTextFileLoaded;

	public GameObject loadingIndicator;
	OVRProgressIndicator progressIndicator;

	private void Awake() { 
		progressIndicator = FindObjectOfType<OVRProgressIndicator> ();
	}

	void ProgressIndicator(float progress)
	{
		//print ("loading text progress: " + progress);
		if (progressIndicator!=null) progressIndicator.currentProgress = progress;
	}

	public void LoadTextFile(string textFileUrl, Action<string> onLoaded = null)
	{
		print ("load text file: " + textFileUrl);
		ShowLoading (true);
		ProgressIndicator (0.01f);
		StartCoroutine (LoadText(textFileUrl, onLoaded));
	}

	WWW www;

	IEnumerator LoadText(string url, Action<string> onLoaded )
	{		
		string text = null;
		if (url.StartsWith ("jar:") || url.StartsWith ("http")) {
			//Debug.Log("Loading text with www : " + url);

			www = new WWW(url);
			www.threadPriority = ThreadPriority.Low;
			yield return www;
			StartCoroutine (UpdateProgress ());

			ShowLoading (false);

			if (www.error!=null) {
				Debug.Log ("error loading file " + url + " : " + www.error);
				text = null;
			} else {
				text = www.text;
			}
			www = null;

			//print ("loaded text: " + text);
		} else {
			if (url.StartsWith ("file://")) url = url.Substring (7);
			//Debug.Log("Reading text file : " + url);
			if (File.Exists (url)) {
				text = System.IO.File.ReadAllText (url);
				//print ("loaded text from file: " + text);
			}

		}

		ShowLoading (false);
		ProgressIndicator (0f);
		if (onLoaded!=null) OnTextFileLoaded = onLoaded;
		if (OnTextFileLoaded != null) OnTextFileLoaded (text);
	}

	IEnumerator UpdateProgress()
	{
		yield return null;
		while(www!=null && !www.isDone) {
			ProgressIndicator (www.progress);
			yield return null;
		}
	}
		
	void ShowLoading(bool show) {
		if (loadingIndicator!=null) loadingIndicator.SetActive (show);
	}
}
