using UnityEngine;
using System.Collections;

public class AirplanePiece : MonoBehaviour {

	//Settings
	[Header("Lift")]
	public float criticalAngle = 15;
	public float liftCoeficient = 50;
	public Vector3 liftDirection;
	[Header ("Area")]
	public Vector3 area = new Vector3 (1, 1, 1);
	[Header("Drag")]
	public Vector3 dragCoefficient = new Vector3 (0.5f, 0.5f, 0.5f);
	[Header ("Thrust")]
	public bool canGenerateThrust = false;
	public float thrustForce = 0;
	public bool canChangeCenterOfMass = false;
	[Header ("Control")]
	public float maxAngle = 15;
	public Vector3 rotationAxis;

	//Needed GameObjects
	[Header("GameObjects")]
	public GameObject airSpeedSensorObject;
	public GameObject liftPositionObject;
	public GameObject thrustPositionObject;
	public GameObject centerOfMassObject;
	public GameObject controlableSurface;

	//Components
	private Rigidbody rb;

	//Private variables
	private float airDensity = 1.2f;
	private bool isRequisitesMet = true;
	private Vector3 airSpeed;
	private Vector3 airSpeedSensorLastPos;

	private Vector3 dragForce;
	private Vector3 liftForce;

	private float surfaceAngle;
	private Vector3 initialAngle;

	// Use this for initialization
	void Start () {

		if (controlableSurface)
			initialAngle = controlableSurface.transform.localEulerAngles;

		//Try to get rigidbody from parent
		rb = transform.root.GetComponentInChildren<Rigidbody>();
		if (rb == null) {
			Debug.Log (gameObject.name + " could not find Rigidbody.");
			isRequisitesMet = false;
		}

		//Checks if it has the airSpeedSensor object and defines initial state
		if (airSpeedSensorObject) {
			airSpeedSensorLastPos = airSpeedSensorObject.transform.position;
		}
		else
		{
			Debug.Log (gameObject.name + " is missing speed sensor GameObject");
			isRequisitesMet = false;
		}

		//Checks if it has liftPosition object defined
		if (!liftPositionObject) {
			Debug.Log (gameObject.name + " is missing lift position GameObject");
			isRequisitesMet = false;
		}

		//Checks if it has thrustPosition object defined
		if (canGenerateThrust) {
			if (!thrustPositionObject) {
				Debug.Log (gameObject.name + " is missing thrust position GameObject");
				isRequisitesMet = false;
			}
		}

		if (canChangeCenterOfMass) {
			if (!centerOfMassObject) {
				Debug.Log (gameObject.name + " is missing center of mass GameObject");
				isRequisitesMet = false;
			} else {
				rb.centerOfMass = transform.position - centerOfMassObject.transform.position;
			}
		}

		//Prints message if requirements failed
		if (!isRequisitesMet) {
			Debug.Log (gameObject.name + " is not configured correctly. Simulation disabled for this piece.");
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		
		if (isRequisitesMet) {
			CalculateAirSpeed();
			ApplyDrag();
			if (liftPositionObject)
				ApplyLift();
			if (canGenerateThrust)
				Thrust ();
		}
	}

	void LateUpdate () {
		DebugLines ();
	}

	/// <summary>
	/// Calculates the air speed in m/s
	/// </summary>
	private void CalculateAirSpeed () {
		airSpeed = (airSpeedSensorObject.transform.position - airSpeedSensorLastPos) / Time.fixedDeltaTime;
		airSpeedSensorLastPos = airSpeedSensorObject.transform.position;
		airSpeed = transform.InverseTransformDirection (airSpeed);
	}

	/// <summary>
	/// Applies drag using the equation: 
	/// Drag = area * dragCoefficient * airSpeed^2 * 0.5 * airDensity
	/// </summary>
	private void ApplyDrag () {
		Vector3 localAirSpeed = transform.TransformDirection(airSpeed);

		//Not possible to multiply two vectors, so I'm using Vector3.Scale to do that
		dragForce = Vector3.Scale(Vector3.Scale (dragCoefficient, area), Vector3.Scale (localAirSpeed, localAirSpeed)) * 0.5f * airDensity;
		if (localAirSpeed.x < 0) dragForce.x *= -1;
		if (localAirSpeed.y < 0) dragForce.y *= -1;
		if (localAirSpeed.z < 0) dragForce.z *= -1;

		rb.AddForceAtPosition (-dragForce, liftPositionObject.transform.position);
	}

	/// <summary>
	/// Applies the lift.
	/// </summary>
	private void ApplyLift () {
		float angleOfAttack;
		float liftCoefficient;
		float liftForceY;
		Vector3 localAirSpeed;

		localAirSpeed = transform.TransformDirection (airSpeed);
		angleOfAttack = Vector3.Angle(transform.forward, new Vector3 (0, localAirSpeed.y, localAirSpeed.z));

		liftCoeficient = 0.11f * angleOfAttack;
		if (angleOfAttack > criticalAngle) {
			liftCoeficient -= 2 * 0.11f * (angleOfAttack - criticalAngle);
		}
	
		if (angleOfAttack < -criticalAngle) {
			liftCoeficient += 2 * 0.11f * (angleOfAttack - criticalAngle);
		}

		if (liftCoeficient < 0)
			liftCoeficient = 0;

		liftForceY = 0.5f * airDensity * Mathf.Pow (airSpeed.z, 2) * area.y * liftCoeficient;
		if (airSpeed.z < 0)
			liftForceY = 0;

		//Add controlable surface influence
		if (controlableSurface)
			liftForceY += (surfaceAngle / maxAngle) * 5000 * liftCoeficient;

		liftForce = liftDirection * liftForceY;
	
		rb.AddForceAtPosition (liftForce, liftPositionObject.transform.position);
	}

	private void Thrust() {
		rb.AddForceAtPosition (transform.forward * thrustForce, thrustPositionObject.transform.position);
	}

	private void DebugLines () {
		Vector3 liftPos = liftPositionObject.transform.position;
		Debug.DrawLine (liftPos, liftPos + (transform.up * (liftForce.y / 3000)), Color.green);
		Debug.DrawLine (liftPos, liftPos + (transform.right * (-dragForce.x / 3000)), Color.red);
		Debug.DrawLine (liftPos, liftPos + (transform.up * (-dragForce.y / 3000)), Color.red);
		Debug.DrawLine (liftPos, liftPos + (transform.forward * (-dragForce.z / 3000)), Color.red);
	}

	public void rotateWing (float input) {
		controlableSurface.transform.localEulerAngles = initialAngle + (input * maxAngle * rotationAxis);
		surfaceAngle = input * maxAngle;
	}

}
