using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	GameObject testAgent2;
	GameObject testAgent1;

	// Use this for initialization
	void Start () {
	testAgent1 = GameObject.FindGameObjectWithTag ("testAgent1");
		testAgent2 = GameObject.FindGameObjectWithTag ("testAgent2");
	}
	
	// Update is called once per frame
	void LateUpdate () {
				if (RVO.Simulator.Instance.getNumAgents () == 0)
						return;
				RVO.Vector2 pos = RVO.Simulator.Instance.getAgentPosition (0);
		testAgent1.transform.position = new Vector3 (pos.x_,0, pos.y_);
		pos = RVO.Simulator.Instance.getAgentPosition (1);
		testAgent2.transform.position = new Vector3 (pos.x_, 0, pos.y_);
	}
}
