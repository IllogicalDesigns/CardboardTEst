using UnityEngine;
using System.Collections;

public class RotateAndMove : MonoBehaviour
{
		// Update is called once per frame
		void Update ()
		{
		float rotX = (Cardboard.SDK.HeadRotation.x - Cardboard.SDK.HeadRotation.x / 2);
		Quaternion rot = new Quaternion (rotX, Cardboard.SDK.HeadRotation.y, Cardboard.SDK.HeadRotation.z, Cardboard.SDK.HeadRotation.w);
						transform.rotation = rot;
		}
}
