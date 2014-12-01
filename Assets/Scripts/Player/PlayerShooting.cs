using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
	public float damage = 11f;                  		// The damage per shot.
	public AudioClip shotClip;                          // An audio clip to play when a shot happens.
	public float flashIntensity = 3f;                   // The intensity of the light when the shot happens.
	public float fadeSpeed = 10f;                       // How fast the light will fade after the shot.
	
	private Animator anim;                              // Reference to the animator.
	private HashIDs hash;                               // Reference to the HashIDs script.
	private LineRenderer laserShotLine;                 // Reference to the laser shot line renderer.
	private Light laserShotLight;                       // Reference to the laser shot light.

	private GameObject enemy;                           // Reference to the enemy's gameObject
	private EnemyHealth enemyHealth;	                // Reference to the enemy's health.
	public GameObject gun;								// Reference to the player's gun
	private bool shooting;                              // A bool to say whether or not the player is currently shooting.
	Ray shootRay;                                  		// A ray from the gun end forwards.
	RaycastHit shootHit;                            	// A raycast hit to get information about what was hit.
	float timer;                                    	// A timer to determine when to fire.
	int shootableMask;                                  // A layer mask so the raycast only hits things on the shootable layer.
	public float timeBetweenBullets = 0.15f;        	// The time between each shot
	float effectsDisplayTime = 0.2f;                	// The proportion of the timeBetweenBullets that the effects will display for.
	public float range = 30f;                      		// The distance the gun can fire.


	void Awake ()
	{
		// Setting up the references.
		anim = GetComponent<Animator>();
		laserShotLine = GetComponentInChildren<LineRenderer>();
		laserShotLight = laserShotLine.gameObject.light;
		//col = GetComponent<SphereCollider>();
		//enemy = GameObject.FindGameObjectWithTag(Tags.enemy).transform;
		//enemyHealth = enemy.gameObject.GetComponent<EnemyHealth>();
		shootableMask = LayerMask.GetMask ("Shootable");
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		
		// The line renderer and light are off to start.
		laserShotLine.enabled = false;
		laserShotLight.intensity = 0f;
	}
	
	
	void Update ()
	{
		// Cache the current value of the shot curve.
		float shot = anim.GetFloat(hash.shotFloat);
		
		// Add the time since Update was last called to the timer.
		timer += Time.deltaTime;
		
		// If the Fire1 button is being press and it's time to fire
		if(Input.GetButton ("Fire1") && timer >= timeBetweenBullets)
		{
			// shoot the gun.
			Shoot();
		}
		
		// If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for
		if(timer >= timeBetweenBullets * effectsDisplayTime)
		{
			// the player is no longer shooting and disable the line renderer.
			shooting = false;
			// disable the effects.
			DisableEffects ();
		}

		// Fade the light out.
		laserShotLight.intensity = Mathf.Lerp(laserShotLight.intensity, 0f, fadeSpeed * Time.deltaTime);
	}
	
	public void DisableEffects ()
	{
		// Disable the line renderer and the light.
		laserShotLine.enabled = false;
		laserShotLight.enabled = false;
	}
	
	
	/*void OnAnimatorIK (int layerIndex)
	{
		// Cache the current value of the AimWeight curve.
		float aimWeight = anim.GetFloat(hash.aimWeightFloat);
		
		// Set the IK position of the right hand to the player's centre.
		anim.SetIKPosition(AvatarIKGoal.RightHand, enemy.position + Vector3.up);
		
		// Set the weight of the IK compared to animation to that of the curve.
		anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
	}*/
	
	
	void Shoot ()
	{
		//reset  the timer
		timer = 0f;

		// The enemy is shooting.
		shooting = true;

		//enable the light
		laserShotLight.enabled = true;

		// Enable the line renderer and set it's first position to be the end of the gun.
		laserShotLine.enabled = true;
		laserShotLine.SetPosition (0, gun.transform.position);

		Vector3 pos = new Vector3 (0.05f, 0.0f, 0.0f);
		// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
		shootRay.origin = gun.transform.position + pos;
		shootRay.direction = gun.transform.forward;
		
		// Perform the raycast against gameobjects on the shootable layer and if it hits something...
		if(Physics.Raycast (shootRay, out shootHit, range,shootableMask))
		{
			// Try and find an EnemyHealth script on the gameobject hit
			enemy = shootHit.transform.gameObject;
			//enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
			enemyHealth = enemy.GetComponent<EnemyHealth>();
			
			// If the EnemyHealth component exist
			if(enemyHealth != null)
			{
				// The enemy takes damage
				enemyHealth.TakeDamage(damage);

				// Set the second position of the line renderer to the point the raycast hit.
				laserShotLine.SetPosition (1, shootHit.point);

				// Make the light flash.
				laserShotLight.intensity = flashIntensity;
				
				// Play the gun shot clip at the position of the muzzle flare.
				AudioSource.PlayClipAtPoint(shotClip, laserShotLight.transform.position);
			}
			
		}
		// If the raycast didn't hit anything on the shootable layer
		else
		{
			// set the second position of the line renderer to the fullest extent of the gun's range
			laserShotLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
		}
		
	}

}