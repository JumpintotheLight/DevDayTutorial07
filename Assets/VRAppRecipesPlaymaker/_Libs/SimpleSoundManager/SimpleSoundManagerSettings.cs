using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZefirVR {
	
	[Serializable]
	public class AppSound {
		public string name;
		public AudioClip audio;
		public float volume = 1.0f;
	}


	[CreateAssetMenu(fileName = "SimpleSoundManagerSettings", menuName = "SimpleSoundManagerSettings", order = 2)]
	public class SimpleSoundManagerSettings : ScriptableObject {

		public AppSound[] sounds;


	}
}

