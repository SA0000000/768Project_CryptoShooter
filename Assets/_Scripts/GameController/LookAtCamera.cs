using UnityEngine;
using System.Collections;

//Secondary camera for whole scene view

public class LookAtCamera : MonoBehaviour {
	public GameObject target;
	
	void LateUpdate() {
		transform.LookAt(target.transform);
	}
}