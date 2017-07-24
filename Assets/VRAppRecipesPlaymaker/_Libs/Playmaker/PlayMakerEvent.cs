using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

public class PlayMakerEvent : MonoBehaviour {

	public PlayMakerFSM fsm;
	public string m_eventName;


	public void SendEvent(string eventName = null) {
		if (eventName!=null) m_eventName = eventName;
		if (fsm!=null) {
			fsm.SendEvent (eventName);
		} else {
			PlayMakerFSM.BroadcastEvent (eventName);
		}
	}
}
