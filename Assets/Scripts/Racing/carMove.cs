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
		public Vector3 forceLocation;										//our median point to simulate engine force {RED}
		public Vector3 gripLocation;										//our grip location
		public Vector3 centerOfGravity = new Vector3 (0f, -0.7f, 0.35f);	//how far do we force the cog down {BLUE}
		public float enginePower = 1500f;									//our engines power and throttle
		public float mySpeed;												//speed read out
		public float maxSpeed = 100f;										//our maxiumum speed

		private Transform carTransform;										//our transform
		private Rigidbody carRidgidbody;									//our ridgidbody
		private Vector3 carUp;												//our up			
		private Vector3 carRight;											//our right
		private Vector3 carForward;											//our forward
		public Collider[] myWheelColliders = new Collider[4]; 				//0RF 1LF 2RB 3LB
		public Transform[] myVisualWheels = new Transform[4]; 				//0RF 1LF 2RB 3LB
		private Vector3 gripImpulseForce;									//our Grip Location changing as wheels are destroyed
		private Vector3 engineForce;										//engine force vector
	
		float h;
		float v;

		// Use this for initialization
		void Start ()
		{
				updateEngineForceLocation ();
				CachingFun ();
				FindAndSetUpWheels ();
	}

		void OnDrawGizmosSelected ()
		{
				Gizmos.color = Color.blue;
				Gizmos.DrawSphere (centerOfGravity, 0.1f);
				Gizmos.color = Color.red;
				Gizmos.DrawSphere (forceLocation, 0.1f);
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
				if (centerOfGravity != Vector3.zero)
						//set it to our custom if we have one
						carRidgidbody.centerOfMass = centerOfGravity;
				else
						//set it to unity's value if we don't
						centerOfGravity = carRidgidbody.centerOfMass;
				//cache this car's vectors
				carUp = carTransform.up;
				carRight = carTransform.right;
				carForward = carTransform.forward;
		}

		void FindAndSetUpWheels ()
		{
		Debug.Log ("test");
				foreach (Transform child in transform) {
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

		void updateEngineForceLocation ()
		{
				List<Vector3> currentlyAliveWheels = new List<Vector3> ();
				if (myDrive == typeOfDrive.FrontWheelDrive) {
						if (myWheelColliders [0].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [0].collider.gameObject.transform.position);
						if (myWheelColliders [1].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [1].collider.gameObject.transform.position);
				}
				if (myDrive == typeOfDrive.RearWheelDrive) {
						if (myWheelColliders [2].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [2].collider.gameObject.transform.position);
						if (myWheelColliders [3].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [3].collider.gameObject.transform.position);
				}
				if (myDrive == typeOfDrive.AllWheelDrive) {
						if (myWheelColliders [0].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [0].collider.gameObject.transform.position);
						if (myWheelColliders [1].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [1].collider.gameObject.transform.position);
						if (myWheelColliders [2].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [2].collider.gameObject.transform.position);
						if (myWheelColliders [3].collider.enabled)
								currentlyAliveWheels.Add (myWheelColliders [3].collider.gameObject.transform.position);
				}
				forceLocation = findCenterOfPoints (currentlyAliveWheels);
		}
	
		void carPhysicsUpdate ()
		{
				//grab all the physics info we need to calculate everything
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
				engineForce = (flatDir * (enginePower) * carMass);	
		}
		// Update is called once per frame
		void Update ()
		{
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Vertical");
				updateEngineForceLocation ();
				carPhysicsUpdate ();
		}

		void FixedUpdate ()
		{
				//if (mySpeed < maxSpeed)
						//carRidgidbody.AddForceAtPosition (gripImpulseForce * Time.deltaTime, forceLocation);	
				//carRidgidbody.AddForce (engineForce * Time.deltaTime);
				//if (mySpeed > maxSpeedToTurn)
				//carRidgidbody.AddTorque (turnVec * Time.deltaTime);
				//else if (mySpeed < maxSpeedToTurn)
				//return;
				//force impulsing for grip
				carRidgidbody.AddForceAtPosition (gripImpulseForce * Time.deltaTime, gripLocation);	
		}
}
