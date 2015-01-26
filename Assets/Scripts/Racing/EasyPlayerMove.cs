using UnityEngine;
using System.Collections;

public class EasyPlayerMove : MonoBehaviour
{
		public float myEngineTorque = 10f;
		public float myTurnAmount = 30f;
		public Transform myEngineForceLocation;
		public WheelCollider[] myColliderWheels = new WheelCollider[4]; 				//0LF 1LB 2RF 3RB
		public Transform[] myVisualWheels = new Transform[4]; 				//0LF 1LB 2RF 3RB
		public int frtRhtTire = 0;
		public int frtLeftTire = 1;
		private float h;
		private float v;

		void rotateVisualWheels ()
		{
				Quaternion newRotation = new Quaternion (transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
				newRotation *= Quaternion.Euler (0f, 0f, myTurnAmount * h); // this add a 90 degrees Y rotation
				myVisualWheels [frtRhtTire].transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, 20f * Time.deltaTime);
				myVisualWheels [frtLeftTire].transform.rotation = Quaternion.Slerp (transform.rotation, newRotation, 20f * Time.deltaTime); 
				myColliderWheels [frtRhtTire].steerAngle = (myTurnAmount / 2f) * h;
				myColliderWheels [frtLeftTire].steerAngle = (myTurnAmount / 2f) * h;
				//Vector3 rotationAmount = transform.right * (rigidbody.velocity.magnitude * 1.6f * Time.deltaTime * Mathf.Rad2Deg);
				//myVisualWheels [0].Rotate (rotationAmount);
				//myVisualWheels [1].Rotate (rotationAmount);
				//myVisualWheels [2].Rotate (rotationAmount);
				//myVisualWheels [3].Rotate (rotationAmount);
		}
		// Use this for initialization
		void Start ()
		{
				rigidbody.centerOfMass += new Vector3 (0f, -1f, 1f);
		}
		// Update is called once per frame
		void Update ()
		{
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Throttle");
		}

		void FixedUpdate ()
		{
				rotateVisualWheels ();
				myColliderWheels [frtRhtTire].motorTorque = myEngineTorque * -v;
				myColliderWheels [frtLeftTire].motorTorque = myEngineTorque * -v;
		}
}
