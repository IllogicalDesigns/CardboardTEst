using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OcMine : MonoBehaviour
{
		public Renderer[] cullingObjects;
		//var objThatCanBeCulled : Renderer[]; //array of objects that can be culled when the player/camera come to this cell

		void Start ()
		{
				string debugReadout = transform.childCount.ToString ();
				Debug.Log (gameObject.name + " " + debugReadout);
				cullingObjects = new Renderer[transform.childCount - 1];
				int i = 0;
				foreach (Transform child in transform) {
						if (child.gameObject.tag == "CullingObject") {
								//cullingObjects.Add (child.gameObject);
								cullingObjects [i] = child.gameObject.renderer;
								if (cullingObjects [i].gameObject != null)
										cullingObjects [i].enabled = false;
								i++;
						}
				}
		}

		void OnTriggerEnter (Collider other)
		{
				if (other.gameObject.tag == "Player") {
						foreach (Renderer cullObj in cullingObjects) {
								if (cullObj.gameObject != null)
										cullObj.renderer.enabled = true;
						}
				}
		}

		void OnTriggerExit (Collider other)
		{
				if (other.gameObject.tag == "Player") {
						foreach (Renderer cullObj in cullingObjects) {
								if (cullObj.gameObject != null)
										cullObj.enabled = false;
						}
				}
		}
}
