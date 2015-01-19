using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class carMove : MonoBehaviour
{
		public bool frontDrive = true;
		public float acceleration = 5f;
		public float maxSpeed = 150f;
		public float currentSpeed;
		public Vector3 yourLocalForcePosition;
		public List<GameObject> wheels = new List<GameObject> ();
		public Rigidbody backAxel;
		public GameObject RFWheel;
		public GameObject LFWheel;
		GameObject myParent;
		float h;
		float v;

		// Use this for initialization
		void Start ()
		{
				myParent = gameObject.transform.parent.gameObject; 																				//Find my parent
				foreach (Transform child in myParent.transform) {																				//get my parents children
						if (child.gameObject.tag == "Axel") {																					//if they are an axel
								if (child.gameObject.name.ToString ().Contains ("Back") || child.gameObject.name.ToString ().Contains ("back"))	//our they the back axel must be called back something
										backAxel = child.gameObject.rigidbody;																	//add back axel reference
								foreach (Transform dChild in child.transform) {																	//find the axels children
										if (dChild.gameObject.tag == "Wheel") {																	//our these the wheels we are looking for
												wheels.Add (dChild.gameObject);																	//make reference to them in wheels
												if (dChild.gameObject.name.ToString ().Contains ("f") || dChild.gameObject.name.ToString ().Contains ("F")) {	//our we a front wheel
														if (dChild.gameObject.name.Contains ("r") || dChild.gameObject.name.ToString ().Contains ("R")) {		//our we the right wheel
																RFWheel = dChild.gameObject;													//then we our the front right wheel
														} else {																				//else
																LFWheel = dChild.gameObject;													//then we our the front left wheel
														}
												}
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
