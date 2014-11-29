using UnityEngine;
using System.Collections;

public class CreateEnemy : MonoBehaviour {

	public static int enemyCount = 5;	// to set number of enemies
	public Vector3 spawnValues;	// positions where the enemy can be spawned
	public GameObject enemy;	// enemy prefab to be spawned
	GameObject[] enemies = new GameObject[enemyCount];		// all the enemies that have been spawned
	public Transform[] wayPoints;	//get a reference to a wayPoint transform to instantiate enemy patrolling regions

	void Start () 
	{
	     for (int i=0; i < enemyCount; i++) {
			Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), 0.0f, spawnValues.z);
			Quaternion spawnRotation = Quaternion.identity;
			enemies[i] = (GameObject)Instantiate(enemy, spawnPosition, spawnRotation);
			//set patrolway points for each enemy
			enemies[i].GetComponent<EnemyAI>().setWayPoints(wayPoints);
			enemies[i].GetComponent<EnemyAI>().wayPointSetFlag = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (enemies[2].transform.position.x);
		//Debug.Log (enemies[4].transform.position.y);
		//Debug.Log (enemies[0].transform.position.z);
	}
}
