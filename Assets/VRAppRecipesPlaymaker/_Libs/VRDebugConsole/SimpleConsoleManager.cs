using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Simple Console that shows log messages in a scrooll view
public class SimpleConsoleManager : MonoBehaviour {

	public GameObject messagePrefab;
	public Transform root;

	private ContentSizeFitter fitter;
	private ScrollRect scrollRect;
	private VerticalLayoutGroup vGroup;
	private Scrollbar scrollbar;

	void Awake()
	{
		fitter = GetComponentInChildren <ContentSizeFitter> ();
		scrollRect = GetComponentInChildren <ScrollRect> ();
		vGroup = GetComponentInChildren <VerticalLayoutGroup> ();
		scrollbar = GetComponentInChildren<Scrollbar> ();
	}

	public void AddNewMessage (string message) {
		GameObject obj = Instantiate (messagePrefab, root);
		obj.transform.localScale = Vector3.one;
		obj.transform.localRotation = Quaternion.identity;

		Text messageText = obj.GetComponent<Text> ();
		messageText.text = message;

		StartCoroutine (ScrollToBottom ());
	}

	IEnumerator ScrollToBottom()
	{
		yield return new WaitForSeconds (0.5f);
		scrollbar.value = 0f;
	}
}
