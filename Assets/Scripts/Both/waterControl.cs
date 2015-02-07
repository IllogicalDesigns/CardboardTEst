using UnityEngine;
using System.Collections;

public class waterControl : MonoBehaviour {
	public AudioClip  mySplashDownSFX;
	public ParticleSystem mySplashParticle;

	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "AiCar") {
			AiOpponet tempAiLink = col.gameObject.GetComponent<AiOpponet>();
			tempAiLink.ResetCar();
		}
		if (col.tag == "Player") {
			PlayerMove tempPlayerLink = col.gameObject.GetComponent<PlayerMove>();
			tempPlayerLink.ResetCar();
		}
	}
}
