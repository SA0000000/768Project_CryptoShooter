using UnityEngine;
using System.Collections;

public class MeshBoundingBox : MonoBehaviour {
	void Start() {
		print("X bounds " + renderer.bounds.min.x + " , " + renderer.bounds.max.x);
		print("Z bounds " + renderer.bounds.min.z + " , " + renderer.bounds.max.z);
	}
}
