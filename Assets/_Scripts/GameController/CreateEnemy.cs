using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RVOGame
{
		public class CreateEnemy : MonoBehaviour
		{
				//Note: numEnemyAgents = RvoAgents - 1 since RvoAgent[0] is player
				public static int numEnemyAgents = 41;	// to set number of enemies
				public GameObject enemy;	//Enemy prefab to be spawned
				public GameObject player;   //Player object
				GameObject[] enemies = new GameObject[numEnemyAgents];		// All the enemies that have been spawned
				public RVO.Simulator sim;
				private RVOGame.SceneLayout _sceneLayout; //SceneLayout object defines the the scene and the initial positions
				RVOGame.Roadmap _roadmap; //Roadmap object defines the goals, waypoints and connectivity	
				private int numAgents; //total RVO agents
				private int numObstacles; //total RVO obstacles
				private int defaultWeight;

				public static double SQ_DIST_TO_GOAL_THRESH = 1f; //To decide when a player has reached its goal/waypoint
				public static double SQ_DIST_TO_PLAYER_THRESH = 50000.0f; //To decide when a player has reached its goal/waypoint

				public static float ENEMY_PREF_SPEED = 1.3f; //Same preferred speed for now
				public static float RVO_AGENT_RADIUS = 0.5f; //Same radius for player and enemies

				public Material[] imageShareTexture;	//store the image shares

				void Start ()
				{
				
						//Set reference and initialize objects
						player = GameObject.FindGameObjectWithTag ("Player");
						_sceneLayout = new RVOGame.SceneLayout ();
						sim = new RVO.Simulator ();
						numObstacles = _sceneLayout._obstacles.Count;
						numAgents = _sceneLayout._agents.Count;
						defaultWeight = RVOGame.Roadmap.NODE_NODE_DIST;

						//Setup RVO simulator
						setupSimulator ();
						//Process scene graph
						createRoadMap ();
						//Setup global plans for each agent
						initGlobalPlans ();	   
						//Place agents
						spawnAgents ();
				}
	
		
				// Update is called once per frame
				void Update ()
				{
						//Relay the main player's position to RVO
						syncRVOUnity ();
						//Update the roadmap to reflect the main players position
						updateRoadmap();
						//Set each agents preferred velocity
						setPreferredVelocities ();
						//Use RVO to compute current velocity
						RVO.Simulator.Instance.setTimeStep (Time.deltaTime);
						RVO.Simulator.Instance.doStep ();
						//Animate the players w.r.t. to their current velocity
						setEnemyState ();
						//reset the roadmap
						resetRoadmap();
				}

				void spawnAgents ()
				{
						//to set image tag share value
						int shareNum;
						Transform imageShare;
						
						//Place the player first
						player.transform.position = new Vector3 (_sceneLayout._agents [0].getInitPosition ().x_, 0.0f, _sceneLayout._agents [0].getInitPosition ().y_);
						//Then place each agent
						for (int i=1; i < numAgents; i++) 
						{
								int agentID = _sceneLayout._agents [i].getAgentID ();
								Vector3 spawnPosition = new Vector3 (_sceneLayout._agents [agentID].getInitPosition ().x_, 0.0f, _sceneLayout._agents [agentID].getInitPosition ().y_);
								Quaternion spawnRotation = Quaternion.identity;
								enemies [i - 1] = (GameObject)Instantiate (enemy, spawnPosition, spawnRotation);
								
								//Randomly assign image tag shares to the enemy
								imageShare = enemies[i-1].transform.Find("Tag_share");
								if(imageShare!=null)	//found tag_share object...
								{
									//...now set the image_share texture randomly to either share 1 or share 2
									shareNum = UnityEngine.Random.Range(0,2);
									enemies[i-1].transform.Find("Tag_share").renderer.material = imageShareTexture[shareNum];
									//set the imageShareNum to image share number for later reference
									enemies[i-1].GetComponent<EnemyHealth>().setEnemyShare(shareNum);
									enemies[i-1].transform.Find ("Tag_share").renderer.enabled = true;
								}
						}
				}
	
				void createRoadMap ()
				{
						_roadmap = new RVOGame.Roadmap ();
						_roadmap.populateGraph ();
				}
	
		void updateRoadmap(){
			RVO.Vector2 mainCharPos = new RVO.Vector2 (player.transform.position.x, player.transform.position.z);
			foreach (Node node in _roadmap.graph.Nodes.Values)
			{
				foreach (NodeConnection edge in node.Connections)
				{
					double distance = RVO.RVOMath.distSqPointLineSegment(mainCharPos, node.Position, edge.Target.Position);
					int weight;
					if (distance != 0 ) weight = int.MaxValue -Convert.ToInt32(distance);
					else weight = int.MaxValue;
					node.AddWeight(edge,weight);
				}
			}

		}

		void resetRoadmap(){
			foreach (Node node in _roadmap.graph.Nodes.Values) {
				foreach (NodeConnection edge in node.Connections){
					node.SetWeight(edge,defaultWeight);
				}
			}
			
		}

				void initGlobalPlans ()
				{
						//Do not need to set the global path for main player, so start at 1
						for (int i = 1; i < numAgents; i++) {
								updateGlobalPlans (_sceneLayout._agents [i].getAgentID ());
						}
				}
	
				void setupSimulator ()
				{
						//Number of threads - fix it to 1 for now.
						RVO.Simulator.Instance.SetNumWorkers (1);
						//Timestep of the simulator :  need to see how this relates to Unity's timestep
						RVO.Simulator.Instance.setTimeStep (0.1f);
						RVO.Simulator.Instance.setAgentDefaults (15.0f, 10, 10.0f, 5.0f, RVO_AGENT_RADIUS, ENEMY_PREF_SPEED, new RVO.Vector2 (0.0f, 0.0f));
						addObstaclesAndAgents ();
				}
	
				void addObstaclesAndAgents ()
				{
						for (int i = 0; i < numAgents; i++) {
								RVO.Simulator.Instance.addAgent (_sceneLayout._agents [i].getInitPosition ());
						}
						for (int i = 0; i < numObstacles; i++) {
								RVO.Simulator.Instance.addObstacle (_sceneLayout._obstacles [i].getVertices ());
						}
						RVO.Simulator.Instance.processObstacles ();
						//Sanity check for synchronization
						if (!RVOandRVOGameSynced ())
								throw new ApplicationException ("RVO Simulator and RVO Game not synced on initialization");
				}
	
				//Move each enemy agent based on the current velocity given by RVO
				void setEnemyState ()
				{
						for (int i = 1; i < numAgents; i++) {
								enemies [i - 1].GetComponent<EnemyAnimation> ().moveAgent (new Vector3 (RVO.Simulator.Instance.getAgentVelocity (i).x_, 0.0f, RVO.Simulator.Instance.getAgentVelocity (i).y_));
						}		
				}

				void setPreferredVelocities ()
				{
						//Do not need to set the global path for main player, so start at i
						for (int i = 1; i < numAgents; i++) {
								int agentID = _sceneLayout._agents [i].getAgentID ();
								//Skip if agent is in "Identify" state
								if (enemies [agentID - 1].GetComponent<EnemyAI> ().inFieldRange) {
										RVO.Simulator.Instance.setAgentPrefVelocity ((agentID), new RVO.Vector2 (0, 0));
										continue;
								}
								//Skip if agent is in "Killed" state; Just virtually place it outside the domain s.t. it doesnt affect collision avoidance
								if (!enemies [agentID - 1].GetComponent<EnemyAI> ().Alive) {
										RVO.Simulator.Instance.setAgentPosition (agentID - 1, new RVO.Vector2 (10000, 10000));
										continue;
								}
								//Is the main player visible?
								bool playerVisible = RVO.Simulator.Instance.queryVisibility (RVO.Simulator.Instance.agents_ [agentID].position_, RVO.Simulator.Instance.agents_ [0].position_, RVO_AGENT_RADIUS);
								double distanceToMainPlayerSq = RVO.Vector2.distSq (RVO.Simulator.Instance.agents_ [agentID].position_, RVO.Simulator.Instance.agents_ [0].position_);
								//Check if enemy is heading towards player					
								RVO.Vector2 playerDirection = new RVO.Vector2 (player.transform.forward.x, player.transform.forward.z);
								double headingToPlayer = RVO.Simulator.Instance.getAgentPrefVelocity (agentID) * playerDirection;	
								//If main player is visible, is within range and is heading towards this enemy, switch paths
								if (playerVisible && distanceToMainPlayerSq < SQ_DIST_TO_PLAYER_THRESH && headingToPlayer < -.2f) {
										//Pick the furthest goal in the direction of the players heading 
										Node finalGoal = _roadmap.randomNodeInDirection (RVO.Simulator.Instance.agents_ [agentID].position_, playerDirection);
										//Pick the closest visible goal in the direction of the players heading
										Node startNode = _roadmap.graph.Nodes [closestVisibleNodeInDirection (agentID, playerDirection)];
										//Update the final goal based on Goal Transitions object defined in SceneLayout	if they are the same				
										if (startNode == finalGoal) {
												finalGoal = _roadmap.graph.Nodes [_sceneLayout.getFinalGoal (finalGoal.Name)];										
										}
										//Set the final goal
										_sceneLayout._agents [agentID]._finalGoal = finalGoal.Name;
										//Reset the nextGoalIndex
										_sceneLayout._agents [agentID]._nextGoalIndex = 0;
										//Compute and store the goal
										_sceneLayout._agents [agentID]._path = _roadmap.getDijkstraPath (startNode.Name, _sceneLayout._agents [agentID]._finalGoal);
										RVO.Simulator.Instance.setAgentPrefVelocity (agentID, RVO.RVOMath.normalize (startNode.Position - RVO.Simulator.Instance.agents_ [agentID].position_));
								} 
				//Case: Main player does not affect enemy agents
				else {
										int goalIdx = _sceneLayout._agents [agentID]._nextGoalIndex;
										Node goalNode = _roadmap.graph.Nodes [_sceneLayout._agents [agentID]._path [goalIdx]];
										double distanceToWayPoint = RVO.Vector2.distSq (goalNode.Position, RVO.Simulator.Instance.agents_ [agentID].position_);
										if (distanceToWayPoint < SQ_DIST_TO_GOAL_THRESH) {
												if (!goalNode.Name.Equals (_sceneLayout._agents [agentID]._finalGoal)) {
														goalIdx++;
														_sceneLayout._agents [agentID]._nextGoalIndex = goalIdx;
												} else {
														updateGlobalPlans (agentID);
												}
										}
					if (goalIdx>= _sceneLayout._agents [agentID]._path.Count){
						//NOOOOOO
						RVO.Simulator.Instance.setAgentPrefVelocity (agentID, new RVO.Vector2(0,0));
						_sceneLayout._agents [agentID]._nextGoalIndex = 0;

						break;
					}
										goalNode = _roadmap.graph.Nodes [_sceneLayout._agents [agentID]._path[goalIdx]];
										RVO.Simulator.Instance.setAgentPrefVelocity (agentID, RVO.RVOMath.normalize (goalNode.Position - RVO.Simulator.Instance.agents_ [agentID].position_));
								}				

						}		
				}

		string closestVisibleNodeInDirection (int agentID, RVO.Vector2 dir)
		{
			double distance = double.PositiveInfinity;
			double distanceBehind = double.PositiveInfinity;
			
			RVO.Vector2 Agentpos = RVO.Simulator.Instance.agents_ [agentID].position_;
			string startNode = "";
			string startNodeBehind = "";
			foreach (Node node in _roadmap.graph.Nodes.Values) {
				double currentDistance = RVO.Vector2.distSq (node.Position, Agentpos);
				RVO.Vector2 posToGoal = node.Position - Agentpos;
				double goalProjection = posToGoal * dir;
				if (goalProjection > 0) {
					if (currentDistance < distance && RVO.Simulator.Instance.queryVisibility (Agentpos, node.Position, RVO_AGENT_RADIUS)) {
						distance = currentDistance;
						startNode = node.Name;
					}
				} else if (goalProjection <= 0) {
					if (currentDistance < distanceBehind && RVO.Simulator.Instance.queryVisibility (Agentpos, node.Position, RVO_AGENT_RADIUS)) {
						distanceBehind = currentDistance;
						startNodeBehind = node.Name;
					}
				}
			}
			if (startNode.Equals (""))
				return startNodeBehind;
			else
				return startNode;
		}

				string closestVisibleGoal (int agentID)
				{
						//Set start node to closest visible node to this agent's current position
						double distance = double.PositiveInfinity;
						string startNode = "";
						foreach (Node node in _roadmap.graph.Nodes.Values) {
								double currentDistance = RVO.Vector2.distSq (node.Position, RVO.Simulator.Instance.agents_ [agentID].position_);
								if (currentDistance < distance && RVO.Simulator.Instance.queryVisibility (RVO.Simulator.Instance.agents_ [agentID].position_, node.Position, RVO_AGENT_RADIUS)) {
										distance = currentDistance;
										startNode = node.Name;
								}
						}//end of foreach
						if (startNode.Equals ("", StringComparison.Ordinal))
								throw new ApplicationException ("startNode empty");
						return startNode;
				}

				void updateGlobalPlans (int agentID)
				{
						//Set start node to closest visible node to this agent's current position
						string startNode = closestVisibleGoal (agentID);

						//Update the final goal based on Goal Transitions object defined in SceneLayout
						_sceneLayout._agents [agentID]._finalGoal = _sceneLayout.getFinalGoal (startNode);
						//Reset the nextGoalIndex
						_sceneLayout._agents [agentID]._nextGoalIndex = 0;
						//Compute and store the goal
						_sceneLayout._agents [agentID]._path = _roadmap.getDijkstraPath (startNode, _sceneLayout._agents [agentID]._finalGoal);
		
				}
	
				//Feed the main character position from Unity to RVO; Also feed enemy positions from Unity to RVO
				public void syncRVOUnity ()
				{
						RVO.Vector2 mainCharPos = new RVO.Vector2 (player.transform.position.x, player.transform.position.z);
						RVO.Simulator.Instance.setAgentPrefVelocity (0, new RVO.Vector2 (0, 0));
						RVO.Simulator.Instance.agents_ [0].setMainCharPos (mainCharPos);
						for (int i = 1; i < numAgents; i++) {
								RVO.Vector2 enemyPos = new RVO.Vector2 (enemies [i - 1].transform.position.x, enemies [i - 1].transform.position.z);
								RVO.Simulator.Instance.setAgentPosition (i, enemyPos);
				
						}
				}

				//Just to make sure the RVO Simulator and RVO game ids are synced
				bool RVOandRVOGameSynced ()
				{
						if ((RVO.Simulator.Instance.getNumAgents () != numAgents))
								return false;
						for (int i = 0; i < numAgents; i++) {
								if (RVO.Simulator.Instance.agents_ [i].id_ != _sceneLayout._agents [i].getAgentID ())
										return false;
						}
						for (int i = 0; i < numObstacles; i++) {
								if (RVO.Simulator.Instance.obstacles_ [i].id_ != _sceneLayout._obstacles [i]._obstacleID)
										return false;
						}
						return true;			
				}
		
				void DebugPrint ()
				{
						int id = 2;
						int goalIdx = _sceneLayout._agents [id]._nextGoalIndex;
						Node goalNode = _roadmap.graph.Nodes [_sceneLayout._agents [id]._path [goalIdx]];
						Debug.Log ("Agent ID : " + id + "Unity Position: " + enemies [id - 1].transform.position + "RVO Pos: " + RVO.Simulator.Instance.getAgentPosition (id) + "Game Goal" + goalNode.Position + "RVO Pref Vel : " + RVO.Simulator.Instance.getAgentPrefVelocity (id) + "RVO Current Velocity :" + RVO.Simulator.Instance.getAgentVelocity (id));
				}
		}
}