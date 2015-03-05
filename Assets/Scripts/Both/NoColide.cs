using UnityEngine;
using System.Collections;

public class NoColide : MonoBehaviour
{
		//public int layerToMask = 8;
		public Collider[] collidersToNotCollideWith;

		// Use this for initialization
		void Start ()
		{
				//Physics.IgnoreLayerCollision (this.gameObject.layer, layerToMask);
				foreach (Collider col in collidersToNotCollideWith) {
						if (col.enabled)
								Physics.IgnoreCollision (this.GetComponent<Collider>(), col);
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}
}
