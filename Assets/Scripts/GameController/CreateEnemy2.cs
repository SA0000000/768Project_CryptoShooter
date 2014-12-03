﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace RVOGame
{
		public class CreateEnemy : MonoBehaviour
		{

				public static int numEnemyAgents = 5;	// to set number of enemies
				public Vector3 spawnValues;	// positions where the enemy can be spawned
				public GameObject enemy;	// enemy prefab to be spawned
				public GameObject player;
				GameObject[] enemies = new GameObject[numEnemyAgents];		// all the enemies that have been spawned
				public RVO.Simulator sim;
				private RVOGame.SceneLayout _sceneLayout;
				private int numAgents;
				private int numObstacles;
				List<RVO.Vector2> goals = new List<RVO.Vector2> ();
				RVOGame.Roadmap _roadmap;
				public static double SQ_DIST_TO_GOAL_THRESH = 100.0f;
				public static float PREF_SPEED = 1.3f;

				void Start ()
				{
						_sceneLayout = new RVOGame.SceneLayout ();
						numObstacles = _sceneLayout._obstacles.Count;
						numAgents = _sceneLayout._agents.Count;
						sim = new RVO.Simulator ();
						player = GameObject.FindGameObjectWithTag ("Player");
						setupSimulator ();
						createRoadMap ();
						initGlobalPlans ();	     
						spawnAgents ();
				}

				void spawnAgents ()
				{
						player.transform.position = new Vector3 (_sceneLayout._agents [0].getInitPosition ().x_, 0.0f, _sceneLayout._agents [0].getInitPosition ().y_);
						for (int i=1; i < numAgents; i++) {
								int agentID = _sceneLayout._agents [i].getAgentID ();
								Vector3 spawnPosition = new Vector3 (_sceneLayout._agents [agentID].getInitPosition ().x_, 0.0f, _sceneLayout._agents [agentID].getInitPosition ().y_);
								Quaternion spawnRotation = Quaternion.identity;
								enemies [i] = (GameObject)Instantiate (enemy, spawnPosition, spawnRotation);
						}
				}
	
				void createRoadMap ()
				{
						_roadmap = new RVOGame.Roadmap ();
						_roadmap.populateGraph ();
				}
		
				void initGlobalPlans ()
				{
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
						RVO.Simulator.Instance.setAgentDefaults (15.0f, 10, 10.0f, 5.0f, 2.0f, 2.0f, new RVO.Vector2 (0.0f, 0.0f));
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
		
				// Update is called once per frame
				void Update ()
				{
						shareMainCharacter ();
						setPreferredVelocities ();
						RVO.Simulator.Instance.doStep ();
						setEnemyState();
				}
		
				void shareMainCharacter ()
				{
						RVO.Vector2 mainCharPos = new RVO.Vector2 (player.transform.position.x, player.transform.position.y );
						RVO.Simulator.Instance.setAgentPrefVelocity (0, new RVO.Vector2 (0, 0));
						RVO.Simulator.Instance.agents_ [0].setMainCharPos (mainCharPos);
				}

		void setEnemyState ()
		{
			for (int i = 1; i < numAgents; i++) {
				enemies[i].GetComponent<EnemyAI>().Velocity = new Vector3(RVO.Simulator.Instance.getAgentVelocity(i).x_, 0.0f, RVO.Simulator.Instance.getAgentVelocity(i).y_);
				enemies[i].GetComponent<EnemyAI>().Position = new Vector3(RVO.Simulator.Instance.getAgentPosition(i).x_, 0.0f, RVO.Simulator.Instance.getAgentPosition(i).y_);
			}		
		}

				void setPreferredVelocities ()
				{
						for (int i = 1; i < numAgents; i++) {
								int agentID = _sceneLayout._agents [i].getAgentID ();
								int goalIdx = _sceneLayout._agents [agentID]._nextGoalIndex;
								Node goalNode = _roadmap.graph.Nodes [_sceneLayout._agents [agentID]._path [goalIdx]];
								if (RVO.Vector2.distSq (goalNode.Position, RVO.Simulator.Instance.agents_ [agentID].position_) < SQ_DIST_TO_GOAL_THRESH) {
										if (goalIdx < (_sceneLayout._agents [agentID]._path.Count - 1))
												goalIdx++;
										else {
												setNewGoal (agentID);
												updateGlobalPlans (agentID);
										}
								}
								goalNode = _roadmap.graph.Nodes [_sceneLayout._agents [agentID]._path [goalIdx]];
								RVO.Simulator.Instance.setAgentPrefVelocity (agentID, (goalNode.Position - RVO.Simulator.Instance.agents_ [agentID].position_) * PREF_SPEED);
						}		
				}
		
				void setNewGoal (int agentID)
				{
					
				}

				void updateGlobalPlans (int agentID)
				{
						//Find the closest visible node to this agent's initial position
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
						//Update the path and goalIndex
						_sceneLayout._agents [agentID]._path = _roadmap.getDijkstraPath (startNode, _sceneLayout._agents [agentID]._finalGoal);
						_sceneLayout._agents [agentID]._nextGoalIndex = 0;
			
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