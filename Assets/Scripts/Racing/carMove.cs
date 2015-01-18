using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class carMove : MonoBehaviour
{
		public GameObject frontAxel;
		public GameObject backAxel;
		public bool frontDrive = true;
		public float acceleration = 5f;
		public float maxSpeed = 150f;
		public float currentSpeed;
		public Vector3 yourLocalForcePosition;
		public List<GameObject> wheels = new List<GameObject> ();
		GameObject myParent;
		float h;
		float v;

		// Use this for initialization
		void Start ()
		{
				myParent = gameObject.transform.parent.gameObject;
				List<GameObject> Axels = new List<GameObject> ();
				foreach (Transform child in myParent.transform) {
						if (child.gameObject.tag == "Axel") {
								foreach (Transform dChild in child.transform) {
										if (dChild.gameObject.tag == "Wheel") {
												wheels.Add (dChild.gameObject);
										}
								}
						}
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Vertical");
				Debug.Log (rigidbody.velocity.magnitude);
		}

		void FixedUpdate ()
		{
				if (rigidbody.velocity.sqrMagnitude > maxSpeed * maxSpeed) {
						rigidbody.velocity = Vector3.ClampMagnitude (rigidbody.velocity, maxSpeed);
				}
				if (v != 0 && !(rigidbody.velocity.magnitude > maxSpeed)) {
						Vector3 worldForcePosition = transform.TransformPoint (yourLocalForcePosition);
						rigidbody.AddForceAtPosition (transform.forward * (acceleration * v), worldForcePosition);
				}
		}
}
