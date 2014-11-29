using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wrapper for accessing the Node objects and constructing the graph
namespace RVOGame{
public class Graph
{
		//Node dictionary. Key -> Name; Value -> Node
	internal IDictionary<string, Node> Nodes { get;  set; }
	
		//Constructor
	public Graph()
	{
		Nodes = new Dictionary<string, Node>();
	}
	
		//Adds nodes to Nodes dictionary structure
	public void AddNode(string name, RVO.Vector2 pos)
	{
		var node = new Node(name, pos);
		Nodes.Add(name, node);
	}
	
		//Adds edges
	public void AddConnection(string fromNode, string toNode, int distance, bool twoWay)
	{
		Nodes[fromNode].AddConnection(Nodes[toNode], distance, twoWay);
	}
}
}