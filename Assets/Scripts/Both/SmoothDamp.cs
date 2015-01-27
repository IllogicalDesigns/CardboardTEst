using UnityEngine;
using System.Collections;

public class SmoothDamp : MonoBehaviour
{
		public Transform target;
		public Vector3 newFollowPoint;
		public float smoothTime = 0.3F;
		private Vector3 velocity = Vector3.zero;
		public bool lookAtTarget = true;
		// Update is called once per frame
		void FixedUpdate ()
		{
				if (lookAtTarget) {
						transform.LookAt (target.position);
				}
				Vector3 targetPosition = target.TransformPoint (newFollowPoint);
				transform.position = Vector3.SmoothDamp (transform.position, targetPosition, ref velocity, smoothTime);
		}
}
