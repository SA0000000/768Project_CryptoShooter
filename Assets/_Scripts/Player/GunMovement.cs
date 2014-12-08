using UnityEngine;
using System.Collections;

public class GunMovement : MonoBehaviour {

	private Transform player; 				//get the player's transform
	private float sensitivity = 300.0f;		//for smoothness of movement of gun with mouse
	private Vector3 lastPos;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag (Tags.player).transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.forward = player.forward;
	}


	void OnMouseDown() {
		lastPos = Input.mousePosition;
	}
	
	void OnMouseDrag() {
		Vector3 pos = Input.mousePosition;
		transform.Rotate(0,(pos.y - lastPos.y)*sensitivity,0);
		lastPos = pos;
	}
}
