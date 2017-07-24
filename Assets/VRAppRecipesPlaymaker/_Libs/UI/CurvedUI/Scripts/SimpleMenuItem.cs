using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SimpleMenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, ISubmitHandler, IDeselectHandler {

	public void OnPointerEnter(PointerEventData e)
	{
		print ("menu item enter " + gameObject.name);
	}

	public void OnPointerExit(PointerEventData e)
	{
		print ("menu item exit " + gameObject.name);
	}

	public void OnPointerClick(PointerEventData e)
	{
		print ("menu item click " + gameObject.name);
	}

	public void OnPointerDown(PointerEventData e)
	{
		print ("menu item down " + gameObject.name);
	}

	public void OnPointerUp(PointerEventData e)
	{
		print ("menu item down " + gameObject.name);
	}
		
	public void OnSelect(BaseEventData e)
	{
		print ("menu item select " + gameObject.name);
	}

	public void OnSubmit(BaseEventData e)
	{
		print ("menu item submit " + gameObject.name);
	}

	public void OnDeselect(BaseEventData e)
	{
		print ("menu item deselect " + gameObject.name);
	}


}
