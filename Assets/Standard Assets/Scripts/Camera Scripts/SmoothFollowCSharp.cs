using UnityEngine; 
using System.Collections;

public class SmoothFollowCSharp : MonoBehaviour {
	public Transform target;
	public float dampening = 3.0f;

	void Update () {
		Quaternion quat;
		transform.Translate(target.position);
		quat = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * dampening);
		transform.rotation = quat;
	}
}