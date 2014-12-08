using System.Collections;
using System.Collections.Generic;
using System;

namespace RVOGame
{
		public class Node
		{
				//Graph edges
				IList<NodeConnection> _connections;

				//Name of the parent node in Dijkstra search
				internal string Parent { get; set; }

				//Name of the node
				internal string Name { get; set; }

				//Position of the Node
				internal RVO.Vector2 Position { get; set; }
	
				//Stores the distance to this node from the start node
				internal double DistanceFromStart { get; set; }
	
				internal IEnumerable<NodeConnection> Connections {
						get { return _connections; }
				}
	
		internal void AddWeight (NodeConnection edge, int weight)
		{
			_connections[_connections.IndexOf(edge)].Distance = _connections[_connections.IndexOf(edge)].Distance + weight;
		}

		internal void SetWeight (NodeConnection edge, int weight)
		{
			_connections[_connections.IndexOf(edge)].Distance = weight;
		}

		//Constructor
		internal Node ()
		{

		}

				//Constructor
				internal Node (string name, RVO.Vector2 position)
				{
						Name = name;
						Position = position;
						_connections = new List<NodeConnection> ();
						Parent = null;
				}
	
				//Method for adding edges
				internal void AddConnection (Node targetNode, double distance, bool twoWay)
				{
						if (targetNode == null)
								throw new ArgumentNullException ("targetNode");
						if (targetNode == this)
								throw new ArgumentException ("Node may not connect to itself.");
						if (distance <= 0)
								throw new ArgumentException ("Distance must be positive.");
		
						_connections.Add (new NodeConnection (targetNode, distance));
						if (twoWay)
								targetNode.AddConnection (this, distance, false);
				}
		}

//Class for holding the edges. Defined by target and distance
		internal class NodeConnection
		{
				internal Node Target { get; private set; }

				internal double Distance { get; set; }
	
				internal NodeConnection (Node target, double distance)
				{
						Target = target;
						Distance = distance;
				}


		}
}