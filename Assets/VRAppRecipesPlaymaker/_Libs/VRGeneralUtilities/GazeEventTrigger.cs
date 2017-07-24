using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GazeEventTrigger : Graphic, IPointerEnterHandler, IPointerExitHandler
{
    [Serializable]
    public class PointerEnterEvent : UnityEvent { }

    public PointerEnterEvent onEnter = new PointerEnterEvent();
    public PointerEnterEvent onExit = new PointerEnterEvent();

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        onEnter.Invoke();
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        onExit.Invoke();
    }
    
	public override bool Raycast(Vector2 sp, Camera eventCamera)
    {
        return true;
    }    
}
