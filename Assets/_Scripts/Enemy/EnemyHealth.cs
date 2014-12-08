using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
		public float health = 10f;                          // How much health the player has left.
		private Animator anim;                              // Reference to the animator component.
		private HashIDs hash;                               // Reference to the HashIDs.
		public float sinkSpeed = 1.5f;              		// The speed at which the enemy sinks through the floor when dead.

		private bool playerDead;                            // A bool to show if the player is dead or not.
		bool isSinking;                             		// Whether the enemy has started sinking through the floor.

		internal int enemyShareNum {set;get;}				//Sets the tag for the enemy to determing whether enemy should be shot or not
		public int scoreValue;								//Amount added or deleted to score when enemy dies

		void Awake ()
		{
			// Setting up the references.
			anim = GetComponent<Animator> ();
			//playerMovement = GetComponent<PlayerMovement>();
			hash = GameObject.FindGameObjectWithTag (Tags.gameController).GetComponent<HashIDs> ();

		}
	
		void Update ()
		{
			// If health is less than or equal to 0...
			if (health <= 0f) 
			{
				// ... and if the player is not yet dead...
				if (!playerDead)
				// ... call the PlayerDying function.
						PlayerDying ();
				else {
						// Otherwise, if the player is dead, call the PlayerDead and LevelReset functions.
						PlayerDead ();
				}
			}
			// If the enemy should be sinking...
			if (isSinking) 
			{
				if(transform.position.y > -2.0f)
				{	// ... move the enemy down by the sinkSpeed per second.
					transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
				}
				else
				{
					//make the enemy inactive and disappear from the scene
					//renderer.enabled = false;
					gameObject.SetActive(false);
					transform.Find ("Tag_share").renderer.enabled = false;
					isSinking = false;
				}
			}
		}
	
		void PlayerDying ()
		{
			// The player is now dead.
			playerDead = true;
			// Set the animator's dead parameter to true also
			anim.SetBool (hash.deadBool, playerDead);
		}
	
		void PlayerDead ()
		{
			// If the player is in the dying state then reset the dead parameter
			if (anim.GetCurrentAnimatorStateInfo (0).nameHash == hash.dyingState)
					anim.SetBool (hash.deadBool, false);
			// Disable the movement
			anim.SetFloat (hash.speedFloat, 0f);
			
			//set the enemyAI Alive flag to false
			gameObject.GetComponent<EnemyAI> ().KilledInAction ();
			
			// The enemy should now sink
			isSinking = true;
	}

		public void TakeDamage (float amount)
		{
			// Decrement the player's health by amount
			health -= amount * 2;
		}
	
		public void setEnemyShare(int val)
		{
			enemyShareNum = val;
			//Debug.Log (val+"\n");
		}

}