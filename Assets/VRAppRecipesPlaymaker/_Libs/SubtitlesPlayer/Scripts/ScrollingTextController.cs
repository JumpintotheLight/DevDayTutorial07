using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleListItem {
	public SubtitleItem subItem;
	public GameObject go;
	public SubtitleButtonItem buttonItem;
}

public class ScrollingTextController : MonoBehaviour {

	public GameObject subtitleItemPrefab;
	public Transform contentRoot;
	public Scrollbar scrollbar;
	public SubtitlesPlayer subPlayer;

	List<SubtitleListItem> subtitles;

	void Start()
	{
		subtitles = new List<SubtitleListItem> ();
	}

	public void Add (SubtitleItem subItem, int index) {
		// deselect all subtitles, except the active one, select active one
		for (int i = 0; i < subtitles.Count; i++) {
			if (subPlayer.loopOneLine && subPlayer.loopIndex == i) {
				subtitles[i].buttonItem.SetRepeating ();
			} else {
				if (i!=index) subtitles [i].buttonItem.DeSelect ();
				else subtitles[i].buttonItem.SetSelected ();
			}
		}

		if (index>=subtitles.Count) {
			// new subtitle, let's add it
			//print ("Add new subtitle to scroller: " + subItem.startTime);
			var listItem = new SubtitleListItem ();
			listItem.subItem = subItem;

			var go = SimplePool.Spawn (subtitleItemPrefab, Vector3.zero, Quaternion.identity);
			go.transform.SetParent (contentRoot);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			go.transform.localRotation = Quaternion.identity;
			listItem.go = go;

			var item = go.GetComponent<SubtitleButtonItem> ();
			item.Setup (subItem, index);
			listItem.buttonItem = item;

			subtitles.Add (listItem);
			StartCoroutine (ScrollToBottom ());
		}
	}

	IEnumerator ScrollToBottom()
	{
		yield return new WaitForSeconds (0.5f);
		scrollbar.value = 0f;
	}
	
}
