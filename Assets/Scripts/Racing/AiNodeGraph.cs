using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AiNodeGraph : MonoBehaviour
{
		public List<Transform> myNodes = new List<Transform> ();
		public List<Transform> mySpeedZones = new List<Transform> ();
		public bool forceShowNodes = false;
		public bool updateInRealtime = false;
		public float detectionRange = 20f;
		private int myNodeCount = 0;
		private int mySpeedCount = 0;

		void OnDrawGizmosSelected ()
		{
				if (!forceShowNodes) {
						for (int i = 0; i < myNodes.Count; i++) {
								if (myNodes [i] != null) {
										Gizmos.color = Color.red;
										Gizmos.DrawWireSphere (myNodes [i].position, detectionRange);
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
						for (int i = 0; i < mySpeedZones.Count; i++) {
								Gizmos.color = Color.blue;
								Gizmos.DrawWireCube (mySpeedZones [i].position, mySpeedZones [i].localScale);
						}
				}
		}
	
		void OnDrawGizmos ()
		{
				if (forceShowNodes) {
						for (int i = 0; i < myNodes.Count; i++) {
								if (myNodes [i] != null) {
										Gizmos.color = Color.red;
										Gizmos.DrawWireSphere (myNodes [i].position, detectionRange);
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
						for (int i = 0; i < mySpeedZones.Count; i++) {
								Gizmos.color = Color.blue;
								Gizmos.DrawWireCube (mySpeedZones [i].position, mySpeedZones [i].transform.lossyScale);
						}
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
				myNodeCount = -1;
				mySpeedCount = -1;
				if (updateInRealtime) {
						myNodes.Clear ();
						mySpeedZones.Clear ();
						foreach (Transform child in transform) {
								if (child.gameObject.tag == "Node") {
										myNodeCount++;
										myNodes.Add (child.gameObject.transform);
										child.gameObject.name = myNodeCount.ToString ();
										;
										//Debug.Log (child.gameObject.name + " Added to " + this.name);
								}
								if (child.gameObject.tag == "SpeedZone") {
										mySpeedCount++;
										mySpeedZones.Add (child.gameObject.transform);
										child.gameObject.name = "SpeedZone " + mySpeedCount;
										//Debug.Log (child.gameObject.name + " Added to " + this.name);
								}
						}
				}
		}

		public Transform GetClosestWaypoint (Vector3 testFrom)
		{
				Transform tMin = null;
				float minDist = Mathf.Infinity;
				foreach (Transform t in myNodes) {
						float dist = Vector3.Distance (t.position, testFrom);
						if (dist < minDist) {
								tMin = t;
								minDist = dist;
						}
				}
				return tMin;
		}
}
