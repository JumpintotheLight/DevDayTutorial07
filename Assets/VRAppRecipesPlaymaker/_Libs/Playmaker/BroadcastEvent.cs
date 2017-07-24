using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;

public class BroadcastEvent : MonoBehaviour {

	public PlayMakerFSM targetFSM = null;

	public void BroadcastEventNow (string eventName) {
		if (targetFSM==null) PlayMakerFSM.BroadcastEvent (eventName);
		else targetFSM.SendEvent (eventName);
	}
}
