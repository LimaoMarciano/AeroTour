using UnityEngine;
using System.Collections;

public class WingBehaviour : MonoBehaviour {

	public GameObject Airplane;
	public GameObject liftPosition;
	public float liftPerSpeed = 45;
	public float maxAileronAngle = 20;
	public float liftPerAileronAngle = 50;
	public float aileronEfficientSpeed = 50;

	private Rigidbody rb;
	private AirplaneManager am;
	private float aileronAngle = 0;

	private float aileronLift;
	private float wingLift;

	// Use this for initialization
	void Start () {
		rb = Airplane.GetComponent<Rigidbody>();
		am = Airplane.GetComponent<AirplaneManager>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		CalculateWingLift ();
		CalculateAileronLift();

	}

	void LateUpdate () {
		DebugForceLines();
	}

	private void CalculateWingLift () {

		wingLift = liftPerSpeed * am.airForwardVelocity;
		rb.AddForceAtPosition(transform.up * wingLift, liftPosition.transform.position);
		Debug.Log("Lift: " + wingLift);
	}

	private void CalculateAileronLift () {
		float aileronEfficiency;
		aileronEfficiency = am.airForwardVelocity / aileronEfficientSpeed;
		aileronLift = liftPerAileronAngle * aileronAngle * aileronEfficiency;
		rb.AddForceAtPosition(transform.up * aileronLift, liftPosition.transform.position);
	}

	public void adjustAileronAngle(float input) {
		aileronAngle = maxAileronAngle * input;
	}

	private void DebugForceLines () {
		Vector3 aileronOffset = new Vector3 (0, 0, 0.05f);

		Debug.DrawLine(liftPosition.transform.position, transform.forward * (am.airForwardVelocity / 50) + liftPosition.transform.position, Color.blue);
		Debug.DrawLine(liftPosition.transform.position, transform.up * (wingLift / 1000) + liftPosition.transform.position, Color.green);
		Debug.DrawLine(liftPosition.transform.position + aileronOffset, transform.up * (aileronLift / 200) + liftPosition.transform.position + aileronOffset, Color.yellow);
	}
}
