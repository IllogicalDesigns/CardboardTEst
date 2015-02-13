using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("Car Control/Player Car Movement")]
public class PlayerMove : MonoBehaviour
{
		public enum controlType
		{
				KeyboardGamepad,
				VirtualReality,
				Mobile
		}
		public static float sensetivity = 2f;
		public static controlType myControlType;
		public float myEngineTorque = 10f;
		public int reverseBrakeAmount = 40;
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
		public AiNodeGraph myNodeGraph;
		float count = 5f;
		bool isGrounded = false;
	
		void DownwardForce ()
		{
				isGrounded = false;
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
				for (int i = 0; i < myTurnColl.Length; i++) {
						myTurnColl [i].steerAngle = (mySteer / 2f) * h;
				}
				for (int i = 0; i < myVisualWheels.Length; i++) {
						myVisualWheels [i].position = GetWheelPos (myColliderWheels [i], myVisualWheels [i]);
						myVisualWheels [i].rotation = myColliderWheels [i].transform.rotation;
						//float myWheelRot = myVisualWheels[i].localRotation.y * myColliderWheels[i].rpm;
						//myVisualWheels[i].localEulerAngles = new Vector3 (myWheelRot, myVisualWheels[i].localEulerAngles.y, myVisualWheels[i].localEulerAngles.z);
						//myVisualWheels [i].Rotate (Vector3.right * Time.deltaTime, Space.Self);
						myVisualWheels [i].RotateAround (myVisualWheels [i].transform.position, myVisualWheels [i].transform.right, 20 * Time.deltaTime);
				}
				for (int i = 0; i < myTurnWheels.Length; i++) {
						myTurnWheels [i].localEulerAngles = new Vector3 (myTurnWheels [i].localEulerAngles.x, 90f + (mySteer * h), myTurnWheels [i].localEulerAngles.z);
				}
		}

		void MobileControls ()
		{
				Vector3 myAccelerometerData = Input.acceleration;
				//Debug.Log (myAccelerometerData);
				h = myAccelerometerData.x * sensetivity;
				h = Mathf.Clamp (h, -1f, 1f);
				if (Input.touchCount > 0) {
						Touch touch = Input.GetTouch (0);
						if (touch.position.x < Screen.width / 2) {
								v = -1;
						} else if (touch.position.x > Screen.width / 2) {
								v = 1;
						}
				} else 
						v = 0;
		}

		//void SetWheelFriction (string typeOfMaterial)	//set Friction values here
		//{
		//if (typeOfMaterial == "Road") {
		//for (int i = 0; i < myColliderWheels.Length; i++) {
		//myColliderWheels [i].sidewaysFriction.stiffness = 0.05f;
		//myColliderWheels [i].forwardFriction.stiffness = 1f;
		//}
		//}
		//}
		// Use this for initialization
		void Start ()
		{
				myEngine = gameObject.GetComponent<AudioSource> ();
				myEngine.pitch = 1f;
				myEngine.Play ();
				sensetivity = GuiMainMeun.mySensitivity;
				//SetWheelFriction ("Road");
		}
		// Update is called once per frame
		void Update ()
		{
				if (myControlType == controlType.VirtualReality) {
						Application.LoadLevel ("MainMenu");
						Debug.LogError ("VR Controls do not work");
				}
				if (myControlType == controlType.KeyboardGamepad) {
						h = Input.GetAxis ("Horizontal");
						v = Input.GetAxis ("Throttle");
				}
				if (myControlType == controlType.Mobile) {
						MobileControls ();
				}
				CalculateSpeed ();
				UpdateVisualWheels ();	
				AdjustVolumePitch ();
				if (Input.GetKeyDown (KeyCode.Space))
						ResetCar ();
				if (count < 0)
						ResetCar ();
				if (!isGrounded)
						count -= Time.deltaTime;
				if (count != 10f && isGrounded)
						count = 10f;
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

		void CalculateSpeed ()
		{
				myCurrentSpeed = 2 * 22 / 7 * myTurnColl [0].radius * myTurnColl [0].rpm * 60 / 1000;
				myCurrentSpeed = Mathf.Round (-myCurrentSpeed);
		}

		void EngineTorque ()
		{
				int revBrake = 0;
				if (v < -0.1 && myCurrentSpeed > 0.5f)
						revBrake = reverseBrakeAmount;
				else
						revBrake = 0;
				if (myCurrentSpeed < topSpeed && myCurrentSpeed > -(topSpeed / 2f)) {
						for (int i = 0; i < myEngineWheels.Length; i++) {
								myEngineWheels [i].motorTorque = ((myEngineTorque + revBrake) * -v) / myEngineWheels.Length;
						}
				} else {
						for (int i = 0; i < myEngineWheels.Length; i++) {
								myEngineWheels [i].motorTorque = 0f;
						}
				}
		}

		public void ResetCar ()
		{
				count = 5f;
				rigidbody.velocity = Vector3.zero;
				foreach (WheelCollider wheelCol in myColliderWheels) {
						wheelCol.rigidbody.velocity = Vector3.zero;
				}
				Transform tempTrans = myNodeGraph.GetClosestWaypoint (transform.position);
						LayerMask mask = -9;
						RaycastHit hit;
						if (!Physics.SphereCast (tempTrans.position, myNodeGraph.detectionRange, transform.forward, out hit, 10, mask)) {
								transform.rotation = tempTrans.rotation;
								transform.position = tempTrans.position;
								count = 5f;
						} else {
				transform.rotation = tempTrans.rotation;
				transform.position = tempTrans.position + Vector3.up * 5f;
				count = 5f;
			}
		
		}
	
		void FixedUpdate ()
		{
				mySteer = Mathf.Lerp (myMaxTurnAmount, myMinTurnAmount, myCurrentSpeed / highSpeed);
				EngineTorque ();
				DownwardForce ();
		}
}
