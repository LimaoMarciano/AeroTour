using UnityEngine;
using System.Collections;

public class AirplaneManager : MonoBehaviour {

	public GameObject centerOfMass;
	public GameObject leftWingObject;
	public GameObject rightWingObject;
	public GameObject leftHStabilizerObject;
	public GameObject rightHStabilizerObject;
	public GameObject verticalStabilizerObject;

	private WingBehaviour leftWing;
	private WingBehaviour rightWing;
	private HorizontalStabilizer leftHStabilizer;
	private HorizontalStabilizer rightHStabilizer;
	private VerticalStabilizer verticalStabilizer;

	public float thrustPower = 10000;
	public float elevatorPower = 2000;
	public float aileronsPower = 2000;
	public float aileronRotateSpeed = 0.05f;

	private Rigidbody rb;
	private float iHorizontal;
	private float iVertical;
	private float iRudder;
	private float iFlap;
	private bool isFlapOn = false;
	private float thrustLevel = 0;

	public float airForwardVelocity;
	public float airUpVelocity;
	public float airRightVelocity;
	private Vector3 localVelocity;
	private Vector3 dragForce;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		leftWing = leftWingObject.GetComponent<WingBehaviour> ();
		rightWing = rightWingObject.GetComponent<WingBehaviour> ();
		leftHStabilizer = leftHStabilizerObject.GetComponent<HorizontalStabilizer> ();
		rightHStabilizer = rightHStabilizerObject.GetComponent<HorizontalStabilizer> ();
		verticalStabilizer = verticalStabilizerObject.GetComponent<VerticalStabilizer> ();
//		rb.centerOfMass = centerOfMass.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		iHorizontal = Input.GetAxis("Horizontal");
		iVertical = Input.GetAxis("Vertical");
		iRudder = Input.GetAxis ("Rudder");


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

		if (Input.GetButtonDown ("Flap") && !isFlapOn) {
			isFlapOn = true;
			iFlap = 1;
			Debug.Log ("Flap on!");
		}
		else if (Input.GetButtonDown ("Flap") && isFlapOn) {
			isFlapOn = false;
			iFlap = 0;
			Debug.Log ("Flap off!");
		}
			
	}

	void FixedUpdate () {
		calculateAirSpeed();
		AirResistence();
		leftHStabilizer.AdjustElevatorAngle(iVertical);
		rightHStabilizer.AdjustElevatorAngle(iVertical);
		verticalStabilizer.AdjustRudderAngle (iRudder);
		rightWing.adjustAileronAngle(-iHorizontal);
		leftWing.adjustAileronAngle(iHorizontal);
		rightWing.adjustFlapAngle (iFlap);
		leftWing.adjustFlapAngle (iFlap);
		Thruster(thrustLevel);

		}

	void LateUpdate () {
		DebugDragForceLines();
	}

	private void calculateAirSpeed() {
		localVelocity = transform.InverseTransformDirection(rb.velocity);
		airForwardVelocity = localVelocity.z;
		airUpVelocity = localVelocity.y;
		airRightVelocity = localVelocity.x;
//		airForwardVelocity = Vector3.Dot(rb.velocity, transform.forward);
//		airUpVelocity = Vector3.Dot(rb.velocity, transform.up);
//		airRightVelocity = Vector3.Dot(rb.velocity, transform.right);
	}

	private void Thruster(float throttle) {
		if (rb.velocity.magnitude < 300)
			rb.AddRelativeForce(Vector3.forward * thrustPower * throttle);
		//rb.AddRelativeForce(Vector3.forward * thrustPower * throttle, ForceMode.VelocityChange);
	}

	private void Elevator(float angle) {
		rb.AddRelativeTorque(Vector3.right * elevatorPower * angle);
	}

	private void AirResistence () {
		float forwardDrag;
		float upDrag;
		float rightDrag;
		float airDensity = 1.2f;
		float forwardArea = 3;
		float upArea = 8;
		float rightArea = 4;
		float forwardDragC = 0.07f;
		float upDragC = 1.15f;
		float rightDragC = 0.47f;

		dragForce = Vector3.zero;

		forwardDrag = forwardDragC * forwardArea * 0.5f * airDensity * Mathf.Pow (airForwardVelocity, 2);
		upDrag = upDragC * upArea * 0.5f * airDensity * Mathf.Pow (-airUpVelocity, 2);
		rightDrag = rightDragC * rightArea * 0.5f * airDensity * Mathf.Pow (airRightVelocity, 2);

		if (airForwardVelocity < 0) forwardDrag *= -1;
		if (airUpVelocity < 0) upDrag *= -1;
		if (airRightVelocity < 0) rightDrag *= -1;

		dragForce = new Vector3 (rightDrag, upDrag, forwardDrag);
		rb.AddRelativeForce(-dragForce);

		//Debug.Log ("Drag force: " + dragForce);
		//		float dragPerSpeed = 0.007f;
		//		float airResistence = rb.velocity.magnitude * dragPerSpeed;
		//		rb.drag = airResistence;
	}

	private void DebugDragForceLines () {
		Vector3 offset = new Vector3(0, 0.3f, 0);
		Vector3 drawPos = transform.position + offset;

		//Debug.Log ("Drag: " + dragForce);
		Debug.DrawLine (drawPos, (dragForce.x / 10000) * transform.right + drawPos, Color.red);
		Debug.DrawLine (drawPos, (dragForce.y / 10000) * transform.up + drawPos, Color.red);
		Debug.DrawLine (drawPos, (dragForce.z / 10000) * transform.forward + drawPos, Color.red);

		//Debug.Log ("Velocity: " + localVelocity);
		//Debug.Log ("True velocity: " + rb.velocity.magnitude);
		Debug.DrawLine (drawPos, (localVelocity.x / 100) * transform.right + drawPos, Color.white);
		Debug.DrawLine (drawPos, (localVelocity.y / 100) * transform.up + drawPos, Color.white);
		Debug.DrawLine (drawPos, (localVelocity.z / 100) * transform.forward + drawPos, Color.white);
	}
}
