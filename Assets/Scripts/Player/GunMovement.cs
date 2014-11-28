using UnityEngine;
using System.Collections;

public class GunMovement : MonoBehaviour {

	private Transform player; 	//get the player's transform

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag (Tags.player).transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.forward = player.forward;
	}
}
