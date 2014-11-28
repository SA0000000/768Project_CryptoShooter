using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
	public float chaseSpeed = 5f;                           // The nav mesh agent's speed when chasing.
	public float chaseWaitTime = 5f;                        // The amount of time to wait when the last sighting is reached.
	public float patrolSpeed = 2f;                          // The nav mesh agent's speed when patrolling.
	public float patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
	public Transform[] patrolWayPoints;                     // An array of transforms for the patrol route.

	public Transform[] defaultWayPoint;							//default patrolling route
	
	private EnemySight enemySight;                          // Reference to the EnemySight script.
	private NavMeshAgent nav;                               // Reference to the nav mesh agent.
	private Transform player;                               // Reference to the player's transform.
	//private PlayerHealth playerHealth;                      // Reference to the PlayerHealth script.
	private LastPlayerSighting lastPlayerSighting;          // Reference to the last global sighting of the player.
	private float chaseTimer;                               // A timer for the chaseWaitTime.
	private float patrolTimer;                              // A timer for the patrolWaitTime.
	private int wayPointIndex;                              // A counter for the way point array.

	public bool wayPointSetFlag=false;							//to check if way points have been assigned or not
	
	void Awake ()
	{
		// Setting up the references.
		enemySight = GetComponent<EnemySight>();
		nav = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		//playerHealth = player.GetComponent<PlayerHealth>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();

		//set default way point
		GameObject[] go = GameObject.FindGameObjectsWithTag("WayPoint");
		defaultWayPoint = new Transform[go.Length];
		for(int i =0; i< go.Length;i++)
			defaultWayPoint[i] = go[i].transform;
	}
	
	
	void Update ()
	{
		// If the player is in sight and is alive...
		if(enemySight.playerInSight )//&& playerHealth.health > 0f)
			// ... stop.
			Stopping();

		// If the player has been sighted and isn't dead...
		else if(enemySight.personalLastSighting != lastPlayerSighting.resetPosition )//&& playerHealth.health > 0f)
			// ... chase.
			Chasing();
		// Otherwise...
		else
			// ... patrol.
			Patrolling();
	}

	//Get and set enemy position
	public Vector3 Position
	{
		get
		{
			return player.transform.position;
		}

		set
		{
			player.transform.position = (Vector3)value;
		}
	}

	//get and set enemy velocity
	public Vector3 Velocity
	{
		get
		{
			return player.rigidbody.velocity;
		}
		set
		{
			player.rigidbody.velocity = value;
		}
	}
	
	
	void Stopping()
	{
		// Stop the enemy where it is.
		nav.Stop();
	}

	void Chasing ()
	{
		// Create a vector from the enemy to the last sighting of the player.
		Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;
		
		// If the the last personal sighting of the player is not close...
		if(sightingDeltaPos.sqrMagnitude > 4f)
			
			/***********ADD THE GLOBAL PLANNER's Goal towards which the enemy should be moving****/
			// ... set the destination for the NavMeshAgent to the last personal sighting of the player.
			nav.destination = enemySight.personalLastSighting;
		
		// Set the appropriate speed for the NavMeshAgent.
		nav.speed = chaseSpeed;
		
		// If near the last personal sighting...
		if(nav.remainingDistance < nav.stoppingDistance)
		{
			// ... increment the timer.
			chaseTimer += Time.deltaTime;
			
			// If the timer exceeds the wait time...
			if(chaseTimer >= chaseWaitTime)
			{
				// ... reset last global sighting, the last personal sighting and the timer.
				lastPlayerSighting.position = lastPlayerSighting.resetPosition;
				enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
				chaseTimer = 0f;
			}
		}
		else
			// If not near the last sighting personal sighting of the player, reset the timer.
			chaseTimer = 0f;
	}

	public void setWayPoints(Transform[] wayPoints)
	{
		//select a random number of points 
		int wayPointsIndex = Random.Range (3,wayPoints.Length-1);
		patrolWayPoints = new Transform[wayPointsIndex];
		//select random positions for each of the points
		for (int i = 0; i< wayPointsIndex; i++) 
		{
			patrolWayPoints[i] = wayPoints[i];	
		}
		//assign them to the patrolWayPoints for the 
	}
	
	
	void Patrolling ()
	{
		//if wayPoints have not been set them to the default wayPoints
		if(!wayPointSetFlag)
			setWayPoints (defaultWayPoint);

		// Set an appropriate speed for the NavMeshAgent.
		nav.speed = patrolSpeed;
		
		// If near the next waypoint or there is no destination...
		if(nav.destination == lastPlayerSighting.resetPosition || nav.remainingDistance < nav.stoppingDistance)
		{
			// ... increment the timer.
			patrolTimer += Time.deltaTime;
			
			// If the timer exceeds the wait time...
			if(patrolTimer >= patrolWaitTime)
			{
				// ... increment the wayPointIndex.
				if(wayPointIndex == patrolWayPoints.Length - 1)
					wayPointIndex = 0;
				else
					wayPointIndex++;
				
				// Reset the timer.
				patrolTimer = 0;
			}
		}
		else
			// If not near a destination, reset the timer.
			patrolTimer = 0;
		
		// Set the destination to the patrolWayPoint.
		nav.destination = patrolWayPoints[wayPointIndex].position;
	}
}