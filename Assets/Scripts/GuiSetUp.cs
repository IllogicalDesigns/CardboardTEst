using UnityEngine;
using System.Collections;

public class GuiSetUp : MonoBehaviour
{
		public Canvas masterCanvas;
		public Camera leftEye;
		public Camera rightEye;

		// Use this for initialization
		void Start ()
		{
				Canvas leftCanvas;
				leftCanvas = Instantiate (masterCanvas, transform.position, Quaternion.identity) as Canvas;
				Canvas rightCanvas;
				rightCanvas = Instantiate (masterCanvas, transform.position, Quaternion.identity) as Canvas;
				//set pixel perfect is it is
				if (masterCanvas.pixelPerfect == true) {
						leftCanvas.pixelPerfect = true;
						rightCanvas.pixelPerfect = true;
				} else {
						leftCanvas.pixelPerfect = false;
						rightCanvas.pixelPerfect = false;
				}
				//set plane distance to that of master Canvas
				float planeDis = masterCanvas.planeDistance;
				leftCanvas.planeDistance = planeDis;
				rightCanvas.planeDistance = planeDis;
				//set our redering mode
				leftCanvas.renderMode = RenderMode.ScreenSpaceCamera;
				rightCanvas.renderMode = RenderMode.ScreenSpaceCamera;
				//set our canvases to the right eyes
				leftCanvas.worldCamera = leftEye;
				rightCanvas.worldCamera = rightEye;
				//set our Master to not render
				masterCanvas.gameObject.SetActive (false);
		}
}
