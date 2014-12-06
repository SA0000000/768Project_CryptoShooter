﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
		public float turnSmoothing = 20f;   // A smoothing value for turning the player.
		public float speedDampTime = 0.1f;  // The damping for the speed parameter
		public float playerSpeed = 5.5f;
		private Animator anim;              // Reference to the animator component.
		private HashIDs hash;               // Reference to the HashIDs.

		void Awake ()
		{
				// Setting up the references.
				anim = GetComponent<Animator> ();
				hash = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<HashIDs> ();
		
				// Set the weight of the shouting layer to 1.
				anim.SetLayerWeight (1, 1f);
		}
	
		void FixedUpdate ()
		{
				float h = Input.GetAxis ("Horizontal");
				float v = Input.GetAxis ("Vertical");
				MovementManagement (h, v);
		}
	
		void Update ()
		{
				// Cache the attention attracting input.
				bool shout = Input.GetButtonDown ("Attract");
		
				// Set the animator shouting parameter.
				anim.SetBool (hash.shoutingBool, shout);

		}

		void MovementManagement (float horizontal, float vertical)
		{

				// If there is some axis input...
				if (horizontal != 0f || vertical != 0f) {
						// set the speed parameter to 5.5f.
						anim.SetFloat (hash.speedFloat, playerSpeed, speedDampTime, Time.deltaTime);
				} else
			// Otherwise set the speed parameter to 0.
						anim.SetFloat (hash.speedFloat, 0);
		}

		void Rotating (float horizontal, float vertical)
		{
				// Create a new vector of the horizontal and vertical inputs.
				Vector3 targetDirection = new Vector3 (horizontal, 0f, vertical);
		
				// Create a rotation based on this new vector assuming that up is the global y axis.
				Quaternion targetRotation = Quaternion.LookRotation (targetDirection, Vector3.up);
		
				// Create a rotation that is an increment closer to the target rotation from the player's rotation.
				Quaternion newRotation = Quaternion.Lerp (rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);

				rigidbody.MoveRotation (newRotation);
		}

		//Set "Identify" event for enemies in range
		void OnTriggerEnter (Collider other)
		{
				if (other.gameObject.CompareTag ("Enemy"))
						other.gameObject.GetComponent<EnemyAI> ().SetCollision ();

		}

		//ReSet "Identify" event for enemies no longer in range
		void OnTriggerExit (Collider other)
		{
				if (other.gameObject.CompareTag ("Enemy"))
						other.gameObject.GetComponent<EnemyAI> ().SetCollision ();
		}

}