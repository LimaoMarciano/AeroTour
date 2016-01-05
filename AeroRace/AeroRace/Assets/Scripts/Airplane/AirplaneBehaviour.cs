using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirplaneBehaviour : MonoBehaviour {

	[Header("Thrust")]
	public float thrustForce = 2000;
	public GameObject thrustPosition;
	public Vector3 thrustVector;
	public GameObject centerOfMassPosition;
	[Header("Flaps")]
	public float flapLiftMultiplier = 1;
	public float flapLiftCoefficient = 0.3f;
	public float flapDragCoefficient = 0.03f;
	public float flapCriticalAoAOffset = 5;
	[Header ("Drag")]
	public float frontDragCoefficient = 0.02f;
	public float upDragCoefficient = 1;
	public float sideDragCoefficient = 0.5f;

	public AirSpeedSensor leftWingSpeedSensor;

	//Variables
	private Rigidbody rb;
	private AirSpeedSensor rightWingSpeedSensor;
//	private AirSpeedSensor leftWingSpeedSensor;
	private AirSpeedSensor tailWingSpeedSensor;

	private float leftWingLift;
	private float rightWingLift;
	private float totalLift;
	private float tailLift;

	private float leftWingAoA;
	private float rightWingAoA;
	private float tailWingAoA;
	private float leftWingAngleCoefficient;
	private float rightWingAngleCoefficient;
	private float tailWingAngleCoefficient;

	public bool isFlapOn = true;
	private float initialCriticalAoA;

	private float frontDrag;
	private float upDrag;
	private float sideDrag;

	private float thrustLevel = 0;
	private float wingAngleOfAttack;

	private float hInput;
	private float vInput;

	void Awake () {
		//Fix for unity5 bug where wheels get locked at start
		foreach (WheelCollider w in GetComponentsInChildren<WheelCollider>()) 
			w.motorTorque = 0.000001f;
	}

	// Use this for initialization
	void Start () {
		rb = transform.root.GetComponentInChildren<Rigidbody>();


//		wingLiftCoefficient = CalculateLiftCoefficient(wingLiftAtStall, wingStallSpeed);
//		tailLiftCoefficient = CalculateLiftCoefficient(tailLiftAtStall, tailStallSpeed);
		SetCenterOfMass(centerOfMassPosition);


//		Debug.Log ("Wing c: " + wingLiftCoefficient + ", Tail c: " + tailLiftCoefficient);
//		tailLiftAtStall = CalculateBalancedTailLift(wingLiftAtStall);
	}
	
	// Update is called once per frame
	void Update () {
		hInput = Input.GetAxis("Horizontal");
		vInput = Input.GetAxis("Vertical");
	}

	void FixedUpdate () {

		rb.AddTorque(transform.forward * -hInput * 4000);
		rb.AddTorque(transform.right * vInput * 8000);

		Vector3 velocity = rb.velocity;

		ApplyThrust();
		CalculateDrag();

	}

	private void SetCenterOfMass (GameObject centerOfMass) {
		rb.centerOfMass = centerOfMass.transform.localPosition;
	}

	private void ApplyThrust () {
		Vector3 thrust;
		thrust = thrustLevel * thrustForce * thrustVector;
		rb.AddRelativeForce (thrust);
	}
		
	private void CalculateDrag () {
		Vector3 airSpeed;
//		Vector3 drag;

		airSpeed = leftWingSpeedSensor.AirSpeed();
		frontDrag = Mathf.Pow(airSpeed.z, 2) * frontDragCoefficient * 0.5f;
		upDrag = Mathf.Pow(airSpeed.y, 2) * upDragCoefficient * 0.5f;
		sideDrag = Mathf.Pow(airSpeed.x, 2) * upDragCoefficient * 0.5f;

		if(isFlapOn) {
			frontDrag += Mathf.Pow(airSpeed.z, 2) * flapDragCoefficient * 0.5f;
		}

		if (airSpeed.z < 0) frontDrag *= -1;
		if (airSpeed.y < 0) upDrag *= -1;
		if (airSpeed.x < 0) sideDrag *= -1;

//		drag = transform.InverseTransformDirection(drag);

		rb.AddForceAtPosition(-frontDrag * transform.forward, centerOfMassPosition.transform.position);
		rb.AddForceAtPosition(-upDrag * transform.up, centerOfMassPosition.transform.position);
		rb.AddForceAtPosition(-sideDrag * transform.right, centerOfMassPosition.transform.position);
	}

	public float GetEngineLevel() {
		return thrustLevel;
	}

	public bool GetFlapsStatus() {
		return isFlapOn;
	}

	public void ChangeThrustLevel (float thrustIncrement) {
		thrustLevel += thrustIncrement * Time.deltaTime;
		thrustLevel = Mathf.Clamp01(thrustLevel);
	}

	public void ToggleFlaps () {
		isFlapOn = !isFlapOn;
	}
}
