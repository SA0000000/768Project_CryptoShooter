using System.Collections;
using System.Collections.Generic;
using System;

namespace RVOGame {
public class SceneLayout
{

		//Number of Obstacles and their configuration
		private int numObstacles;
		internal IList<RVOGame.Obstacles> _obstacles{ get; set;}
		//Number of Agents and their initial positions
		internal IList<RVOGame.Agents> _agents{ get; set;}
		IDictionary<string,string> dictGoalTransition;

		public SceneLayout ()
		{
				_obstacles = new List<RVOGame.Obstacles> ();
				_agents = new List<RVOGame.Agents> ();
				dictGoalTransition = new Dictionary<string, string>();
				initObstacles ();
				initAgents ();
				initGoalTransitions ();
		}

		public void initGoalTransitions(){
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

		public string getFinalGoal(string startNode){
			if (!dictGoalTransition.ContainsKey(startNode))
			{
					throw new ApplicationException ("Start node " + startNode + " does not exist in goal transition dictionary");
			}
			return dictGoalTransition [startNode];
		}

	public void initAgents(){
			RVOGame.Agents agent1 = new RVOGame.Agents (0, new RVO.Vector2(500,500), "G_BR");
			_agents.Add (agent1);
			RVOGame.Agents agent2 = new RVOGame.Agents (1, new RVO.Vector2(900,750), "G_BL");
			_agents.Add (agent2);
			RVOGame.Agents agent3 = new RVOGame.Agents (2, new RVO.Vector2(100,100), "G_BR");
			_agents.Add (agent3);
			RVOGame.Agents agent4 = new RVOGame.Agents (3, new RVO.Vector2(900,250), "G_TL");
			_agents.Add (agent4);
			RVOGame.Agents agent5 = new RVOGame.Agents (4, new RVO.Vector2(100,750), "G_BR");
			_agents.Add (agent5);
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
				vertices4.Add (new RVO.Vector2 (200,700));
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

		
		public IList<RVOGame.Agents> getAgentList(){
			return _agents;
		}
		
		public IList<RVOGame.Obstacles> getObstacleList(){
			return _obstacles;
		}


}
}


