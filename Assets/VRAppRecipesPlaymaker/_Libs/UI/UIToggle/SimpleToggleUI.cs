using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleToggleUI : MonoBehaviour {

	public Image background;
	public Image handleOn;
	public Image handleOff;

	public Sprite backgroundOn;
	public Sprite backgroundOff;

	private Toggle myToggle;
	private float animationTime = 0.3f;
	private Vector2 offPos, onPos;

	void Awake()
	{
		myToggle = gameObject.GetComponent<Toggle> ();
		myRect = handleOn.GetComponent<RectTransform> ();
	}

	void Start () {
		offPos = handleOff.GetComponent<RectTransform> ().anchoredPosition;
		onPos = handleOn.GetComponent<RectTransform> ().anchoredPosition;
		UpdateToggle ();
	}

	private RectTransform myRect;

	public void ChangeState()
	{
		if (myToggle.isOn) {
			print ("on....");
			Vector2 pos = offPos;
			ZefirVR.Tween.Value (animationTime).From (offPos.x).To (onPos.x).OnUpdate ((v)=>{
				pos.x = v;
				myRect.anchoredPosition = pos;
			}).Ease (ZefirVR.Tween.EaseType.easeOutExpo).Start ().OnComplete (()=>{
				UpdateToggle ();
			});
		} else {
			print ("off....");
			Vector2 pos = onPos;
			ZefirVR.Tween.Value (animationTime).From (onPos.x).To (offPos.x).OnUpdate ((v)=>{
				pos.x = v;
				myRect.anchoredPosition = pos;
			}).Ease (ZefirVR.Tween.EaseType.easeOutExpo).Start ().OnComplete (()=>{
				UpdateToggle ();
			});			
		}
	}
	
	void UpdateToggle () {
		if (myToggle.isOn) {
			background.sprite = backgroundOn;
		} else {
			background.sprite = backgroundOff;
		}
		//handleOn.gameObject.SetActive (myToggle.isOn);
		//handleOff.gameObject.SetActive (!myToggle.isOn);
	}
}
