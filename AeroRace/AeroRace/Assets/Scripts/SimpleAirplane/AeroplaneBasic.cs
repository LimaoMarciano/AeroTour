using UnityEngine;
using System.Collections;

public class AeroplaneBasic : MonoBehaviour {

	public float thrustPower = 10000;
	public float elevatorPower = 2000;
	public float aileronsPower = 2000;
	public float maxPitchDrag = 3;
	public GameObject liftPosition;

	private Rigidbody rb;
	private float iHorizontal;
	private float iVertical;
	private float thrustLevel = 0;

	public float maxWingLift = 10000;
	public float liftPerSpeed = 33;
	public float airForwardVelocity;
	public float airUpVelocity;
	public float airRightVelocity;
	private float currentLift;
	private Vector3 airDirection;

	private Vector3 forceSum = new Vector3(0,0,0);

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		//rb.velocity = new Vector3(0, 0, 200);
	}
	
	// Update is called once per frame
	void Update () {
		iHorizontal = Input.GetAxis("Horizontal");
		iVertical = Input.GetAxis("Vertical");
		if(Input.GetButton("IncreaseSpeed")) {
			thrustLevel += 0.5f * Time.deltaTime;
			if (thrustLevel > 1) {
				thrustLevel = 1;
			}
		}
		if(Input.GetButton("DecreaseSpeed")) {
			thrustLevel -= 0.5f * Time.deltaTime;
			if (thrustLevel < 0) {
				thrustLevel = 0;
			}
		}
//		Debug.Log("Thrust level: " + thrustLevel);
	}

	void FixedUpdate () {
		forceSum = Vector3.zero;
		calculateAirSpeed();
		AirResistence();
		Elevator(iVertical);
		Ailerons(iHorizontal);
		Thruster(thrustLevel);
		CalculateWingLift();

		rb.AddRelativeForce(forceSum, ForceMode.Force);
	}

	private void Thruster(float throttle) {
		if (rb.velocity.magnitude < 300)
			rb.AddRelativeForce(Vector3.forward * thrustPower * throttle);
		//rb.AddRelativeForce(Vector3.forward * thrustPower * throttle, ForceMode.VelocityChange);
	}

	private void Elevator(float angle) {
		rb.AddRelativeTorque(Vector3.right * elevatorPower * angle);
	}

	private void Ailerons(float angle) {
		rb.AddRelativeTorque(Vector3.forward * aileronsPower * -angle);
	}

//	private void PitchAirResistance () {
//		float maxAngle = 180;
//		float pitchDrag;
//		float angleDif;
//
//		angleDif = Vector3.Angle(transform.forward, rb.velocity);
//		pitchDrag = (maxPitchDrag * angleDif) / maxAngle;
//		rb.drag = pitchDrag;
//	}

	//public float maxWingSpeedLift = 500;


	private void calculateAirSpeed() {
		airForwardVelocity = Vector3.Dot(rb.velocity, transform.forward);
		airUpVelocity = Vector3.Dot(rb.velocity, transform.up);
		airRightVelocity = Vector3.Dot(rb.velocity, transform.right);
	}

	private void CalculateWingLift () {
		
		currentLift = liftPerSpeed * airForwardVelocity;
		if (currentLift > maxWingLift) {
			currentLift = maxWingLift;
		}
		//Debug.Log ("Wind Speed:" + airForwardVelocity + ", Lift:" + currentLift);
		rb.AddForceAtPosition(transform.up * currentLift, liftPosition.transform.position);
	}

	private void AirResistence () {
		float forwardDrag;
		float upDrag;
		float rightDrag;
		float airDensity = 1.2f;
		Vector3 dragForce;
		float forwardArea = 3;
		float upArea = 10;
		float rightArea = 5;
		float forwardDragC = 0.07f;
		float upDragC = 1.15f;
		float rightDragC = 0.47f;

		dragForce = Vector3.zero;

		forwardDrag = forwardDragC * forwardArea * 0.5f * airDensity * Mathf.Pow (airForwardVelocity, 2);
		upDrag = upDragC * upArea * 0.5f * airDensity * airUpVelocity * Mathf.Pow (airUpVelocity, 2);
		rightDrag = rightDragC * rightArea * 0.5f * airDensity * Mathf.Pow (airRightVelocity, 2);

		dragForce = new Vector3 (-rightDrag, -upDrag, -forwardDrag);
		rb.AddRelativeForce(dragForce);

		Debug.Log ("Drag force: " + dragForce);
//		float dragPerSpeed = 0.007f;
//		float airResistence = rb.velocity.magnitude * dragPerSpeed;
//		rb.drag = airResistence;
	}

	void LateUpdate () {
		Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.red);
		Debug.DrawLine(transform.position, transform.position + transform.up * currentLift, Color.green);
		//Debug.DrawLine(transform.position, transform.position + forceSum);
		//Debug.DrawLine(transform.position, transform.position + transform.forward * 10, Color.blue);
		//Debug.DrawLine(transform.position, windSpeed + transform.position, Color.green);
	}
}
