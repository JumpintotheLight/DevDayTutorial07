using UnityEngine;
using System.Collections;

public class SpinObject : MonoBehaviour {

	void Update () {
        if (this.enabled) transform.Rotate(0, 0, -1f);
	}
}
