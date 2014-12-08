using System.Collections;
using System.Collections.Generic;
using System;

namespace RVOGame
{
		public class SceneLayout
		{

				//Number of Obstacles and their configuration
				private int numObstacles;

				internal IList<RVOGame.Obstacles> _obstacles{ get; set; }
				//Number of Agents and their initial positions
				internal IList<RVOGame.Agents> _agents{ get; set; }

				IDictionary<string,string> dictGoalTransition;
				Random randomGenerator ;
				internal IList<string> _goals;

				public SceneLayout ()
				{
						_obstacles = new List<RVOGame.Obstacles> ();
						_agents = new List<RVOGame.Agents> ();
						_goals = new List<string> ();
						dictGoalTransition = new Dictionary<string, string> ();
						initObstacles ();
						initAgents ();
						//initGoalTransitions ();
						initGoals ();
						randomGenerator = new Random ();
				}

				public void initGoals ()
				{
						_goals.Add ("G_TL");
						_goals.Add ("G_TR");
						_goals.Add ("G_BL");
						_goals.Add ("G_BR");
						_goals.Add ("G_MB");
				}

				public void initGoalTransitions ()
				{
						dictGoalTransition.Add ("G_TL", "G_BR");
						dictGoalTransition.Add ("G_TR", "G_BL");
						dictGoalTransition.Add ("G_BL", "G_TR");
						dictGoalTransition.Add ("G_BR", "G_TL");
						dictGoalTransition.Add ("W_MB", "G_TR");
						dictGoalTransition.Add ("W_MC", "G_TR");
						dictGoalTransition.Add ("W_MT", "G_BL");
						dictGoalTransition.Add ("W_MR", "G_TL");
						dictGoalTransition.Add ("W_ML", "G_BR");
				}

				public string getFinalGoal (string startNode)
				{
						int x = randomGenerator.Next (_goals.Count - 1);
						string finalGoal = _goals [x];
						while (startNode.Equals(finalGoal)) {
								x = randomGenerator.Next (_goals.Count - 1);
								finalGoal = _goals [x];
						}

						return finalGoal;
				}

				public string getFinalGoalDeprecated (string startNode)
				{
						if (!dictGoalTransition.ContainsKey (startNode)) {
								throw new ApplicationException ("Start node " + startNode + " does not exist in goal transition dictionary");
						}
						return dictGoalTransition [startNode];
				}

