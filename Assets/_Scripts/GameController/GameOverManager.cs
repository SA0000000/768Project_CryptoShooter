using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
	public PlayerShooting playerShooting;       // Reference to the player's health
	public float restartDelay = 7f;         	// Time to wait before restarting the level
	
	
	Animator anim;                          // Reference to the animator component
	float restartTimer;                     // Timer to count up to restarting the level

	public Slider scoreSlider;
	
	void Awake ()
	{
		// Set up the reference
		anim = GetComponent <Animator> ();
		playerShooting = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<PlayerShooting> ();
	}
	
	
	void Update ()
	{
		// If the player has reached min or max score
		if(playerShooting.currentScore <= scoreSlider.minValue || playerShooting.currentScore >= scoreSlider.maxValue)
		{
			//yield WaitForSeconds(4);
			// ... tell the animator the game is over
			anim.SetTrigger ("GameOver");
			
			// .. increment a timer to count up to restarting
			restartTimer += Time.deltaTime;
			
			// .. if it reaches the restart delay...
			if(restartTimer >= restartDelay)
			{
				// .. then reload the currently loaded level
				//Application.LoadLevel(Application.loadedLevel);
			}
		}
	}
}