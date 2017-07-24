using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HutongGames.PlayMaker;

public class SubtitleButtonItem : MonoBehaviour {

	public Text subtitleText;
	public Text numText;
	public SubtitleItem subItem;

	public Color selectedColor = Color.green;
	public Color normalColor = Color.white;
	public Color repeatingColor = Color.red;

	public string text;
	int index;

	public void Setup (SubtitleItem sItem, int sIndex) {
		subItem = sItem;
		index = sIndex;

		string currentSubText = "";
		for (int i = 0; i < subItem.Lines.Count; i++) {
			currentSubText += subItem.Lines [i];
			if (i + 1 < subItem.Lines.Count) {
				currentSubText += "\r\n";
			}
		}
		text = currentSubText;
		subtitleText.text = text;
		if (numText!=null) numText.text = index.ToString () + ". " + subItem.startTime.ToString ("0:00");

		//print ("setup subtitle: " + text);
		var rect = GetComponent <RectTransform>();
		rect.sizeDelta = new Vector2 (rect.rect.width, subtitleText.preferredHeight);
		rect.anchoredPosition3D = new Vector3 (0, 15.0f + index * rect.rect.height, 0);
		rect.SetAsLastSibling ();
		SetSelected ();
	}	

	public void OnClick()
	{
		var videoSeek = PlayMakerGlobals.Instance.Variables.GetFsmFloat ("videoSeekValue");
		videoSeek.Value = subItem.startTime;
		print ("go back to subtitle: " + index + " at: " + subItem.startTime);
		PlayMakerFSM.BroadcastEvent ("SeekTo");
	}

	public void DeSelect()
	{
		subtitleText.color = normalColor;
	}

	public void SetSelected()
	{
		subtitleText.color = selectedColor;
	}

	public void SetRepeating()
	{
		subtitleText.color = repeatingColor;
	}

}
