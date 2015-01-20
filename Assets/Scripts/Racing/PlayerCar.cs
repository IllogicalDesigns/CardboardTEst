using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class PlayerCar : MonoBehaviour {
	public Transform RFwheel;											//our wheel
	public Transform LFwheel;											//our wheel
	public Transform RBwheel;											//our wheel
	public Transform LBwheel;											//our wheel
	public Transform leftFrontWheel;									//our wheel
	public Transform rightFrontWheel;									//our wheel
	public Transform[] myWheels;										//our wheels
	private Transform carTransform;										//our transform
	private Rigidbody carRidgidbody;									//our ridgidbody
	private Vector3 carUp;												//our up			
	private Vector3 carRight;											//our right
	private Vector3 carForward;											//our forward
	private Vector3 carBackwards;										//our back	
	private Vector3 relativeVelocity;									//our relative velocity
	private Vector3 myRight;											//vector my current right
	private Vector3 flatVelo;											//a vector that is flat ????
	private Vector3 imp;												//something to do with fixed update and impulsing the force
	private float maxSpeedToTurn = 0.2f; 								//Max speed of the Turning
	private Vector3 engineForce;										//the force the engine gives off
	private Vector3 turnVec;											//a vector of turing
	private Vector3 velo;												//a vector vector
	private float actualTurn;											//turn
	private Vector3 tmpVec; 											//a non-tmp tmp variable
	private Vector3 dir;												//dir var
	private Vector3 flatDir;											//flat direction no vertical
	private float slideSpeed;											//our curr sliding speed
	private float rev;													//revs currently


	public float actualGrip;
	public float maxSpeed = 50f;										//limit speed to this value
	public float throttle = 5f;											//our throttle amount
	public Vector3 centerOfGravity = new Vector3(0f,-0.7f,0.35f);		//how far do we force the cog down 
	public float carMass;												//how heavy is our car
	public float mySpeed;												//our speed
	public float power = 300f;											//power
	public float carGrip = 70f;											//our cars grip
	public float turnSpeed = 3f;										//what is our turn speed

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
		//we are gonna ad a wheel finder script evenually
		if(RFwheel == null || LFwheel == null|| RBwheel == null|| LBwheel == null)
		{
			Debug.LogError("wheel transforms have not been found for " + gameObject.name);
			Debug.Break();
		}
		else
		{
			myWheels[0] = LFwheel;
			myWheels[1] = LBwheel;
			myWheels[2] = RFwheel;
			myWheels[3] = RFwheel;
		}

	}

	private Vector3  rotationAmount;

	void rotateVisualWheels ()
	{
		//Front wheels roation based on steering
		//leftFrontWheel.localEulerAngles.y = h * 30f;
		//rightFrontWheel.localEulerAngles.y = h * 30f;

		rotationAmount = carRight * (relativeVelocity.z * 1.6f * Time.deltaTime * Mathf.Rad2Deg);

		myWheels [0].Rotate (rotationAmount);
		myWheels [1].Rotate (rotationAmount);
		myWheels [2].Rotate (rotationAmount);
		myWheels [3].Rotate (rotationAmount);
	}
	void carPhysicsUpdate ()
	{
		//grab all the physics info we need to calculate everything
		myRight = carTransform.right;
		//we find our velocity
		velo = carRidgidbody.velocity;
		tmpVec = new Vector3 (velo.x, 0, velo.z);
		//our velocity with our y movement
		flatVelo = tmpVec;
		//find direction we our moving in
		dir = transform.TransformDirection (carForward);
		tmpVec = new Vector3 (dir.x, 0, dir.z);
		//figure out our velocity without y movement - our flat velocity
		flatDir = Vector3.Normalize (tmpVec);
		//calculate realtive velocity
		relativeVelocity = carTransform.InverseTransformDirection (flatVelo);
		//calculate sliding along x
		slideSpeed = Vector3.Dot (myRight, flatVelo);
		//calculae current speed along flat velocity
		mySpeed = flatVelo.magnitude;
		//check to see if we are movinf in reverse
		rev = Mathf.Sign (Vector3.Dot (flatVelo, flatDir));
		//calculate engine force with flat vector and acceration
		engineForce = (flatDir * (power * throttle) * carMass);
		//perform the turning fuction
		actualTurn = h;
		//if reversing we need to reverse the turn
		if (rev < 0.1f)
						actualTurn -= actualTurn;
		//calculate tourqe to apply to the ridgidbody
		turnVec = (((carUp * turnSpeed) * actualTurn) * carMass) * 800f;
		//calculate pulses to make grip more realistic
		actualGrip = Mathf.Lerp(100f, carGrip, mySpeed * 0.02f);
		imp = myRight * (-slideSpeed * carMass * actualGrip);

	}
	void SlowVelocity () 
	{
		carRidgidbody.AddForce (-flatVelo * 0.8f);
	}
	void updateEngineSound ()
	{
		audio.pitch = 0.30f + mySpeed * 0.025f;
		if (mySpeed > 30)
						audio.pitch = 0.25f + mySpeed * 0.015f;
		if (mySpeed > 40)
						audio.pitch = 0.20f + mySpeed * 0.013f;
		if (mySpeed > 49)
						audio.pitch = 0.15f + mySpeed * 0.011f;
		//dont let the pitch get too high then reset it
		if (audio.pitch > 2.0f)
						audio.pitch = 2f;
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
	void FixedUpdate () 
	{
		if (mySpeed < maxSpeed)
						carRidgidbody.AddForce (engineForce * Time.deltaTime);
		if (mySpeed > maxSpeedToTurn)
						carRidgidbody.AddTorque (turnVec * Time.deltaTime);
				else if (mySpeed < maxSpeedToTurn)
						return;
		carRidgidbody.AddForce (imp * Time.deltaTime);
	}
	void LateUpdate () 
	{
		//turn the visual wheels
		rotateVisualWheels ();
		//make the vroom! vrooom!
		updateEngineSound ();
	}
}
