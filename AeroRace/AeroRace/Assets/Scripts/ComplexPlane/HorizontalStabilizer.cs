using UnityEngine;
using System.Collections;

public class HorizontalStabilizer : MonoBehaviour {

	public GameObject airplane;
	public GameObject liftPosition;
	private Rigidbody rb;
	private AirplaneManager am;

	private float elevatorAngle = 0;
	private float elevatorAngleOffset = 0;
	private float maxElevatorAngle = 20;
	public float liftPerElevatorAngle = 30;
	public float elevatorEfficientSpeed = 50;
	public float liftPerSpeed = 10;
	private float wingLift;

	private float elevatorLift;

	// Use this for initialization
	void Start () {
		rb = airplane.GetComponent<Rigidbody>();
		am = airplane.GetComponent<AirplaneManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		CalculateWingLift ();
		CalculateElevatorLift();
	}

	void LateUpdate () {
		DebugForceLines ();
	}

	private void CalculateWingLift () {

		wingLift = liftPerSpeed * am.airForwardVelocity;
		rb.AddForceAtPosition(transform.up * wingLift, liftPosition.transform.position);
		//		Debug.Log("Lift: " + wingLift);
	}

	private void CalculateElevatorLift () {
		float elevatorEfficiency;
		elevatorEfficiency = am.airForwardVelocity / elevatorEfficientSpeed;
		elevatorLift = elevatorAngle * liftPerElevatorAngle * elevatorEfficiency;

		rb.AddForceAtPosition (transform.up * elevatorLift, liftPosition.transform.position);
	}

	public void AdjustElevatorAngle (float input) {
		elevatorAngle = (maxElevatorAngle * input) + elevatorAngleOffset;
	}

	private void DebugForceLines () {
		Vector3 elevatorOffset = new Vector3 (0, 0, 0f);

		Debug.DrawLine(liftPosition.transform.position, transform.up * (elevatorLift / 500) + liftPosition.transform.position, Color.yellow);
		Debug.DrawLine(liftPosition.transform.position, -transform.forward * (am.airForwardVelocity / 100) + liftPosition.transform.position, Color.blue);
	}
}
