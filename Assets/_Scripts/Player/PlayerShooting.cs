using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
	Ray shootRay;                                  		// A ray from the gun end forwards.
	RaycastHit shootHit;                            	// A raycast hit to get information about what was hit.
	float timer;                                    	// A timer to determine when to fire.
	int shootableMask;                                  // A layer mask so the raycast only hits things on the shootable layer.
	public float timeBetweenBullets = 0.15f;        	// The time between each shot
	float effectsDisplayTime = 0.2f;                	// The proportion of the timeBetweenBullets that the effects will display for.
	public float range = 100f;                      	// The distance the gun can fire.

	public int startingScore = 50;                   		// Score the player starts the game with
	public int currentScore;                           	// The current score
	public Slider scoreSlider;                         	// Reference to the UI's score bar
	public Image damageImage;                           // Reference to an image to flash on the screen on hitting an enemy
	public float flashSpeed = 5f;                               // The speed the damageImage will fade at
	public Color correctKillColor = new Color(0f, 1f, 0f, 0.1f);     // The colour the damageImage is set to, to flash
	public Color incorrectKillColor = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash
	private int shotEnemy = 2;								//to keep track of which enemy the player shot
	public Text gameOverText;


	void Awake ()
	{
		// Setting up the references.
		anim = GetComponent<Animator>();
		laserShotLine = GetComponentInChildren<LineRenderer>();
		laserShotLight = laserShotLine.gameObject.light;
		shootableMask = LayerMask.GetMask ("Shootable");
		hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
		
		// The line renderer and light are off to start.
		laserShotLine.enabled = false;
		laserShotLight.intensity = 0f;

		//scoring bookkeeping
		currentScore = startingScore;
	}
	
	
	void Update ()
	{
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
			// disable the effects
			DisableEffects ();
		}

		// Fade the light out
		laserShotLight.intensity = Mathf.Lerp(laserShotLight.intensity, 0f, fadeSpeed * Time.deltaTime);

		// ... Update score and color of Damage Image based on which enemy shot
		if(shotEnemy == 0)
		{
			UpdateScore(correctKillColor,10);
		}
		else if(shotEnemy == 1)
		{
			UpdateScore(incorrectKillColor,-10);
		}
		// Otherwise...
		else
		{
			// ... transition the colour back to clear
			damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
		}
		
		// Reset the shotEnemy flag
		shotEnemy = 2;

		//check if player has won or lost based on score
		gameStatus ();
	}
	
	public void DisableEffects ()
	{
		// Disable the line renderer and the light.
		laserShotLine.enabled = false;
		laserShotLight.enabled = false;
	}
	

	void Shoot ()
	{
		//reset  the timer
		timer = 0f;

		//enable the light
		laserShotLight.enabled = true;

		// Enable the line renderer and set it's first position to be the end of the gun.
		laserShotLine.enabled = true;
		laserShotLine.SetPosition (0, gun.transform.position + new Vector3(0.1f,0.0f,0.0f));

		Vector3 pos = new Vector3 (0.05f, 0.0f, 0.0f);
		// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
		shootRay.origin = gun.transform.position + pos;
		shootRay.direction = gun.transform.forward;
		
		// Perform the raycast against gameobjects on the shootable layer and if it hits something...
		if(Physics.Raycast (shootRay, out shootHit, range,shootableMask))
		{
			// Try and find an EnemyHealth script on the gameobject hit
			enemy = shootHit.transform.gameObject;
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

				shotEnemy = enemyHealth.enemyShareNum;
			}
			
		}
		// If the raycast didn't hit anything on the shootable layer
		else
		{
			// set the second position of the line renderer to the fullest extent of the gun's range
			laserShotLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
		}
		
	}

	void UpdateScore(Color damageColor,int amount)
	{
		//set the damage color
		damageImage.color = damageColor;

		// change the current score by amount
		currentScore += amount;
		
		// Set the score bar's value to the current score
		scoreSlider.value = currentScore;
	}

	void gameStatus()
	{
		//if currentScore is > 150 the player has won
		if (currentScore >= scoreSlider.maxValue)
		{
			//set the game over text to Winning	
			gameOverText.text = "You Won!! \n Game Over";
		}
		//...else if score is less than 0 the player has lost
		else if (currentScore <=scoreSlider.minValue)
		{
			//set the game over text to losing
			gameOverText.text = "GAME OVER \n You Lost!!";
		}
	}

}