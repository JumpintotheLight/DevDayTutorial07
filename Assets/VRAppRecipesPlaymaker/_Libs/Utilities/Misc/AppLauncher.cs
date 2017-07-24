using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AppLauncher : MonoBehaviour {

	public void LaunchApp (string exeFilePath) {
		print ("LaunchApp: " + exeFilePath);
		System.Diagnostics.Process.Start(exeFilePath);
	}

	public void KillAppProcess (string processName) {
		print ("KillAppProcess: " + processName);
		foreach (var p in System.Diagnostics.Process.GetProcessesByName(processName)) {
			Debug.Log ("kill process:  " + p.ProcessName);
			p.CloseMainWindow();
			//p.Kill ();
		}
	}

}
