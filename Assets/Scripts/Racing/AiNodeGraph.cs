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
		public bool showRotation = false;
		public bool isCircle = false;
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
										if (isCircle) {
												Gizmos.color = Color.blue;
												Gizmos.DrawLine (myNodes [tempLastCubeIndex].position, myNodes [i].position);
										}
										if (showRotation == true) {
												Gizmos.color = Color.green;
												Vector3 direction = transform.TransformDirection (-myNodes [i].right) * 5f;
												Gizmos.DrawRay (myNodes [i].position, direction);
										}
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
										if (isCircle) {
												Gizmos.color = Color.blue;
												Gizmos.DrawLine (myNodes [tempLastCubeIndex].position, myNodes [i].position);
										}
										if (showRotation == true) {
												Gizmos.color = Color.green;
												Vector3 direction = transform.TransformDirection (-myNodes [i].right) * 5f;
												Gizmos.DrawRay (myNodes [i].position, direction);
										}
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
						for (int i = 0; i < myNodes.Count; i++) {
								int tempInt = i + 1;
								if (tempInt < 0)
										tempInt = (myNodes.Count - 1);
								if (tempInt > myNodes.Count - 1)
										tempInt = 0;
								//transform.right = (pointYouAreLookingAt - transform.position).normalized;
								myNodes [i].right = -(myNodes [tempInt].position - myNodes [i].position).normalized;
						}
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
		
		public List<Transform> getPath (Transform start, Transform end)
		{
				//test to see if we can't go from point A to Point B
				if (Physics.Linecast (start.position, end.position, 9)) {
						//if true we need to debug out that we can;t 
						//then we need to path there
						Debug.DrawLine (start.position, end.position, Color.red, 0.25f);
						//This Code is here Temporaialy ___________________________________
						//in this situation there is only 1 point to go to so make a new list
						List<Transform> tempList = new List<Transform> ();
						//add that point to the list to go torwards
						tempList.Add (end);
						//then we want to return the list
						return tempList;
				} else {
						//if false we need to debug out that we can
						//then we can go staight there
						Debug.DrawLine (start.position, end.position, Color.blue, 0.25f);
						//in this situation there is only 1 point to go to so make a new list
						List<Transform> tempList = new List<Transform> ();
						//add that point to the list to go torwards
						tempList.Add (end);
						//then we want to return the list
						return tempList;
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
