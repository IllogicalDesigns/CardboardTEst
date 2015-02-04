using UnityEngine;
using System.Collections;

[AddComponentMenu("Car Control/Ai Car Movement")]
public class AiOpponet : MonoBehaviour
{
		public enum driveType
		{
			Angry,
			Provoked,
			Normal
		}
		public driveType myAngerLevel;
		public int AngerBar = 0;
		public float myEngineTorque = 10f;
		public float myMaxTurnAmount = 50f;
		public float myMinTurnAmount = 5f;
		public float highSpeed = 50f;
		public float myCurrentSpeed;
		public float topSpeed = 10f;
		public float downPressureFactor = 0.5f;
		[Tooltip("The WheelColliders in this order : 0 = LF; 1 = LB; 2 = RF; 3 = RB")]
		public WheelCollider[]
				myColliderWheels = new WheelCollider[4]; 				//0LF 1LB 2RF 3RB
		[Tooltip("The WheelMesh in this order : 0 = LF; 1 = LB; 2 = RF; 3 = RB")]
		public Transform[]
				myVisualWheels = new Transform[4]; 							//0LF 1LB 2RF 3RB
		[Tooltip("The WheelColliders you wish to turn.  The order does not matter.")]
		public WheelCollider[]
				myTurnColl = new WheelCollider[2];						//0 == left && 1 == right
		[Tooltip("The WheelMeshes that you wish to turn. The order must match Turning Colliders.")]
		public Transform[]
				myTurnWheels = new Transform[2];								//0 == left && 1 == right
		[Tooltip("The WheelColliders which will have power applied to them.  The order does not matter.")]
		public WheelCollider[]
				myEngineWheels = new WheelCollider[2];
		private float h;
		private float v;
		private float mySteer;
		private AudioSource myEngine;

		// Use this for initialization
		void Start ()
		{
				myEngine = gameObject.GetComponent<AudioSource> ();
				myEngine.pitch = 1f;
				myEngine.Play ();
				myAngerLevel = driveType.Normal;
		}

		// Update is called once per frame
		void Update ()
		{
				CalculateSpeed ();
				UpdateVisualWheels ();	
				AdjustVolumePitch ();
		}
		
		void FixedUpdate ()
		{
				mySteer = Mathf.Lerp (myMaxTurnAmount, myMinTurnAmount, myCurrentSpeed / highSpeed);
				EngineTorque ();
				DownwardForce ();
		}

		void CalculateSpeed ()
		{
				myCurrentSpeed = 2 * 22 / 7 * myTurnColl [0].radius * myTurnColl [0].rpm * 60 / 1000;
				myCurrentSpeed = Mathf.Round (-myCurrentSpeed);
		}

		void CalculateAiWheelSteering (Vector3 Target2Turn2)
		{
			//here we will be turning the turn wheels torward our target
			//we will have in normal the regular driving stuff
			//in provoked we will have driving and retaliations
			//in Angry we will have drving and attacks at any chance
		}

		void UpdateVisualWheels ()
		{
				for (int i = 0; i < myTurnColl.Length; i++) {
						myTurnColl [i].steerAngle = (mySteer / 2f) * h;
				}
				for (int i = 0; i < myVisualWheels.Length; i++) {
						myVisualWheels [i].position = GetWheelPos (myColliderWheels [i], myVisualWheels [i]);
						myVisualWheels [i].rotation = myColliderWheels [i].transform.rotation;
						myVisualWheels [i].RotateAround (myVisualWheels [i].transform.position, myVisualWheels [i].transform.right, 20 * Time.deltaTime);
				}
				for (int i = 0; i < myTurnWheels.Length; i++) {
						myTurnWheels [i].localEulerAngles = new Vector3 (myTurnWheels [i].localEulerAngles.x, 90f + (mySteer * h), myTurnWheels [i].localEulerAngles.z);
				}
		}

		void AdjustVolumePitch ()
		{
		
				float tmpFloat = Mathf.Abs (myCurrentSpeed);
				myEngine.pitch = 1f + tmpFloat * 0.025f;
				if (myCurrentSpeed > 40f)
						myEngine.pitch = 1.15f + tmpFloat * 0.015f;
				if (myCurrentSpeed > 60f)
						myEngine.pitch = 1.25f + tmpFloat * 0.013f;
				if (myCurrentSpeed > 80f)
						myEngine.pitch = 1.5f + tmpFloat * 0.011f;
				if (myEngine.pitch < 1f)
						myEngine.pitch = 1f;
				if (myEngine.pitch > 2.3f)
						myEngine.pitch = 2.3f;
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

		void EngineTorque ()
		{
				if (myCurrentSpeed < topSpeed && myCurrentSpeed > -(topSpeed / 2f)) {
						for (int i = 0; i < myEngineWheels.Length; i++) {
								myEngineWheels [i].motorTorque = (myEngineTorque * -v) / myEngineWheels.Length;
						}
				} else {
						for (int i = 0; i < myEngineWheels.Length; i++) {
								myEngineWheels [i].motorTorque = 0f;
						}
				}
		}

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
}
