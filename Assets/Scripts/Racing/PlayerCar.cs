using UnityEngine;
using System.Collections;

public class PlayerCar : MonoBehaviour {
	private Transform carTransform;										//our transform
	private Rigidbody carRidgidbody;									//our ridgidbody
	private Vector3 carUp;												//our up			
	private Vector3 carRight;											//our right
	private Vector3 carForward;											//our forward
	private Vector3 carBackwards;										//our back	

	public Vector3 centerOfGravity = new Vector3(0,-0.7,0.35);			//how far do we force the cog down 
	public float carMass;												//how heavy is our car

	private float h;													//our Horizantal axis position used for steering
	private float v;													//our Vertical axis Position used for throttle and brake

	void initilizer () 
	{
		//cache this car's transform
		carTransform = transform;
		//cache this car's ridgidbody
		carRidgidbody = rigidbody;
		//set up this car's mass
		carMass = carRidgidbody.mass;
		//to prevent the car from fliping we need to set the car's center of gravity lower
		if (centerOfGravity != Vector3.zero)
						//set it to our custom if we have one
						carRidgidbody.centerOfMass = centerOfGravity;
				else
						//set it to unity's value if we don't
						centerOfGravity = carRidgidbody.centerOfMass;
		//cache this car's vectors
		carUp = carTransform.up;
		carUp = carTransform.right;
		carUp = carTransform.forward;
		//start the set up wheels function
		setUpWheels ();
	}

	void setUpWheels()
	{

	}

	void carPhysicsUpdate ()
	{

	}

	void rotateVisualWheels ()
	{

	}
	void updateEngineSound ()
	{

	}
	// Use this for initialization
	void Start () 
	{
		initilizer ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		h = Input.GetAxis ("Horizontal");
		v = Input.GetAxis ("Vertical");

		carPhysicsUpdate ();
	}
	void LateUpdate () 
	{
		//turn the visual wheels
		rotateVisualWheels ();
		//make the vroom! vrooom!
		updateEngineSound ();
	}
}
