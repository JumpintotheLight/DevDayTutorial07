using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ZefirVR {

	public class SimpleSliderButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler  {

		public float timeToSelect = 3.0f;
		public UnityEvent triggerEvent;

		private Slider mySlider;
		private bool isGazing = false;
		private float gazingTime = 0;

		void Awake()
		{
			mySlider = GetComponent<Slider> ();
		}


		public void OnPointerEnter(PointerEventData e)
		{
			SimpleSoundManager.Instance.PlaySound ("hover");
			gazingTime = 0f;
			isGazing = true;
		}

		public void OnPointerExit(PointerEventData e)
		{
			//SimpleSoundManager.Instance.PlaySound ("hoverOver");
			gazingTime = 0;
			mySlider.value = 0f;
			isGazing = false;
		}

		public void OnPointerClick(PointerEventData e)
		{
		}

		void Update()
		{
			if (isGazing) {
				gazingTime += Time.deltaTime;
				if (gazingTime>=timeToSelect) {
					isGazing = false;
					gazingTime = 0;
					SimpleSoundManager.Instance.PlaySound ("click");
					triggerEvent.Invoke ();
				} else {
					mySlider.value = Mathf.Clamp01 (gazingTime / timeToSelect);
				}
			}
		}

	}

}