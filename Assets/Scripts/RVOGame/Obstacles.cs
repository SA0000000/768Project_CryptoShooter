using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for RVOGame agents
namespace RVOGame
{
		public class Obstacles
		{
		internal int _obstacleID{ get; set;}
		//Stores the number of vertices in the obstacle
				private int _numVertices ;
				//Stores the vertex locations (anti-clockwise)
		private IList<RVO.Vector2 > _vertices;

				//Setter
				public void setVertices (IList<RVO.Vector2 > vertexList, int ID)
				{
			_obstacleID = ID;
						_numVertices = vertexList.Count;
						_vertices = vertexList;

				}

		//Getter
		public IList<RVO.Vector2 > getVertices ()
		{
			return _vertices;
			
		}

		}
}

