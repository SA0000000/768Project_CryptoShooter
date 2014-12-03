using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for RVOGame agents

namespace RVOGame{
	
	public class Agents
	{
		//Agent ID
		private int _agentID;
		//Spawning position
		private RVO.Vector2 _initPosition;
		//Index of the next intermediate/final goal in list _path
		internal int _nextGoalIndex  { get;  set; }
		//Name of the goal node
		internal string _finalGoal { get;  set; }
		//Stores the names of the intermediate goals and the final goal (in that order)
		internal IList<string> _path  { get;  set; }

		public Agents(int id, RVO.Vector2 pos, string goal){
			_agentID = id;
			_initPosition = pos;
			_finalGoal = goal;
				}

		public int getAgentID(){
			return _agentID;
		}

		public RVO.Vector2 getInitPosition(){
			return _initPosition;
				}

		public void setInitPosition(RVO.Vector2 pos){
			_initPosition = pos;
		}


	}
	


}