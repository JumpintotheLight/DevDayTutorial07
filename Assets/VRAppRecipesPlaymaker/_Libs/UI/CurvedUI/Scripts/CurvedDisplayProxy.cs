using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CurvedDisplayProxy : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

	public OVRRaycaster targetRaycaster;

	public OVRInputModule inputModule;
	public EventSystem eventSystem;

	private Canvas targetCanvas;

	private List<RaycastResult> m_RaycastResults = new List<RaycastResult>();

	void Awake()
	{
		targetCanvas = targetRaycaster.gameObject.GetComponent<Canvas> ();
	}

	public void OnPointerEnter(PointerEventData e)
	{
		if (e.IsVRPointer())
		{
			Ray incomingRay = e.GetRay ();
			Vector3 pos = e.pointerCurrentRaycast.worldPosition;
			Vector3 direction = transform.forward * -1;

			incomingRay.direction = direction;
			incomingRay.origin = pos;

			targetRaycaster.OnPointerEnter (e);

			print ("VR pointer entered: " + e.position + " " + e.GetRay ().direction);
		}
	}

	public void OnPointerExit(PointerEventData e)
	{
//		if (e.IsVRPointer())
//		{
//			Ray incomingRay = e.GetRay ();
//			Vector3 pos = e.pointerCurrentRaycast.worldPosition;
//			Vector3 direction = transform.forward;
//
//			incomingRay.direction = direction;
//			incomingRay.origin = pos;
//
//			targetRaycaster.Raycast (e, m_RaycastResults, incomingRay, false);
//
//			print ("VR pointer exit: " + e.position + " " + e.GetRay ().direction);
//		}
	}

	private Ray lastRay;

	public void OnPointerClick(PointerEventData e)
	{
		if (e.IsVRPointer())
		{
		
			Ray incomingRay = e.GetRay ();
			Vector3 pos = e.pointerCurrentRaycast.worldPosition;
			Vector3 direction = transform.forward;

			incomingRay.direction = direction;
			incomingRay.origin = pos;

			//targetRaycaster.Raycast (e, m_RaycastResults, incomingRay, false);
			//inputModule.SendSubmitEventToSelectedObject ();

			print ("VR pointer click: " + pos);
		}
	}
}
