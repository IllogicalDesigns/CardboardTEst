using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AiNodeGraph : MonoBehaviour
{
		public List<Transform> myNodes = new List<Transform> ();
		public bool forceShowNodes = false;
		public bool updateInRealtime = false;
		private int myNodeCount = 0;

		void OnDrawGizmosSelected ()
		{
				if (!forceShowNodes) {
						for (int i = 0; i < myNodes.Count; i++) {
								if (myNodes [i] != null) {
										Gizmos.color = Color.red;
										Gizmos.DrawCube (myNodes [i].position, new Vector3 (1f, 1f, 1f));
										int tempLastCubeIndex = i;
										tempLastCubeIndex = tempLastCubeIndex - 1;
										if (tempLastCubeIndex < 0)
												tempLastCubeIndex = (myNodes.Count - 1);
										if (tempLastCubeIndex > myNodes.Count)
												tempLastCubeIndex = 0;
										Gizmos.color = Color.green;
										Gizmos.DrawLine (myNodes [tempLastCubeIndex].position, myNodes [i].position);
								}
						}
				}
		}

		void OnDrawGizmos ()
		{
				if (forceShowNodes) {
						for (int i = 0; i < myNodes.Count; i++) {
								if (myNodes [i] != null) {
										Gizmos.color = Color.red;
										Gizmos.DrawCube (myNodes [i].position, new Vector3 (1f, 1f, 1f));
										int tempLastCubeIndex = i;
										tempLastCubeIndex = tempLastCubeIndex - 1;
										if (tempLastCubeIndex < 0)
												tempLastCubeIndex = (myNodes.Count - 1);
										if (tempLastCubeIndex > myNodes.Count)
												tempLastCubeIndex = 0;
										Gizmos.color = Color.green;
										Gizmos.DrawLine (myNodes [tempLastCubeIndex].position, myNodes [i].position);
								}
						}
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
				myNodeCount = -1;
				if (updateInRealtime) {
						myNodes.Clear ();
						foreach (Transform child in transform) {
								myNodeCount++;
								if (child.gameObject.tag == "Node") {
										myNodes.Add (child.gameObject.transform);
										child.gameObject.name = "Node " + myNodeCount;
										Debug.Log (child.gameObject.name + " Added to " + this.name);
								}
						}
				}
		}
}
