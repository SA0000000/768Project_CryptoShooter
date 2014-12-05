using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RVOGame
{
		public class CreateEnemy : MonoBehaviour
		{
				//Note: numEnemyAgents = RvoAgents - 1 since RvoAgent[0] is player
				public static int numEnemyAgents = 4;	// to set number of enemies
				public GameObject enemy;	//Enemy prefab to be spawned
				public GameObject player;   //Player object
				GameObject[] enemies = new GameObject[numEnemyAgents];		// All the enemies that have been spawned
				private int numAgents;
				private int numObstacles;
				public RVO.Simulator sim;
				private RVOGame.SceneLayout _sceneLayout; //SceneLayout object defines the the scene and the initial positions
				RVOGame.Roadmap _roadmap; //Roadmap object
				public static double SQ_DIST_TO_GOAL_THRESH = 1f; //To decide when a player has reached its goal/waypoint
				public static float PREF_SPEED = 1.3f; //Same preferred speed for now

				void Start ()
				{
						//Set reference
						player = GameObject.FindGameObjectWithTag ("Player");
						//Initialize objects
						_sceneLayout = new RVOGame.SceneLayout ();
						sim = new RVO.Simulator ();
						numObstacles = _sceneLayout._obstacles.Count;
						numAgents = _sceneLayout._agents.Count;
						//Setup simulator
						setupSimulator ();
						createRoadMap ();
						initGlobalPlans ();	   
						//Setup scene
						spawnAgents ();
				}
	
				void spawnAgents ()
				{
						//Place the player first
						player.transform.position = new Vector3 (_sceneLayout._agents [0].getInitPosition ().x_, 0.0f, _sceneLayout._agents [0].getInitPosition ().y_);
						//Then place each agent
						for (int i=1; i < numAgents; i++) {
								int agentID = _sceneLayout._agents [i].getAgentID ();
								Vector3 spawnPosition = new Vector3 (_sceneLayout._agents [agentID].getInitPosition ().x_, 0.0f, _sceneLayout._agents [agentID].getInitPosition ().y_);
								Quaternion spawnRotation = Quaternion.identity;
								enemies [i - 1] = (GameObject)Instantiate (enemy, spawnPosition, spawnRotation);
						}
				}
	
				void createRoadMap ()
				{
						_roadmap = new RVOGame.Roadmap ();
						_roadmap.populateGraph ();
				}
	
				void initGlobalPlans ()
				{
						//Do not need to set the global path for main player, so start at i
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
						//Might have to change the agent defaults
						RVO.Simulator.Instance.setAgentDefaults (15.0f, 10, 10.0f, 5.0f, 2.0f, 2.0f, new RVO.Vector2 (0.0f, 0.0f));
						addObstaclesAndAgents ();
				}

				void DebugPrint ()
				{
						int id = 2;
						int goalIdx = _sceneLayout._agents [id]._nextGoalIndex;
						Node goalNode = _roadmap.graph.Nodes [_sceneLayout._agents [id]._path [goalIdx]];
						Debug.Log ("Agent ID : " + id + "Unity Position: " + enemies [id - 1].transform.position + "RVO Pos: " + RVO.Simulator.Instance.getAgentPosition (id) + "Game Goal" + goalNode.Position + "RVO Pref Vel : " + RVO.Simulator.Instance.getAgentPrefVelocity (id) + "RVO Current Velocity :" + RVO.Simulator.Instance.getAgentVelocity (id));
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
	
				// Update is called once per frame
				void Update ()
				{
						//Relay the main player's position to RVO
						syncRVOUnity ();
						//Set each agents preferred velocity
						setPreferredVelocities ();
						//Use RVO to compute current velocity
			RVO.Simulator.Instance.setTimeStep (Time.deltaTime);
				RVO.Simulator.Instance.doStep ();
						DebugPrint ();
						//Animate the players w.r.t. to their current velocity
						setEnemyState ();
				}

				void shareMainCharacter ()
				{
						RVO.Vector2 mainCharPos = new RVO.Vector2 (player.transform.position.x, player.transform.position.z);
						RVO.Simulator.Instance.setAgentPrefVelocity (0, new RVO.Vector2 (0, 0));
						RVO.Simulator.Instance.agents_ [0].setMainCharPos (mainCharPos);
				}
	
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
								int goalIdx = _sceneLayout._agents [agentID]._nextGoalIndex;
								Node goalNode = _roadmap.graph.Nodes [_sceneLayout._agents [agentID]._path [goalIdx]];
								double distanceToWayPoint = RVO.Vector2.distSq (goalNode.Position, RVO.Simulator.Instance.agents_ [agentID].position_);
								if (distanceToWayPoint < SQ_DIST_TO_GOAL_THRESH) {
					if (!goalNode.Name.Equals(_sceneLayout._agents [agentID]._finalGoal)) {
												goalIdx++;
												_sceneLayout._agents [agentID]._nextGoalIndex = goalIdx;
										} else {
												updateGlobalPlans (agentID);
										}
								}
								goalNode = _roadmap.graph.Nodes [_sceneLayout._agents [agentID]._path [goalIdx]];
								RVO.Simulator.Instance.setAgentPrefVelocity (agentID, RVO.RVOMath.normalize (goalNode.Position - RVO.Simulator.Instance.agents_ [agentID].position_));
						}		
				}
	
				void updateGlobalPlans (int agentID)
				{
						//Set start node to closest visible node to this agent's current position
						double distance = double.PositiveInfinity;
						string startNode = "";
						foreach (Node node in _roadmap.graph.Nodes.Values) {
								double currentDistance = RVO.Vector2.distSq (node.Position, RVO.Simulator.Instance.agents_ [agentID].position_);
								if (currentDistance < distance && RVO.Simulator.Instance.queryVisibility (RVO.Simulator.Instance.agents_ [agentID].position_, node.Position, 2.0f)) {
										distance = currentDistance;
										startNode = node.Name;
								}
						}//end of foreach
						if (startNode.Equals ("", StringComparison.Ordinal))
								throw new ApplicationException ("startNode empty");
						//Update the final goal, next goal index and path for this agent
						_sceneLayout._agents [agentID]._finalGoal = _sceneLayout.getFinalGoal (startNode);
						_sceneLayout._agents [agentID]._nextGoalIndex = 0;
						_sceneLayout._agents [agentID]._path = _roadmap.getDijkstraPath (startNode, _sceneLayout._agents [agentID]._finalGoal);
		
				}
	
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
		}
}