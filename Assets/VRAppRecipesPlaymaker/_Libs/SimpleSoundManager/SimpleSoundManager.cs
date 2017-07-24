using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZefirVR;
using System;

public class SimpleSoundManager : Singleton<SimpleSoundManager> {

	public static SimpleSoundManager Instance {
		get {
			return ((SimpleSoundManager)mInstance);
		} set {
			mInstance = value;
		}
	}

	public SimpleSoundManagerSettings settings;

	AudioSource source;

	void Awake()
	{
		source = gameObject.AddComponent<AudioSource> ();
		source.loop = false;
		source.playOnAwake = false;
	}

	public void PlaySound (string name) {
		for(int i=0;i<settings.sounds.Length;i++) {
			if (settings.sounds[i].name == name ) {
				source.Stop ();
				source.clip = settings.sounds [i].audio;
				source.volume = settings.sounds [i].volume;
				source.Play ();
			}
		}
	}
	
}
