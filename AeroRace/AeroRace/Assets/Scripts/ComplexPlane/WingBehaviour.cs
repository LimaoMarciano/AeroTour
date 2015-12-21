using UnityEngine;
using System.Collections;

public class WingBehaviour : MonoBehaviour {

	public GameObject Airplane;
	public GameObject liftPosition;
	public GameObject flapLiftPosition;
	public float liftPerSpeed = 45;
	public float maxAileronAngle = 20;
	public float liftPerAileronAngle = 50;
	public float aileronEfficientSpeed = 50;
	public float liftPerFlapAngle = 1f;
	public float dragPerFlapAngle = 1f;
	public float maxFlapAngle = 20;
	public float flapEfficientSpeed = 20;

	private Rigidbody rb;
	private AirplaneManager am;
	private float aileronAngle = 0;
	private float flapAngle = 0;

	private float aileronLift;
	private float wingLift;
	private float flapLift;
	private float flapDrag;

	// Use this for initialization
	void Start () {
		rb = Airplane.GetComponent<Rigidbody>();
		am = Airplane.GetComponent<AirplaneManager>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		CalculateWingLift ();
		CalculateAileronLift ();
		CalculateFlapLift ();

	}

	void LateUpdate () {
		DebugForceLines();
	}

	private void CalculateWingLift () {

		wingLift = liftPerSpeed * am.airForwardVelocity;
		rb.AddForceAtPosition(transform.up * wingLift, liftPosition.transform.position);
//		Debug.Log("Lift: " + wingLift);
	}

	private void CalculateAileronLift () {
		float aileronEfficiency;
		aileronEfficiency = am.airForwardVelocity / aileronEfficientSpeed;
		aileronLift = liftPerAileronAngle * aileronAngle * aileronEfficiency;
		rb.AddForceAtPosition(transform.up * aileronLift, liftPosition.transform.position);
	}

	private void CalculateFlapLift () {
		float flapEfficiency;
		Vector3 forceToApply;
		flapEfficiency = am.airForwardVelocity / flapEfficientSpeed;
		flapLift = liftPerFlapAngle * flapAngle * flapEfficiency;
		flapDrag = dragPerFlapAngle * flapAngle * flapEfficiency;
		forceToApply = new Vector3 ( 0, flapLift, -flapDrag);
		forceToApply = transform.InverseTransformDirection (forceToApply);
		rb.AddForceAtPosition (forceToApply, flapLiftPosition.transform.position);


	}

	public void adjustAileronAngle(float input) {
		aileronAngle = maxAileronAngle * input;
	}

	public void adjustFlapAngle(float input) {
		flapAngle = maxFlapAngle * input;
	}

	private void DebugForceLines () {
		Vector3 aileronOffset = new Vector3 (0.1f, 0, 0);

		Debug.DrawLine(liftPosition.transform.position, -transform.forward * (am.airForwardVelocity / 100) + liftPosition.transform.position, Color.blue);
		Debug.DrawLine(liftPosition.transform.position, transform.up * (wingLift / 1000) + liftPosition.transform.position, Color.green);
		Debug.DrawLine(liftPosition.transform.position + aileronOffset, transform.up * (aileronLift / 500) + liftPosition.transform.position + aileronOffset, Color.yellow);
	}
}
