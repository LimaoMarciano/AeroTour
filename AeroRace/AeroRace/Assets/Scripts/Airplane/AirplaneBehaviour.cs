using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AirplaneBehaviour : MonoBehaviour {

	[Header("Thrust")]
	public float thrustForce = 2000;
	public GameObject thrustPosition;
	public Vector3 thrustVector;
	public GameObject centerOfMassPosition;
	[Header("Lift")]
	public float wingLiftMultiplier = 1;
	public float wingLiftCoefficient = 4;
	public float wingCriticalAngleOfAttack = 15;
	public float wingAngleOffset = 5;
	public GameObject wingLiftPosition;
	public GameObject leftWingSpeedSensorObj;
	public GameObject rightWingSpeedSensorObj;
	[Header("Tail wing")]
	public float tailLiftMultiplier = -1;
	public float tailLiftCoefficient = 0.44f;
	public float tailCriticalWingOfAttack = 15;
	public float tailAngleOffset = 0;
	public GameObject tailLiftPosition;
	public GameObject tailWingSpeedSensorObj;
	[Header("Flaps")]
	public float flapLiftMultiplier = 1;
	public float flapLiftCoefficient = 0.3f;
	public float flapDragCoefficient = 0.03f;
	public float flapCriticalAoAOffset = 5;
	[Header ("Drag")]
	public float frontDragCoefficient = 0.02f;
	public float upDragCoefficient = 1;
	public float sideDragCoefficient = 0.5f;

	//Variables
	private Rigidbody rb;
	private AirSpeedSensor rightWingSpeedSensor;
	private AirSpeedSensor leftWingSpeedSensor;
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
		rightWingSpeedSensor = rightWingSpeedSensorObj.GetComponent<AirSpeedSensor>();
		leftWingSpeedSensor = leftWingSpeedSensorObj.GetComponent<AirSpeedSensor>();
		tailWingSpeedSensor = tailWingSpeedSensorObj.GetComponent<AirSpeedSensor>();
		initialCriticalAoA = wingCriticalAngleOfAttack;


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
		leftWingAoA = CalculateAngleOfAttack(velocity, leftWingSpeedSensor.AirSpeed(), wingAngleOffset);
		rightWingAoA = CalculateAngleOfAttack(velocity, rightWingSpeedSensor.AirSpeed(), wingAngleOffset);
		tailWingAoA = CalculateAngleOfAttack(velocity, tailWingSpeedSensor.AirSpeed(), tailAngleOffset);

		leftWingAngleCoefficient = CalculateAngleCoefficient(leftWingAoA, wingCriticalAngleOfAttack, isFlapOn);
		rightWingAngleCoefficient = CalculateAngleCoefficient(rightWingAoA, wingCriticalAngleOfAttack, isFlapOn);
		tailWingAngleCoefficient = CalculateAngleCoefficient(tailWingAoA, tailCriticalWingOfAttack, isFlapOn);

		ApplyThrust();
		ApplyWingLift();
		ApplyTailLift();
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

	private float CalculateLift (Vector3 airSpeed, float liftMultiplier, float liftCoefficient, float angleCoefficient) {
		float liftForce;

		if (airSpeed.z >= 0)
			liftForce = Mathf.Pow(airSpeed.z, 2) * liftCoefficient * liftMultiplier * 0.5f * angleCoefficient;
		else
			liftForce = 0;

		if (liftForce > 80000)
			liftForce = 80000;
		return liftForce;
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

	private float CalculateAngleOfAttack (Vector3 velocity, Vector3 airVelocity, float angleOfAttackOffset) {
		Vector3 velocity2D;
		Vector3 airVelocity2D;
		float angleOfAttack;
		float angleSign;
		Vector3 crossProduct;

//		velocity2D = new Vector3 (0, velocity.y, velocity.z);
//		airVelocity2D = new Vector3 (0, airVelocity.y, airVelocity.z);

//		velocity2D = Quaternion.AngleAxis(-angleOfAttackOffset, transform.right) * velocity2D;

		Vector3 planeNormal = Vector3.Cross(transform.up, transform.forward);
		planeNormal.Normalize();
		float distance = -Vector3.Dot(planeNormal, rb.velocity);
		Vector3 projectedVelocity = rb.velocity + planeNormal * distance;

		angleOfAttack = Vector3.Angle(transform.forward, projectedVelocity);
		crossProduct = Vector3.Cross(transform.forward, projectedVelocity);
		angleSign = Mathf.Sign(Vector3.Dot(transform.right, crossProduct));
		
		angleOfAttack *= angleSign;

		Debug.DrawLine(wingLiftPosition.transform.position, wingLiftPosition.transform.position + projectedVelocity.normalized * 5, Color.blue);
		Debug.DrawLine(wingLiftPosition.transform.position, wingLiftPosition.transform.position + transform.forward * 5, Color.white);
		Debug.DrawLine(wingLiftPosition.transform.position, wingLiftPosition.transform.position + crossProduct.normalized * 5, Color.yellow);

		return angleOfAttack;
	}

	private float CalculateAngleCoefficient (float angleOfAttack, float criticalAngleOfAttack, bool useFlap) {
		float angleCoefficient;

		if (useFlap) {
			criticalAngleOfAttack -= flapCriticalAoAOffset / 2;
			angleCoefficient = (angleOfAttack + flapCriticalAoAOffset + 5) * 0.07f;
		}
		else {
			angleCoefficient = (angleOfAttack + 5)* 0.07f;
		}
			
		if (angleOfAttack > criticalAngleOfAttack) {
			float excess = (angleOfAttack - criticalAngleOfAttack) * 0.07f;
			angleCoefficient -= 2 * excess;
			if (angleOfAttack > 25)
				angleCoefficient = 0;
		}
		if (angleOfAttack < -criticalAngleOfAttack) {
			float excess = (angleOfAttack + criticalAngleOfAttack) * 0.07f;
			angleCoefficient -= 2 * excess;
			if (angleOfAttack < -25)
				angleCoefficient = 0;
		}
		return angleCoefficient;
	}

	private void ApplyWingLift () {
		Vector3 direction;
		direction = Vector3.Cross(new Vector3(0, rb.velocity.y, rb.velocity.z), transform.right);
		direction.Normalize();

		Debug.DrawLine(wingLiftPosition.transform.position, wingLiftPosition.transform.position + direction);

		leftWingLift = CalculateLift(leftWingSpeedSensor.AirSpeed(), wingLiftMultiplier, wingLiftCoefficient / 2, leftWingAngleCoefficient);
		rightWingLift = CalculateLift(rightWingSpeedSensor.AirSpeed(), wingLiftMultiplier, wingLiftCoefficient / 2, rightWingAngleCoefficient);

		if (isFlapOn) {
			leftWingLift += CalculateLift(leftWingSpeedSensor.AirSpeed(), flapLiftMultiplier, flapLiftCoefficient / 2, leftWingAngleCoefficient);
			rightWingLift += CalculateLift(rightWingSpeedSensor.AirSpeed(), flapLiftMultiplier, flapLiftCoefficient / 2, rightWingAngleCoefficient);
		}

		totalLift = leftWingLift + rightWingLift;
		rb.AddForceAtPosition(totalLift * direction, wingLiftPosition.transform.position);

		//		Debug.Log("Wing Lift: " + totalLift);
	}

	private void ApplyTailLift () {
		Vector3 direction;
		direction = Vector3.Cross(new Vector3(0, rb.velocity.y, rb.velocity.z), transform.right);
		direction.Normalize();
		tailLift = CalculateLift(tailWingSpeedSensor.AirSpeed(), tailLiftMultiplier, tailLiftCoefficient, tailWingAngleCoefficient);
		//		tailLift = transform.InverseTransformDirection(tailLift);
		rb.AddForceAtPosition(tailLift * direction, tailLiftPosition.transform.position);

		//		Debug.Log("Tail Lift: " + tailLift);
	}

	public List<float> GetLiftForce() {
		List<float> liftValues = new List<float>();
		liftValues.Add(rightWingLift);
		liftValues.Add(leftWingLift);
		liftValues.Add(tailLift);

		return liftValues;
	}

	public List<float> GetDragForce() {
		List<float> dragValues = new List<float>();
		dragValues.Add(sideDrag);
		dragValues.Add(upDrag);
		dragValues.Add(frontDrag);

		return dragValues;
	}

	public List<Vector3> GetAirSpeed() {
		List<Vector3> airSpeedValues = new List<Vector3>();
		airSpeedValues.Add(leftWingSpeedSensor.AirSpeed());
		airSpeedValues.Add(rightWingSpeedSensor.AirSpeed());
		airSpeedValues.Add(tailWingSpeedSensor.AirSpeed());

		return airSpeedValues;
	}

	public List<float> GetAngleOfAttack() {
		List<float> aOAValues = new List<float>();
		aOAValues.Add(leftWingAoA);
		aOAValues.Add(rightWingAoA);
		aOAValues.Add(tailWingAoA);

		return aOAValues;
	}

	public List<float> GetAngleCoefficients() {
		List<float> coefficientValues = new List<float>();
		coefficientValues.Add(leftWingAngleCoefficient);
		coefficientValues.Add(rightWingAngleCoefficient);
		coefficientValues.Add(tailWingAngleCoefficient);

		return coefficientValues;
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
