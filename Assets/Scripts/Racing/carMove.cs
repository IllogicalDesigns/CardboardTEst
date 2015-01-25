using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class carMove : MonoBehaviour
{
		public enum typeOfDrive
		{
				FrontWheelDrive,
				RearWheelDrive,
				AllWheelDrive
		}
		public typeOfDrive myDrive = typeOfDrive.FrontWheelDrive;			//our drive type
		public float carMass;												//how heavy is our car
		private Transform forceLocation;									//our median point to simulate engine force {RED}
		public Transform gripCenter;										//our grip location
		public Transform centerOfGravity;									//how far do we force the cog down {BLUE}
		public Transform turnPoint;											//our turn point to add force to
		public float enginePower = 1500f;									//our engines power and throttle
		public float gripPower = 50f;										//our grip
		public float slip = 1f;												//slip to our grip
		public float turnForce = 5f;										//turn the car
		public float mySpeed;												//speed read out
		public float maxSpeed = 100f;										//our maxiumum speed
		public bool customCOG = false;										//allows us to overide unity
		private Transform carTransform;										//our transform
		private Rigidbody carRidgidbody;									//our ridgidbody
		public Collider[] myWheelColliders = new Collider[4]; 				//0RF 1LF 2RB 3LB
		private Transform[] myVisualWheels = new Transform[4]; 				//0RF 1LF 2RB 3LB
		private float originalEnginePower;
		private float originalGripForce;
		public bool onGround = false;
		float h;
		float v;



		// Use this for initialization
		void Start ()
		{
				originalEnginePower = enginePower;
				FindWheelsAndForces ();
				updateForceLocation (true);
				CachingFun ();
		}

		void OnDrawGizmosSelected ()
		{
				Color color;
				color = Color.green;
				if (forceLocation != null)
						DrawHelperAtCenter (forceLocation.position, this.transform.up, color, v * mySpeed);
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere (centerOfGravity.position, 0.1f);
				color = Color.magenta;
				if (gripCenter != null)
						DrawHelperAtCenter (gripCenter.position, this.transform.right, color, 1f);

		}

		private void DrawHelperAtCenter (Vector3 myPos, Vector3 direction, Color color, float scale)
		{
				Gizmos.color = color;
				Vector3 destination = myPos + direction * scale;
				Gizmos.DrawLine (myPos, destination);
		}

		void CachingFun ()
		{
				//cache this car's transform
				carTransform = transform;
				//cache this car's ridgidbody
				carRidgidbody = rigidbody;
				//set up this car's mass
				if (carMass != 0f)
						//set it to our custom if we have one
						carRidgidbody.mass = carMass;
				else
						//set it to unity's value if we don't
						carMass = carRidgidbody.mass;
				//to prevent the car from fliping we need to set the car's center of gravity lower
				if (customCOG)
						carRidgidbody.centerOfMass = centerOfGravity.position;
				else
						centerOfGravity.position = carRidgidbody.centerOfMass;
				;
		}

		void FindWheelsAndForces ()
		{
				foreach (Transform child in transform) {
						if (child.gameObject.tag == "EngineForce")
								forceLocation = child.gameObject.transform;
						if (child.gameObject.tag == "CenterOfGravity")
								centerOfGravity = child.gameObject.transform;
						if (child.gameObject.tag == "GripForce")
								gripCenter = child.gameObject.transform;
						if (child.gameObject.tag == "TurnPoint")
								turnPoint = child.gameObject.transform;
						if (child.gameObject.tag == "VisualWheel") {																		//our these the wheels we are looking for
								if (child.gameObject.name.ToString ().Contains ("f") || child.gameObject.name.ToString ().Contains ("F")) {	//our we a front wheel
										if (child.gameObject.name.Contains ("r") || child.gameObject.name.ToString ().Contains ("R")) {		//our we the right wheel
												myVisualWheels [0] = child.gameObject.transform;
										} else {																							//else
												myVisualWheels [1] = child.gameObject.transform;
										}
								} else {
										if (child.gameObject.name.Contains ("r") || child.gameObject.name.ToString ().Contains ("R")) {		//our we the right wheel
												myVisualWheels [2] = child.gameObject.transform;
										} else {																							//else
												myVisualWheels [3] = child.gameObject.transform;
										}
								}
						}
		
						if (child.gameObject.tag == "Wheel") {																				//our these the wheels we are looking for
								if (child.gameObject.name.ToString ().Contains ("f") || child.gameObject.name.ToString ().Contains ("F")) {	//our we a front wheel
										if (child.gameObject.name.Contains ("r") || child.gameObject.name.ToString ().Contains ("R")) {		//our we the right wheel
												myWheelColliders [0] = child.gameObject.collider;
										} else {																							//else
												myWheelColliders [1] = child.gameObject.collider;
										}
								} else {
										if (child.gameObject.name.Contains ("r") || child.gameObject.name.ToString ().Contains ("R")) {		//our we the right wheel
												myWheelColliders [2] = child.gameObject.collider;
										} else {																							//else
												myWheelColliders [3] = child.gameObject.collider;
										}
								}
						}
				}
				if (myWheelColliders [0] == null || myWheelColliders [1] == null || myWheelColliders [2] == null || myWheelColliders [3] == null)
						Debug.LogError ("we can't Find all the wheelColliders for " + transform.parent.name);
				if (myVisualWheels [0] == null || myVisualWheels [1] == null || myVisualWheels [2] == null || myVisualWheels [3] == null)
						Debug.LogError ("we can't Find all the visualWheels for " + transform.parent.name);
				if (forceLocation == null)
						Debug.LogError ("our Engine Force Point came up null " + transform.parent.name);
				if (centerOfGravity == null)
						Debug.LogError ("our C.O.G. is null +" + transform.parent.name);
		}

		Vector3 findCenterOfPoints (List<Vector3> vecs2Test)
		{
				float newX = 0f;
				float newY = 0f;
				float newZ = 0f;
				int amountTested = 0;
				foreach (Vector3 vecX in vecs2Test) {
						newX = newX + vecX.x;
						amountTested ++;
				}
				foreach (Vector3 vecY in vecs2Test) {
						newY = newY + vecY.y;
				}
				foreach (Vector3 vecZ in vecs2Test) {
						newZ = newZ + vecZ.z;
				}
				return new Vector3 (newX / amountTested, newY / amountTested, newZ / amountTested);
		}

		void updateForceLocation (bool checkEverthing)		//change to update both engineForce and gripCenter
		{
				enginePower = originalEnginePower;
				List<Vector3> ForceAliveWheels = new List<Vector3> ();
				List<Vector3> allAliveWheels = new List<Vector3> ();
				List<Vector3> frontAliveWheels = new List<Vector3> ();
				if (myDrive == typeOfDrive.FrontWheelDrive) {																//FrontWheel
						if (myWheelColliders [0].collider.enabled) {
								ForceAliveWheels.Add (myWheelColliders [0].collider.gameObject.transform.position);			//if active add to alive wheels
						}
						if (myWheelColliders [1].collider.enabled) {															//if active add to alive wheels
								ForceAliveWheels.Add (myWheelColliders [1].collider.gameObject.transform.position);
						}
						if (myWheelColliders [0].collider.enabled == false || myWheelColliders [1].collider.enabled == false)	//if wheel is missing reduce force
								enginePower = enginePower / 2f;
						if (myWheelColliders [0].collider.enabled == false && myWheelColliders [1].collider.enabled == false)	//if no wheels zero force
								enginePower = 0f;
				}																												//FrontWheelEnd
				if (myDrive == typeOfDrive.RearWheelDrive) {																	//RearWheel
						if (myWheelColliders [2].collider.enabled)
								ForceAliveWheels.Add (myWheelColliders [2].collider.gameObject.transform.position);				//if active add to alive wheels
						if (myWheelColliders [3].collider.enabled)
								ForceAliveWheels.Add (myWheelColliders [3].collider.gameObject.transform.position);				//if active add to alive wheels
						if (myWheelColliders [2].collider.enabled == false || myWheelColliders [3].collider.enabled == false)	//if wheel is missing reduce force
								enginePower = enginePower / 2f;
						if (myWheelColliders [2].collider.enabled == false && myWheelColliders [3].collider.enabled == false)	//if no wheels zero force
								enginePower = 0f;
				}																												//RearWheelEnd
				if (myDrive == typeOfDrive.AllWheelDrive) {																		//AllWheel
						if (myWheelColliders [0].collider.enabled) {															//if active add to alive wheels
								ForceAliveWheels.Add (myWheelColliders [0].collider.gameObject.transform.position);
						}
						if (myWheelColliders [1].collider.enabled) {
								ForceAliveWheels.Add (myWheelColliders [1].collider.gameObject.transform.position);			//if active add to alive wheels
						}
						if (myWheelColliders [2].collider.enabled) {
								ForceAliveWheels.Add (myWheelColliders [2].collider.gameObject.transform.position);			//if active add to alive wheels
						}
						if (myWheelColliders [3].collider.enabled) {
								ForceAliveWheels.Add (myWheelColliders [3].collider.gameObject.transform.position);			//if active add to alive wheels
						}
						//Reduce Values if tires our missing
						if (myWheelColliders [0].collider.enabled == false || myWheelColliders [1].collider.enabled == false)	//if wheel is missing reduce force
								enginePower = enginePower - (enginePower / 4f);
						if (myWheelColliders [2].collider.enabled == false || myWheelColliders [3].collider.enabled == false)	//if wheel is missing reduce force
								enginePower = enginePower - (enginePower / 4f);
						if (myWheelColliders [0].collider.enabled == false && myWheelColliders [1].collider.enabled == false) 	//if wheel is missing reduce force
								enginePower = enginePower / 2f;
						if (myWheelColliders [2].collider.enabled == false && myWheelColliders [3].collider.enabled == false) 	//if wheel is missing reduce force
								enginePower = enginePower / 2f;
						if (myWheelColliders [1].collider.enabled == false && myWheelColliders [2].collider.enabled == false) 	//if wheel is missing reduce force
								enginePower = enginePower / 2f;
						if (myWheelColliders [0].collider.enabled == false && myWheelColliders [3].collider.enabled == false) 	//if wheel is missing reduce force
								enginePower = enginePower / 2f;
						if (myWheelColliders [0].collider.enabled == false && myWheelColliders [1].collider.enabled == false && myWheelColliders [2].collider.enabled == false && myWheelColliders [3].collider.enabled == false)	//if no wheels zero force
								enginePower = 0f;
				}																												//AllWheel
				if (myWheelColliders [0].collider.enabled != false) {															//alive wheels for grip and front
						frontAliveWheels.Add (myWheelColliders [0].transform.position);
						allAliveWheels.Add (myWheelColliders [0].transform.position);
				} else {
						gripPower = gripPower - (gripPower / 4f);
				}
				if (myWheelColliders [1].collider.enabled != false) {
						frontAliveWheels.Add (myWheelColliders [1].transform.position);
						allAliveWheels.Add (myWheelColliders [1].transform.position);
				} else {
						gripPower = gripPower - (gripPower / 4f);
				}
				if (myWheelColliders [2].collider.enabled != false) {
						allAliveWheels.Add (myWheelColliders [2].transform.position);
				} else {
						gripPower = gripPower - (gripPower / 4f);
				}
				if (myWheelColliders [3].collider.enabled != false) {
						allAliveWheels.Add (myWheelColliders [3].transform.position);
				} else {
						gripPower = gripPower - (gripPower / 4f);
				}																													//end AllWheelsAlive for grip and Front
		
				forceLocation.position = findCenterOfPoints (ForceAliveWheels);
				if (allAliveWheels.Count != 0)
						gripCenter.position = findCenterOfPoints (allAliveWheels);
				if (frontAliveWheels.Count != 0)
						turnPoint.position = findCenterOfPoints (frontAliveWheels);
		}

		private Vector3  rotationAmount;
	
		void rotateVisualWheels ()
		{
				//Front wheels roation based on steering
				float newY0 = h * 30f * transform.localPosition.x;
				myVisualWheels [0].transform.localEulerAngles = new Vector3 (myVisualWheels [0].transform.localEulerAngles.x, newY0, myVisualWheels [0].transform.localEulerAngles.z);
				myVisualWheels [1].transform.localEulerAngles = new Vector3 (myVisualWheels [1].transform.localEulerAngles.x, newY0, myVisualWheels [1].transform.localEulerAngles.z);
		
				rotationAmount = transform.right * (mySpeed * 1.6f * Time.deltaTime * Mathf.Rad2Deg);
		
				myVisualWheels [0].Rotate (rotationAmount);
				myVisualWheels [1].Rotate (rotationAmount);
				myVisualWheels [2].Rotate (rotationAmount);
				myVisualWheels [3].Rotate (rotationAmount);
		}
	
		void carPhysicsUpdate ()
		{
				/*	//grab all the physics info we need to calculate everything
				Vector3 myRight = carTransform.right;
				//we find our velocity
				Vector3 velo = carRidgidbody.velocity;
				Vector3 tmpVec = new Vector3 (velo.x, 0, velo.z);
				//our velocity with our y movement
				Vector3 flatVelo = tmpVec;
				//find direction we our moving in
				Vector3 dir = transform.TransformDirection (carForward);
				tmpVec = new Vector3 (dir.x, 0, dir.z);
				//figure out our velocity without y movement - our flat velocity
				Vector3 flatDir = Vector3.Normalize (tmpVec);
				//calculate engine force with flat vector and acceration
				engineForce = (flatDir * (enginePower) * carMass);	*/
		}

		void updateEngineSound ()
		{
				audio.pitch = 0.30f + mySpeed * 0.025f;
				if (mySpeed > 30)
						audio.pitch = 0.25f + mySpeed * 0.015f;
				if (mySpeed > 40)
						audio.pitch = 0.20f + mySpeed * 0.013f;
				if (mySpeed > 49)
						audio.pitch = 0.15f + mySpeed * 0.011f;
				//dont let the pitch get too high then reset it
				if (audio.pitch > 2.0f)
						audio.pitch = 2f;
		}
		// Update is called once per frame
		void Update ()
		{
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Throttle");
				//updateEngineForceLocation ();
				mySpeed = carRidgidbody.velocity.magnitude;
				rotateVisualWheels ();
				Debug.DrawRay (transform.position, -transform.forward, Color.red);
				RaycastHit hit;
				if (Physics.Raycast (centerOfGravity.position, -this.transform.forward, out hit, 0.5f)) {
						if (hit.collider.tag == "Ground")
								onGround = true;
				} else {
						onGround = false;
				}
				if (Input.GetKeyDown (KeyCode.LeftControl))
						updateForceLocation (true);

		}

		void FixedUpdate ()
		{
				//carRidgidbody.AddForceAtPosition (centerOfGravity.position, 50f * -this.transform.up);
				if (!onGround) {
						if (mySpeed > 0.1f)
								carRidgidbody.AddForceAtPosition (this.transform.right * turnForce * (mySpeed * 0.1f) * -h, turnPoint.position);
						if (v > 0.1f && mySpeed < maxSpeed || v < -0.1f && mySpeed < maxSpeed) {
								//carRidgidbody.AddForceAtPosition (Vector3.forward * enginePower * v, forceLocation);
								//Vector3 worldForcePosition = transform.TransformPoint (this.transform.up);
								carRidgidbody.AddForceAtPosition (this.transform.up * enginePower * v, forceLocation.position);
						}
				}
		}
}
