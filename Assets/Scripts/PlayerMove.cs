using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
		public GameObject playerHead;
		public Transform tarV;
		public Transform tarH;
		public int speed = 5;
		public float headHeight = 1.5f;
		private float h;
		private float v;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Vertical");
		}

		void FixedUpdate ()
		{
				//Sets our rotation to  our head
				//transform.rotation = new Quaternion (transform.rotation.x, playerHead.transform.position.y, transform.rotation.z, playerHead.transform.rotation.w);
				Quaternion rot = new Quaternion (transform.rotation.x, Cardboard.SDK.HeadRotation.y, transform.rotation.z, Cardboard.SDK.HeadRotation.w);
				transform.rotation = rot;

				playerHead.transform.position = new Vector3 (transform.position.x, transform.position.y + headHeight, transform.position.z); 
				
				//mkes to places to move towards
				//Vector3 hVec3 = new Vector3 (transform.position.x + 1f, transform.position.y, transform.position.z);
				//Vector3 vVec3 = new Vector3 (transform.position.x, transform.position.y, transform.position.z + 1f);
				
				//does the move torwards
				float step = speed * Time.deltaTime * h;
				transform.position = Vector3.MoveTowards (transform.position, tarH.position, step);
				float step2 = speed * Time.deltaTime * v;
				transform.position = Vector3.MoveTowards (transform.position, tarV.position, step2);
		}
}
