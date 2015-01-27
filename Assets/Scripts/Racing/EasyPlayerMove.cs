using UnityEngine;
using System.Collections;

public class EasyPlayerMove : MonoBehaviour
{
		public float myEngineTorque = 10f;
		public float myMaxTurnAmount = 50f;
		public float myMinTurnAmount = 5f;
		public float highSpeed = 50f;
		public float currentSpeed;
		public float topSpeed = 10f;
		public float downPressureFactor = 0.5f;
		public float massCenterY = -0.1f;
		public Transform myEngineForceLocation;
		public WheelCollider[] myColliderWheels = new WheelCollider[4]; 				//0LF 1LB 2RF 3RB
		public Transform[] myVisualWheels = new Transform[4]; 							//0LF 1LB 2RF 3RB
		public WheelCollider[] myTurnColl = new WheelCollider[2];						//0 == left && 1 == right
		public Transform[] myTurnWheels = new Transform[2];								//0 == left && 1 == right
		public WheelCollider[] myEngineWheels = new WheelCollider[2];
		private float h;
		private float v;
		private float mySteer;

		void DownwardForce ()
		{
				bool isGrounded = false;
				for (int i = 0; i < myColliderWheels.Length; i++) {
						if (myColliderWheels [i].isGrounded) {
								isGrounded = true;
								break;
						}
				}
				if (isGrounded) {		
						Vector3 downPressure = new Vector3 (0f, 0f, 0f);
						downPressure.y = -Mathf.Pow (rigidbody.velocity.magnitude, 1.2f) * downPressureFactor;
						downPressure.y = Mathf.Max (downPressure.y, -70f);
						rigidbody.AddForce (downPressure, ForceMode.Acceleration);
				}
		}

	Vector3 GetWheelPos (WheelCollider myWheelColl, Transform myWheel)
		{
		RaycastHit hit;
				if (Physics.Raycast (myWheelColl.transform.position, -myWheelColl.transform.up, out hit, myWheelColl.suspensionDistance + myWheelColl.radius)) {
						myWheel.position = hit.point + myWheelColl.transform.up * myWheelColl.radius; 
				} else {
						myWheel.position = myWheelColl.transform.position - (myWheelColl.transform.up * myWheelColl.suspensionDistance);
				}
				return myWheel.position;
		}

		void UpdateVisualWheels ()
		{
				Quaternion newRotation = new Quaternion (transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
				newRotation *= Quaternion.Euler (0f, 0f, mySteer * h); // this add a 90 degrees Y rotation
				for (int i = 0; i < myTurnWheels.Length; i++) {
						myTurnWheels [i].transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, 20f * Time.deltaTime);
				}
				for (int i = 0; i < myTurnColl.Length; i++) {
						myTurnColl [i].steerAngle = (mySteer / 2f) * h;
				}
				for (int i = 0; i < myVisualWheels.Length; i++) {
					myVisualWheels[i].transform.position = GetWheelPos(myColliderWheels[i], myVisualWheels[i]) ;
				}
				
		}
		// Use this for initialization
		void Start ()
		{
				rigidbody.centerOfMass = new Vector3 (0.0f, massCenterY, 0.0f);
		}
		// Update is called once per frame
		void Update ()
		{
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Throttle");
				CalculateSpeed ();
				UpdateVisualWheels ();
		}

		void CalculateSpeed ()
		{
				currentSpeed = 2 * 22 / 7 * myTurnColl [0].radius * myTurnColl [0].rpm * 60 / 1000;
				currentSpeed = Mathf.Round (-currentSpeed);
		}

		void EngineTorque ()
		{
				if (currentSpeed < topSpeed && currentSpeed > -(topSpeed / 2f)) {
						for (int i = 0; i < myEngineWheels.Length; i++) {
								myEngineWheels [i].motorTorque = (myEngineTorque * -v) / myEngineWheels.Length;
						}
				} else {
						for (int i = 0; i < myEngineWheels.Length; i++) {
								myEngineWheels [i].motorTorque = 0f;
						}
				}
		}

		void FixedUpdate ()
		{
				mySteer = Mathf.Lerp (myMaxTurnAmount, myMinTurnAmount, currentSpeed / highSpeed);
				EngineTorque ();
				DownwardForce ();
		}
}
