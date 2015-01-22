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
		//public Vector3 forceLocation;										//our median point to simulate engine force {RED}
		private Transform forceLocation;
		private Transform gripLocation;										//our grip location
		private Transform centerOfGravity;									//how far do we force the cog down {BLUE}
		public float enginePower = 1500f;									//our engines power and throttle
		public float mySpeed;												//speed read out
		public float maxSpeed = 100f;										//our maxiumum speed
		public bool customCOG = false;
		private Transform carTransform;										//our transform
		private Rigidbody carRidgidbody;									//our ridgidbody
		private Vector3 carUp;												//our up			
		private Vector3 carRight;											//our right
		private Vector3 carForward;											//our forward
		public Collider[] myWheelColliders = new Collider[4]; 				//0RF 1LF 2RB 3LB
		private Transform[] myVisualWheels = new Transform[4]; 				//0RF 1LF 2RB 3LB
		private Vector3 gripImpulseForce;									//our Grip Location changing as wheels are destroyed
		private Vector3 engineForce;										//engine force vector
		private float originalEnginePower;
		float h;
		float v;



		// Use this for initialization
		void Start ()
		{
				originalEnginePower = enginePower;
				FindWheelsAndForces ();
				updateForceLocation ();
				CachingFun ();
		}

		void OnDrawGizmosSelected ()
		{
				Color color;
				color = Color.green;
				if (forceLocation != null)
						DrawHelperAtCenter (forceLocation.position, this.transform.up, color, v * mySpeed);

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
				//cache this car's vectors
				carUp = carTransform.up;
				carRight = carTransform.right;
				carForward = carTransform.forward;
		}

		void FindWheelsAndForces ()
		{
				foreach (Transform child in transform) {
						if (child.gameObject.tag == "EngineForce")
								forceLocation = child.gameObject.transform;
						if (child.gameObject.tag == "CenterOfGravity")
								centerOfGravity = child.gameObject.transform;
						if (child.gameObject.tag == "GripForce")
								gripLocation = child.gameObject.transform;
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

		void updateForceLocation ()		//change to update both engineForce and gripLocation
		{
				enginePower = originalEnginePower;
				List<Vector3> currentlyAliveWheels = new List<Vector3> ();
				if (myDrive == typeOfDrive.FrontWheelDrive) {																	//FrontWheel
						if (myWheelColliders [0].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [0].collider.gameObject.transform.position);			//if active add to alive wheels
						if (myWheelColliders [1].collider.enabled)																//if active add to alive wheels
								currentlyAliveWheels.Add (myWheelColliders [1].collider.gameObject.transform.position);
						if (myWheelColliders [0].collider.enabled == false || myWheelColliders [1].collider.enabled == false)	//if wheel is missing reduce force
								enginePower = enginePower / 2f;
						if (myWheelColliders [0].collider.enabled == false && myWheelColliders [1].collider.enabled == false)	//if no wheels zero force
								enginePower = 0f;
				}																												//FrontWheelEnd
				if (myDrive == typeOfDrive.RearWheelDrive) {																	//RearWheel
						if (myWheelColliders [2].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [2].collider.gameObject.transform.position);			//if active add to alive wheels
						if (myWheelColliders [3].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [3].collider.gameObject.transform.position);			//if active add to alive wheels
						if (myWheelColliders [2].collider.enabled == false || myWheelColliders [3].collider.enabled == false)	//if wheel is missing reduce force
								enginePower = enginePower / 2f;
						if (myWheelColliders [2].collider.enabled == false && myWheelColliders [3].collider.enabled == false)	//if no wheels zero force
								enginePower = 0f;
				}																												//RearWheelEnd
				if (myDrive == typeOfDrive.AllWheelDrive) {																		//AllWheel
						if (myWheelColliders [0].collider.enabled)																//if active add to alive wheels
								currentlyAliveWheels.Add (myWheelColliders [0].collider.gameObject.transform.position);
						if (myWheelColliders [1].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [1].collider.gameObject.transform.position);			//if active add to alive wheels
						if (myWheelColliders [2].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [2].collider.gameObject.transform.position);			//if active add to alive wheels
						if (myWheelColliders [3].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [3].collider.gameObject.transform.position);			//if active add to alive wheels
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
		
				forceLocation.position = findCenterOfPoints (currentlyAliveWheels);
		}

		private Vector3  rotationAmount;
	
		void rotateVisualWheels ()
		{
				//Front wheels roation based on steering
				//leftFrontWheel.localEulerAngles.y = h * 30f;
				//rightFrontWheel.localEulerAngles.y = h * 30f;
		
				rotationAmount = carRight * (mySpeed * 1.6f * Time.deltaTime * Mathf.Rad2Deg);
		
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
		// Update is called once per frame
		void Update ()
		{
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Throttle");
				//updateEngineForceLocation ();
				mySpeed = carRidgidbody.velocity.magnitude;
				rotateVisualWheels ();
				if (Input.GetKeyDown (KeyCode.LeftControl))
						updateForceLocation ();

		}

		void FixedUpdate ()
		{
				if (v > 0.1f && mySpeed < maxSpeed || v < -0.1f && mySpeed < maxSpeed) {
						//carRidgidbody.AddForceAtPosition (Vector3.forward * enginePower * v, forceLocation);
						//Vector3 worldForcePosition = transform.TransformPoint (this.transform.up);
						carRidgidbody.AddForceAtPosition (this.transform.up * enginePower * v, forceLocation.position);
				}
		}
}
