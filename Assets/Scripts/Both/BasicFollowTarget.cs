using UnityEngine;
using System.Collections;

public class BasicFollowTarget : MonoBehaviour {
	NavMeshAgent myNavAgent;
	public Transform target;

	// Use this for initialization
	void Start () {
		myNavAgent = gameObject.GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		myNavAgent.SetDestination (target.position);
	}
}
