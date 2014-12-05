using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
	public float turnSmoothing = 20f;   // A smoothing value for turning the player.
	public float speedDampTime = 0.1f;  // The damping for the speed parameter
	public Vector3 rotationVector;
	GameObject MainCamera;

	private Animator anim;              // Reference to the animator component.
	private HashIDs hash;               // Reference to the HashIDs.

	int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
	float camRayLength = 100f;          // The length of the ray from the camera into the scene.

	void Awake ()
	{
		// Setting up the references.
		anim = GetComponent<Animator>();
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		
		// Set the weight of the shouting layer to 1.
		anim.SetLayerWeight(1, 1f);

		rotationVector = new Vector3 (0, 0, 0);
		MainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
	}
	
	
	void FixedUpdate ()
	{
		// Cache the inputs.
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
		//bool sneak = Input.GetButton("Sneak");
		MovementManagement(h, v);
		//MyMovementManagement (h, v);
	}
	
	
	void Update ()
	{
		// Cache the attention attracting input.
		bool shout = Input.GetButtonDown("Attract");
		
		// Set the animator shouting parameter.
		anim.SetBool(hash.shoutingBool, shout);

	}

	void MyMovementManagement (float horizontal, float vertical)
	{
		getInput (ref horizontal, ref vertical);

		// If there is some axis input...
		if(horizontal != 0f || vertical != 0f)
		{
			// ... set the players rotation and set the speed parameter to 5.5f.
			Rotating(horizontal, vertical);
			anim.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.deltaTime);
		}
		else
			// Otherwise set the speed parameter to 0.
			anim.SetFloat(hash.speedFloat, 0);
	}

	void getInput(ref float horizontal, ref float vertical){
		vertical = 0;
		horizontal = 0;
		if (Input.GetKey (KeyCode.UpArrow))
			vertical = vertical + 1;
		if (Input.GetKey (KeyCode.DownArrow))
			vertical = vertical -1;
		if (Input.GetKey (KeyCode.RightArrow))
			vertical = horizontal + 1;
		if (Input.GetKey (KeyCode.LeftArrow))
			vertical = horizontal -1;
		}
	
	void MovementManagement (float horizontal, float vertical)
	{

		// If there is some axis input...
		if(horizontal != 0f || vertical != 0f)
		{
			// ... set the players rotation and set the speed parameter to 5.5f.
			Rotating(horizontal, vertical);
			anim.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.deltaTime);
		}
		else
			// Otherwise set the speed parameter to 0.
			anim.SetFloat(hash.speedFloat, 0);
	}


	void MyRotating (float horizontal, float vertical)
	{
		if (vertical > 0.0)
			return;
		
		// Create a new vector of the horizontal and vertical inputs.
		Vector3 targetDirection = new Vector3 (horizontal, 0f, vertical);
		
		// Create a rotation based on this new vector assuming that up is the global y axis.
		Quaternion targetRotation = Quaternion.LookRotation (targetDirection, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Lerp (rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		rigidbody.MoveRotation (newRotation);
	}

	void Rotating (float horizontal, float vertical)
	{
		
				// Create a new vector of the horizontal and vertical inputs.
				Vector3 targetDirection = new Vector3 (horizontal, 0f, vertical);
		
				// Create a rotation based on this new vector assuming that up is the global y axis.
				Quaternion targetRotation = Quaternion.LookRotation (targetDirection, Vector3.up);
		
				// Create a rotation that is an increment closer to the target rotation from the player's rotation.
				Quaternion newRotation = Quaternion.Lerp (rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		
				// Change the players rotation to this new rotation.
				rigidbody.MoveRotation (newRotation);
		}




}