				public void initAgents ()
				{
						//Main Player
						_agents.Add (new RVOGame.Agents (0, new RVO.Vector2 (800, 550), "G_BR"));
						//BL players
						_agents.Add (new RVOGame.Agents (1, new RVO.Vector2 (30, 30), "G_TR"));
						_agents.Add (new RVOGame.Agents (2, new RVO.Vector2 (139, 50), "G_BR"));
						_agents.Add (new RVOGame.Agents (3, new RVO.Vector2 (244, 75), "G_TL"));
						_agents.Add (new RVOGame.Agents (4, new RVO.Vector2 (404, 30), "G_BR"));
						_agents.Add (new RVOGame.Agents (5, new RVO.Vector2 (30, 60), "G_TR"));
						_agents.Add (new RVOGame.Agents (6, new RVO.Vector2 (30, 139), "G_TL"));
						_agents.Add (new RVOGame.Agents (7, new RVO.Vector2 (30, 244), "G_TR"));
						_agents.Add (new RVOGame.Agents (8, new RVO.Vector2 (30, 404), "G_BR"));
						_agents.Add (new RVOGame.Agents (9, new RVO.Vector2 (255, 450), "G_TR"));
						_agents.Add (new RVOGame.Agents (10, new RVO.Vector2 (450, 255), "G_TL"));
						//TR players
						_agents.Add (new RVOGame.Agents (11, new RVO.Vector2 (970, 970), "G_TL"));
						_agents.Add (new RVOGame.Agents (12, new RVO.Vector2 (860, 950), "G_BR"));
						_agents.Add (new RVOGame.Agents (13, new RVO.Vector2 (750, 925), "G_BL"));
						_agents.Add (new RVOGame.Agents (14, new RVO.Vector2 (600, 970), "G_TL"));
						_agents.Add (new RVOGame.Agents (15, new RVO.Vector2 (970, 860), "G_BL"));
						_agents.Add (new RVOGame.Agents (16, new RVO.Vector2 (970, 750), "G_BR"));
						_agents.Add (new RVOGame.Agents (17, new RVO.Vector2 (970, 600), "G_TL"));
						_agents.Add (new RVOGame.Agents (18, new RVO.Vector2 (970, 940), "G_BR"));
						_agents.Add (new RVOGame.Agents (19, new RVO.Vector2 (750, 550), "G_BL"));
						_agents.Add (new RVOGame.Agents (20, new RVO.Vector2 (550, 750), "G_BR"));
						//TL players
						_agents.Add (new RVOGame.Agents (21, new RVO.Vector2 (30, 970), "G_TR"));
						_agents.Add (new RVOGame.Agents (22, new RVO.Vector2 (140, 950), "G_BR"));
						_agents.Add (new RVOGame.Agents (23, new RVO.Vector2 (250, 925), "G_BL"));
						_agents.Add (new RVOGame.Agents (24, new RVO.Vector2 (400, 970), "G_TR"));
						_agents.Add (new RVOGame.Agents (25, new RVO.Vector2 (30, 860), "G_BL"));
						_agents.Add (new RVOGame.Agents (26, new RVO.Vector2 (30, 750), "G_BR"));
						_agents.Add (new RVOGame.Agents (27, new RVO.Vector2 (30, 600), "G_BL"));
						_agents.Add (new RVOGame.Agents (28, new RVO.Vector2 (30, 940), "G_BR"));
						_agents.Add (new RVOGame.Agents (29, new RVO.Vector2 (250, 550), "G_TR"));
						_agents.Add (new RVOGame.Agents (30, new RVO.Vector2 (450, 750), "G_BR"));
						//BR players
						_agents.Add (new RVOGame.Agents (31, new RVO.Vector2 (970, 30), "G_TR"));
						_agents.Add (new RVOGame.Agents (32, new RVO.Vector2 (860, 50), "G_BL"));
						_agents.Add (new RVOGame.Agents (33, new RVO.Vector2 (750, 75), "G_TL"));
						_agents.Add (new RVOGame.Agents (34, new RVO.Vector2 (600, 30), "G_BL"));
						_agents.Add (new RVOGame.Agents (35, new RVO.Vector2 (970, 60), "G_TR"));
						_agents.Add (new RVOGame.Agents (36, new RVO.Vector2 (970, 139), "G_TL"));
						_agents.Add (new RVOGame.Agents (37, new RVO.Vector2 (970, 244), "G_TR"));
						_agents.Add (new RVOGame.Agents (38, new RVO.Vector2 (970, 404), "G_BL"));
						_agents.Add (new RVOGame.Agents (39, new RVO.Vector2 (750, 450), "G_TR"));
						_agents.Add (new RVOGame.Agents (40, new RVO.Vector2 (650, 255), "G_TL"));
				}
	
