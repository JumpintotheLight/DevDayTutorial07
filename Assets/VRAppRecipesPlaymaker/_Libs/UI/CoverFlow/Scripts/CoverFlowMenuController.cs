using UnityEngine;
using System;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine.UI;
using ZefirVR;
using HutongGames.PlayMaker;

// Cover Flow menu
public class CoverFlowMenuController : Singleton<CoverFlowMenuController>
{
	public List<MenuItem>  menuItems;
	public GameObject menuItemPrefab;

	public PlayMakerFSM targetFsm;

	private float Time = 0.333f;
	private int Offset = 1;
	private bool Clamp = true;

	private string menuItemClickedEvent = "MenuItemClicked";
	private string menuSetupFinishedEvent = "MenuSetupFinished";

	[HideInInspector] public int currentItemIndex;
	[HideInInspector] public int totalMenuItems;

	private List<GameObject> menuObjects;
	private int _clamp;
	private int _tweenInertia;

	void Awake()
	{
		// setup event camera
		Canvas[] canvases = GetComponentsInChildren<Canvas> ();
		foreach(Canvas canvas in canvases) {
			canvas.worldCamera = Camera.main;
		}
	}

	void ReleaseOldItems()
	{
		if (menuObjects!=null) foreach(var go in menuObjects) SimplePool.Despawn (go);
		menuObjects = new List<GameObject>();
	}

	public void SetupFlowMenu()
	{
		ReleaseOldItems ();

		// Generate menu items
		for(int i=0; i<menuItems.Count; i++) {

			Vector3 to = new Vector3(i * Offset, 0.0f, Mathf.Abs(i) * Offset);
			var go = SimplePool.Spawn (menuItemPrefab, to, Quaternion.identity);
			go.transform.SetParent (transform);
			go.transform.localPosition = to;
			go.transform.localRotation = Quaternion.Euler (new Vector3 (0, 180, 0));
			menuObjects.Add (go);

			// setup link from canvases to main camera
			var canvases = menuObjects [i].GetComponentsInChildren <Canvas> ();
			foreach(var can in canvases) {
				can.worldCamera = Camera.main;
			}

			// Setup CoverFlow Menu item
			var itemController = go.GetComponentInChildren <MenuItemController> ();
			if (itemController!=null) {
				// setup menu item
				menuItems[i].index = i;
				itemController.OnItemClicked = MenuItemClicked;
				itemController.OnItemGazeSelected = MenuItemGazeSelected;
				itemController.Setup (menuItems[i]);
			}
		}

		_clamp = menuObjects.Count * Offset + 1;

		// Scroll flow menu to selected item
		Flow (currentItemIndex);
		PlayMakerFSM.BroadcastEvent (menuSetupFinishedEvent);
	}

	// called when a menu item was clicked
	public void MenuItemClicked(int index) 
	{
		if (index!=currentItemIndex) Flow(index);
		else {
			print ("menu item "+ index + " was clicked" );
			if (targetFsm!=null) targetFsm.SendEvent (menuItemClickedEvent);
			PlayMakerFSM.BroadcastEvent (menuItemClickedEvent);
		}
	}

	// called when a menu item was gaze selected
	public void MenuItemGazeSelected(int index)
	{
		// flow to the selected menu item
		if (index!=currentItemIndex) Flow(index);
	}


	public int GetClosestIndex()
	{
		int closestIndex = -1;
		float closestDistance = float.MaxValue;
		for (int i = 0; i < menuObjects.Count; i++)
		{
			float distance = (Vector3.zero - menuObjects[i].transform.localPosition).sqrMagnitude;
			if (distance < closestDistance)
			{
				closestIndex = i;
				closestDistance = distance;
			}
		}
		return closestIndex;
	}

	public void Flow()
	{
		Flow(GetClosestIndex());
	}

	private int GetIndex(GameObject view)
	{
		int found = -1;
		for (int i = 0; i < menuObjects.Count; i++)
		{
			if (view == menuObjects[i])
			{
				found = i;
			}
		}
		return found;
	}

	public void Flow(GameObject target)
	{
		int found = GetIndex(target);
		if (found != -1)
		{
			Flow(found);
		}
	}

	public void Flow(int target)
	{
		for (int i = 0; i < menuObjects.Count; i++)
		{
			int delta = (target - i) * -1;
			Vector3 to = new Vector3(delta * Offset, 0.0f, Mathf.Abs(delta) * Offset);
			LeanTween.moveLocal(menuObjects[i], to, Time).setEase(LeanTweenType.easeSpring);
		}
		currentItemIndex = target;
	}

	public void Flow(float offset)
	{
		for (int i = 0; i < menuObjects.Count; i++)
		{
			Vector3 p = menuObjects[i].transform.localPosition;
			float newX = p.x + offset;
			bool negative = newX < 0;
			Vector3 newP;
			if (Clamp)
			{
				float clampX = Mathf.Clamp(newX, ClampXMin(i, negative), ClampXMax(i, negative));
				float clampZ = Mathf.Clamp(Mathf.Abs(newX), 0.0f, ClampXMax(i, negative));
				newP = new Vector3(clampX, p.y, clampZ);
			}
			else
			{
				newP = new Vector3(newX, p.y, Mathf.Abs(newX));
			}
			menuObjects[i].transform.localPosition = newP;
		}
	}

	private float ClampXMin(int index, bool negative)
	{
		float newIndex = negative ? index : newIndex = menuObjects.Count - index - 1;
		return -(_clamp - (Offset * newIndex));
	}

	private float ClampXMax(int index, bool negative)
	{
		float newIndex = negative ? index : newIndex = menuObjects.Count - index - 1;
		return _clamp - (Offset * newIndex);
	}

	public void Inertia(float velocity)
	{
		_tweenInertia = LeanTween.value(gameObject, Flow, velocity, 0, 0.5f).setEase(LeanTweenType.easeInExpo).setOnComplete(Flow).id;
	}

	public void StopInertia()
	{
		LeanTween.cancel(gameObject, _tweenInertia);
	}

	public void Prev()
	{
		if (currentItemIndex > 0)
		{
			Flow(currentItemIndex - 1);
		}
	}

	public void Next()
	{
		if (currentItemIndex < menuObjects.Count - 1)
		{
			Flow(currentItemIndex + 1);
		}
	}
}
