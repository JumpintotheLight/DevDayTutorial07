using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace ZefirVR {

	public class UIPlaySounds : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

		public string hoverSoundName = "hover";
		public string clickCoundName = "click";

		public void OnPointerEnter(PointerEventData e)
		{
			SimpleSoundManager.Instance.PlaySound (hoverSoundName);
		}

		public void OnPointerExit(PointerEventData e)
		{
			SimpleSoundManager.Instance.PlaySound ("hoverOver");
		}

		public void OnPointerClick(PointerEventData e)
		{
			SimpleSoundManager.Instance.PlaySound (clickCoundName);
		}
	
	}

}