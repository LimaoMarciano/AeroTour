using UnityEngine;
using System.Collections;

public class AirplaneManager : MonoBehaviour {

	public GameObject leftWingObject;
	public GameObject rightWingObject;
	private WingBehaviour leftWing;
	private WingBehaviour rightWing;

	public float thrustPower = 10000;
	public float elevatorPower = 2000;
	public float aileronsPower = 2000;
	public float aileronRotateSpeed = 0.05f;

	private Rigidbody rb;
	private float iHorizontal;
	private float iVertical;
	private float thrustLevel = 0;

	public float airForwardVelocity;
	public float airUpVelocity;
	public float airRightVelocity;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		leftWing = leftWingObject.GetComponent<WingBehaviour>();
		rightWing = rightWingObject.GetComponent<WingBehaviour>();
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


	}

	void FixedUpdate () {
		AirResistence();
		calculateAirSpeed();
		Elevator(iVertical);
		leftWing.adjustAileronAngle(iHorizontal);
		rightWing.adjustAileronAngle(-iHorizontal);
		Thruster(thrustLevel);
	}

	private void calculateAirSpeed() {
		airForwardVelocity = Vector3.Dot(rb.velocity, transform.forward);
		airUpVelocity = Vector3.Dot(rb.velocity, transform.up);
		airRightVelocity = Vector3.Dot(rb.velocity, transform.right);
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
}