				public void initObstacles ()
				{
						///////First insert the symmetric objects in the scene
						//Initialize BottomLeft Object
						RVOGame.Obstacles obs1 = new RVOGame.Obstacles ();
						IList<RVO.Vector2 > vertices1 = new List<RVO.Vector2> ();
						vertices1.Add (new RVO.Vector2 (200, 200));
						vertices1.Add (new RVO.Vector2 (300, 300));
						vertices1.Add (new RVO.Vector2 (300, 300));
						vertices1.Add (new RVO.Vector2 (300, 300));
						obs1.setVertices (vertices1, 0);
						_obstacles.Add (obs1);
						//Initialize BottomRight Object
						RVOGame.Obstacles obs2 = new RVOGame.Obstacles ();
						IList<RVO.Vector2 > vertices2 = new List<RVO.Vector2> ();
						vertices2.Add (new RVO.Vector2 (700, 200));
						vertices2.Add (new RVO.Vector2 (800, 200));
						vertices2.Add (new RVO.Vector2 (800, 300));
						vertices2.Add (new RVO.Vector2 (700, 300));
						obs2.setVertices (vertices2, 1);
						_obstacles.Add (obs2);
						//Initialize TopRight Object
						RVOGame.Obstacles obs3 = new RVOGame.Obstacles ();
						IList<RVO.Vector2 > vertices3 = new List<RVO.Vector2> ();
						vertices3.Add (new RVO.Vector2 (700, 700));
						vertices3.Add (new RVO.Vector2 (800, 700));
						vertices3.Add (new RVO.Vector2 (800, 800));
						vertices3.Add (new RVO.Vector2 (700, 800));
						obs3.setVertices (vertices3, 2);
						_obstacles.Add (obs3);
						//Initialize TopLeft Object
						RVOGame.Obstacles obs4 = new RVOGame.Obstacles ();
						IList<RVO.Vector2 > vertices4 = new List<RVO.Vector2> ();
						vertices4.Add (new RVO.Vector2 (200, 700));
						vertices4.Add (new RVO.Vector2 (300, 700));
						vertices4.Add (new RVO.Vector2 (300, 800));
						vertices4.Add (new RVO.Vector2 (200, 800));
						obs4.setVertices (vertices4, 3);
						_obstacles.Add (obs4);	

//				///////Now inserting the boundary objects
//				//Initialize Bottom Wall
//		RVOGame.Obstacles obs5 = new RVOGame.Obstacles ();
//				IList<RVO.Vector2 > vertices5 = new List<RVO.Vector2> ();
//				vertices5.Add (new RVO.Vector2 (-0.01f, -0.01f));
//				vertices5.Add (new RVO.Vector2 (100.01f, -0.01f));
//				vertices5.Add (new RVO.Vector2 (100.01f, 0.0f));
//				vertices5.Add (new RVO.Vector2 (-0.01f, 0.0f));
//				obs5.setVertices (vertices5, 4);
//				_obstacles.Add (obs5);
//				//Initialize Right Wall
//		RVOGame.Obstacles obs6 = new RVOGame.Obstacles ();
//				IList<RVO.Vector2 > vertices6 = new List<RVO.Vector2> ();
//				vertices6.Add (new RVO.Vector2 (100.0f, 0.0f));
//				vertices6.Add (new RVO.Vector2 (100.01f, 0.0f));
//				vertices6.Add (new RVO.Vector2 (100.01f, 100.0f));
//				vertices6.Add (new RVO.Vector2 (100.0f, 100.0f));
//				obs6.setVertices (vertices6, 5);
//				_obstacles.Add (obs6);
//				//Initialize Left Wall
//		RVOGame.Obstacles obs7 = new RVOGame.Obstacles ();
//				IList<RVO.Vector2 > vertices7 = new List<RVO.Vector2> ();
//				vertices7.Add (new RVO.Vector2 (-0.01f, 0.0f));
//				vertices7.Add (new RVO.Vector2 (0.0f, 0.0f));
//				vertices7.Add (new RVO.Vector2 (0.0f, 100.0f));
//				vertices7.Add (new RVO.Vector2 (-0.01f, 100.0f));
//				obs7.setVertices (vertices7, 6);
//				_obstacles.Add (obs7);
//				//Initialize Top Wall
//		RVOGame.Obstacles obs8 = new RVOGame.Obstacles ();
//				IList<RVO.Vector2 > vertices8 = new List<RVO.Vector2> ();
//				vertices8.Add (new RVO.Vector2 (-0.01f, 100.0f));
//				vertices8.Add (new RVO.Vector2 (100.01f, 100.0f));
//				vertices8.Add (new RVO.Vector2 (100.01f, 100.01f));
//				vertices8.Add (new RVO.Vector2 (-0.01f, 100.01f));
//				obs8.setVertices (vertices8, 7);
//				_obstacles.Add (obs8);	
				}
		
				public IList<RVOGame.Agents> getAgentList ()
				{
						return _agents;
				}
		
				public IList<RVOGame.Obstacles> getObstacleList ()
				{
						return _obstacles;
				}


		}
}


