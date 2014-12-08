using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//Wrapper for the Graph. 

namespace RVOGame
{
		public class Roadmap
		{
				
				public static int NODE_NODE_DIST = 500;
				//Graph datastructure
				public Graph graph;

				//Constructor
				public Roadmap ()
				{
						graph = new Graph ();
				}

				//Populates the graph with nodes and edges
				public void populateGraph ()
				{
						//Nodes
						graph.AddNode ("G_TR", new RVO.Vector2 (970, 970));
						graph.AddNode ("G_TL", new RVO.Vector2 (30, 970));
						graph.AddNode ("G_BR", new RVO.Vector2 (970, 30));
						graph.AddNode ("G_BL", new RVO.Vector2 (30, 30));
						graph.AddNode ("W_ML", new RVO.Vector2 (30, 500));
						graph.AddNode ("W_MC", new RVO.Vector2 (500, 500));
						graph.AddNode ("W_MR", new RVO.Vector2 (970, 500));
						graph.AddNode ("W_MT", new RVO.Vector2 (500, 1000));
						graph.AddNode ("W_MB", new RVO.Vector2 (500, 30));		
						//Connections
						graph.AddConnection ("G_BL", "W_MB", NODE_NODE_DIST, true);
						graph.AddConnection ("G_BL", "W_ML", NODE_NODE_DIST, true);
						graph.AddConnection ("G_TL", "W_MT", NODE_NODE_DIST, true);
						graph.AddConnection ("G_TL", "W_MB", NODE_NODE_DIST, true);
						graph.AddConnection ("G_TR", "W_MT", NODE_NODE_DIST, true);
						graph.AddConnection ("G_TR", "W_MR", NODE_NODE_DIST, true);
						graph.AddConnection ("G_BR", "W_MR", NODE_NODE_DIST, true);
						graph.AddConnection ("G_BR", "W_MB", NODE_NODE_DIST, true);
						graph.AddConnection ("W_MC", "W_MB", NODE_NODE_DIST, true);
						graph.AddConnection ("W_MC", "W_MR", NODE_NODE_DIST, true);
						graph.AddConnection ("W_MC", "W_ML", NODE_NODE_DIST, true);
						graph.AddConnection ("W_MC", "W_MT", NODE_NODE_DIST, true);
				}



				//Finds the shortest path from startNode to endNode
				public IList<string> getDijkstraPath (string startNode, string endNode)
				{
						IList<string> temp, path;
						path = new List<string> ();
						temp = new List<string> ();
						var calculator = new GraphDistanceCalculator ();
						var dictNodeDistance = calculator.CalculateDistances (graph, startNode);  // Start from "G"	
						//Debug.Log ("Shortest distance to node: " + endNode + " from source " + startNode + " is " + dictNodeDistance [endNode]);
						Node currentNode = graph.Nodes [endNode];
						//Debug.Log ("Beginning to backtrack");
						temp.Add (currentNode.Name);
						while (currentNode.Name != startNode) {
								//Debug.Log ("\t\tShortest distance to node: " + currentNode.Name + " is " + dictNodeDistance [currentNode.Name]);
								currentNode = graph.Nodes [currentNode.Parent];
								temp.Add (currentNode.Name);
						}
						for (int i = (temp.Count-1); i >= 0; i--) {
								path.Add (temp [i]);
						}
						return path;
				}

		public Node randomNodeInDirection (RVO.Vector2 pos, RVO.Vector2 dir)
		{
			Node lastNode = new Node();
			System.Random randomGenerator = new System.Random ();
			IList<Node> nodeList = new List<Node> ();
			foreach (Node node in graph.Nodes.Values) {
				lastNode = node;
				RVO.Vector2 posToGoal = node.Position - pos;
				double goalProjection = posToGoal * dir;
				if (goalProjection > 0) {
					nodeList.Add(node);
				}
			}
			if (nodeList.Count == 0)
				return lastNode;
			else
			return nodeList[randomGenerator.Next (nodeList.Count-1)];


		}

				public Node farthestNodeInDirection (RVO.Vector2 pos, RVO.Vector2 dir)
				{
						Node furthestNode = new Node ();
						double furthestDistance = double.NegativeInfinity;
						foreach (Node node in graph.Nodes.Values) {
								RVO.Vector2 posToGoal = node.Position - pos;
								double goalProjection = posToGoal * dir;
								if (goalProjection > furthestDistance) {
										furthestDistance = goalProjection;
										furthestNode = node;
								}
						}
						return furthestNode;
				}

		}

		public class GraphDistanceCalculator
		{
				public IDictionary<string, double> CalculateDistances (Graph graph, string startingNode)
				{
						if (!graph.Nodes.Any (n => n.Key == startingNode))
								throw new ArgumentException ("Starting node must be in graph.");
						InitialiseGraph (graph, startingNode);
						ProcessGraph (graph, startingNode);
						return ExtractDistances (graph);
				}

				private void InitialiseGraph (Graph graph, string startingNode)
				{
						foreach (Node node in graph.Nodes.Values) {
								node.DistanceFromStart = double.PositiveInfinity;
								node.Parent = null;
						}
						graph.Nodes [startingNode].DistanceFromStart = 0;
				}
		  
				private void ProcessGraph (Graph graph, string startingNode)
				{
						bool finished = false;
						var queue = graph.Nodes.Values.ToList ();
						while (!finished) {
								Node nextNode = queue.OrderBy (n => n.DistanceFromStart).FirstOrDefault (
				n => !double.IsPositiveInfinity (n.DistanceFromStart));
								if (nextNode != null) {
										ProcessNode (nextNode, queue);
										queue.Remove (nextNode);
								} else {
										finished = true;
								}
						}
				}

				private void ProcessNode (Node node, List<Node> queue)
				{
						var connections = node.Connections.Where (c => queue.Contains (c.Target));
						foreach (var connection in connections) {
								double distance = node.DistanceFromStart + connection.Distance;
								if (distance < connection.Target.DistanceFromStart) {
										connection.Target.DistanceFromStart = distance;
										connection.Target.Parent = node.Name;
								}
						}
				}

				private IDictionary<string, double> ExtractDistances (Graph graph)
				{
						return graph.Nodes.ToDictionary (n => n.Key, n => n.Value.DistanceFromStart);
				}

		}
}