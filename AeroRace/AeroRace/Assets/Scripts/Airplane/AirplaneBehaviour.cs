using UnityEngine;
using System.Collections;

public class AirplaneBehaviour : MonoBehaviour {

	[Header("Thrust")]
	public float thrustForce = 2000;
	public GameObject thrustPosition;
	public Vector3 thrustVector;
	public GameObject centerOfMassPosition;
	[Header("Lift")]
	public float wingStallSpeed = 30;
	public float wingLiftAtStall = 5886;
	public float wingLiftMultiplier = 1;
	public float wingLiftCoefficient = 4;
	public float criticalAngleOfAttack = 15;
	public GameObject wingLiftPosition;
	public GameObject leftWingSpeedSensorObj;
	public GameObject rightWingSpeedSensorObj;
	[Header("Elevator")]
	public float tailStallSpeed = 30;
	public float tailLiftAtStall = 300;
	public float tailLiftMultiplier = -1;
	public float tailLiftCoefficient = 0.44f;
	public GameObject tailLiftPosition;
	public GameObject tailSpeedSensorObj;
	[Header ("Drag")]
	public float frontDragCoefficient = 0.02f;
	public float upDragCoefficient = 1;
	public float sideDragCoefficient = 0.5f;

	//Variables
	private Rigidbody rb;
	private AirSpeedSensor rightWingSpeedSensor;
	private AirSpeedSensor leftWingSpeedSensor;
	private AirSpeedSensor tailSpeedSensor;

	private float thrustLevel = 1;
	private float wingAngleOfAttack;


	// Use this for initialization
	void Start () {
		rb = transform.root.GetComponentInChildren<Rigidbody>();
		rightWingSpeedSensor = rightWingSpeedSensorObj.GetComponent<AirSpeedSensor>();
		leftWingSpeedSensor = leftWingSpeedSensorObj.GetComponent<AirSpeedSensor>();
		tailSpeedSensor = tailSpeedSensorObj.GetComponent<AirSpeedSensor>();


//		wingLiftCoefficient = CalculateLiftCoefficient(wingLiftAtStall, wingStallSpeed);
//		tailLiftCoefficient = CalculateLiftCoefficient(tailLiftAtStall, tailStallSpeed);
		SetCenterOfMass(centerOfMassPosition);


//		Debug.Log ("Wing c: " + wingLiftCoefficient + ", Tail c: " + tailLiftCoefficient);
//		tailLiftAtStall = CalculateBalancedTailLift(wingLiftAtStall);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		//reset force sum
		ApplyThrust();
		ApplyWingLift();
		ApplyTailLift();
		CalculateDrag();

//		Debug.Log("Speed:" + leftWingSpeedSensorObj.GetComponent<AirSpeedSensor>().AirSpeed().z);
	}

	private float CalculateLiftCoefficient (float liftAtStall, float stallSpeed) {
		float liftCoefficient;
		liftCoefficient = (liftAtStall / Mathf.Pow(stallSpeed, 2));
		return liftCoefficient;
	}

	private void SetCenterOfMass (GameObject centerOfMass) {
		rb.centerOfMass = centerOfMass.transform.position - transform.position;
	}

	private void ApplyThrust () {
		Vector3 thrust;
		thrust = thrustLevel * thrustForce * thrustVector;
		rb.AddRelativeForce (thrust);
	}

	public void ChangeThrustLevel (float thrustIncrement) {
		thrustLevel -= thrustIncrement * Time.deltaTime;
	}

	private float CalculateLift (Vector3 airSpeed, float liftMultiplier, float liftCoefficient) {
		float liftForce;
		float angleOfAttack;
		float angleCoefficient;

//		angleOfAttack = CalculateAngleOfAttack(rb.velocity, leftWingSpeedSensor.AirSpeed());
		angleOfAttack = Vector3.Angle(transform.forward, new Vector3(0, rb.velocity.y, rb.velocity.z));

		angleCoefficient = angleOfAttack / criticalAngleOfAttack;
		if (angleOfAttack > criticalAngleOfAttack) {
			float excess = (angleOfAttack - criticalAngleOfAttack) / criticalAngleOfAttack;
			angleCoefficient -= 2 * excess;
		}
		angleCoefficient = Mathf.Clamp01(angleCoefficient);

//		Debug.Log ("AoA: " + angleOfAttack + ", C: " + angleCoefficient);

		if (airSpeed.z >= 0)
			liftForce = Mathf.Pow(airSpeed.z, 2) * liftCoefficient * liftMultiplier * 0.5f * angleCoefficient;
		else
			liftForce = 0;

		if (liftForce > 80000)
			liftForce = 80000;

		return liftForce;
	}

	private void ApplyWingLift () {
		float leftWingLift;
		float rightWingLift;
		float totalLift;

		leftWingLift = CalculateLift(leftWingSpeedSensor.AirSpeed(), wingLiftMultiplier, wingLiftCoefficient / 2);
		rightWingLift = CalculateLift(rightWingSpeedSensor.AirSpeed(), wingLiftMultiplier, wingLiftCoefficient / 2);
		totalLift = leftWingLift + rightWingLift;
//		totalLift = transform.InverseTransformDirection(totalLift);
		rb.AddForceAtPosition(totalLift * transform.up, wingLiftPosition.transform.position);

//		Debug.Log("Wing Lift: " + totalLift);
	}

	private void ApplyTailLift () {
		float tailLift;

		tailLift = CalculateLift(tailSpeedSensor.AirSpeed(), tailLiftMultiplier, tailLiftCoefficient);
//		tailLift = transform.InverseTransformDirection(tailLift);
		rb.AddForceAtPosition(tailLift * transform.up, tailLiftPosition.transform.position);

//		Debug.Log("Tail Lift: " + tailLift);
	}

	private void CalculateDrag () {
		float frontDrag;
		float upDrag;
		float sideDrag;
		Vector3 airSpeed;
//		Vector3 drag;

		airSpeed = leftWingSpeedSensor.AirSpeed();
		frontDrag = Mathf.Pow(airSpeed.z, 2) * frontDragCoefficient * 0.5f;
		upDrag = Mathf.Pow(airSpeed.y, 2) * upDragCoefficient * 0.5f;
		sideDrag = Mathf.Pow(airSpeed.x, 2) * upDragCoefficient * 0.5f;
		if (airSpeed.z < 0) frontDrag *= -1;
		if (airSpeed.y < 0) upDrag *= -1;
		if (airSpeed.x < 0) sideDrag *= -1;

//		drag = transform.InverseTransformDirection(drag);

		rb.AddForceAtPosition(frontDrag * transform.forward, wingLiftPosition.transform.position);
		rb.AddForceAtPosition(upDrag * transform.up, wingLiftPosition.transform.position);
		rb.AddForceAtPosition(sideDrag * transform.right, wingLiftPosition.transform.position);
	}

	private float CalculateAngleOfAttack (Vector3 velocity, Vector3 airVelocity) {
		Vector3 velocity2D;
		Vector3 airSpeed2D;
		float angleOfAttack;

		velocity2D = new Vector3 (0, velocity.y, velocity.z);
		airSpeed2D = new Vector3 (0, airVelocity.y, airVelocity.z);
		angleOfAttack = Vector3.Angle(airSpeed2D, velocity2D);
		return angleOfAttack;
//		Vector3.Angle(forwardReference, 
	}
}
