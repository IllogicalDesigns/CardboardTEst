using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameMenus : MonoBehaviour {
	private SmoothDamp mySmoothDamp;
	private Transform myGamePlayTrackPoint;
	public Transform myMenuTrackPoint;
	public Canvas myInGameGui;
	public Canvas myPauseMenu;
	private float myOriginalSmooth;
	private Vector3 myOriginalFollowPoint;
	public Vector3 myMenuFollowPoint;
	public float myPauseSmooth;
	public bool amIPaused = false;

	// Use this for initialization
	void Start () 
	{
		mySmoothDamp = gameObject.GetComponent<SmoothDamp> ();
		myOriginalSmooth = mySmoothDamp.smoothTime;
		myGamePlayTrackPoint = mySmoothDamp.target;
		myOriginalFollowPoint = mySmoothDamp.newFollowPoint;
	}

	public void UnPauseGame ()
	{
		/*
		mySmoothDamp.newFollowPoint = myOriginalFollowPoint;
		mySmoothDamp.target = myGamePlayTrackPoint;
		mySmoothDamp.smoothTime = myOriginalSmooth;
		*/
		mySmoothDamp.enabled = true;
		amIPaused = false;
		myPauseMenu.enabled = false;
		myInGameGui.enabled = true;
		Time.timeScale = 1f;
	}

	public void PauseGame ()
	{
		/*
		myOriginalFollowPoint = mySmoothDamp.newFollowPoint;
		myGamePlayTrackPoint = mySmoothDamp.target;
		mySmoothDamp.newFollowPoint = myMenuFollowPoint;
		transform.position = myMenuTrackPoint.position;
		transform.rotation = myMenuTrackPoint.rotation;
		mySmoothDamp.target = myMenuTrackPoint;
		mySmoothDamp.smoothTime = myPauseSmooth;
		*/
		mySmoothDamp.enabled = false;
		amIPaused = true;
		myPauseMenu.enabled = true;
		myInGameGui.enabled = false;
		Time.timeScale = 0f;
	}
	public void goToMain ()
	{
		Application.LoadLevel ("MainMenu");
		Time.timeScale = 1f;
	}
	private float HowCloseAmI (Vector3 pointA, Vector3 pointB)	//this is what you have to do to make this work
	{
		Vector3 offset = pointA - pointB;					//This is our offset from the target
		float sqrLen = offset.sqrMagnitude;					//this does a love function i cant describe to you
		return sqrLen;										//this gives you the product of that sweet love function
	}
	void Update ()
	{
		if (amIPaused && HowCloseAmI (transform.position, myMenuTrackPoint.position) > 7f) {
						transform.position = Vector3.Lerp (transform.position, myMenuTrackPoint.position, 0.1f);
						transform.LookAt (myMenuTrackPoint);
				}

	}
}
