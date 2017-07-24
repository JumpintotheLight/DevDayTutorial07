using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using UnityEngine.UI;
using ZefirVR;

public class MenuItemController : MonoBehaviour {
	public MenuItem menuItem;
	public Image coverImage;
	public Text titleText;

	[HideInInspector] public Action<int> OnItemClicked;
	[HideInInspector] public Action<int> OnItemGazeSelected;

	private Downloader downloader;

	void Awake()
	{
		downloader = GetComponent<Downloader> ();
	}

	public void Setup(MenuItem item)
	{
		print ("setup menu item: " + item.index);

		menuItem = item;
		titleText.text = item.title;
		if (menuItem.cover!=null) {
			coverImage.sprite = Sprite.Create (menuItem.cover, new Rect(0,0,menuItem.cover.width, menuItem.cover.height), new Vector2(0,0));
		} else if (!string.IsNullOrEmpty (menuItem.coverImageUrl)) {
			downloader.DownloadAndShowImage (menuItem.coverImageUrl, coverImage);
		}
	}

	// Menu Item clicked / selected
	public void onClick () {
		print ("MenuItem: " + menuItem.index + " OnClick");
		if (OnItemClicked!=null) OnItemClicked(menuItem.index);
	}

	// Menu Item gaze selected - moving it forward
	public void onSelected()
	{
		print ("move to item: " + menuItem.index);
		if (OnItemGazeSelected!=null) OnItemGazeSelected(menuItem.index);
	}
}
