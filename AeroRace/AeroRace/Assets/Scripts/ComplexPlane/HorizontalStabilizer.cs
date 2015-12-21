using UnityEngine;
using System.Collections;

public class HorizontalStabilizer : MonoBehaviour {

	public GameObject airplane;
	public GameObject liftPosition;
	private Rigidbody rb;
	private AirplaneManager am;

	private float elevatorAngle = 0;
	private float elevatorAngleOffset = 7;
	private float maxElevatorAngle = 20;
	public float liftPerElevatorAngle = 30;
	public float elevatorEfficientSpeed = 50;

	// Use this for initialization
	void Start () {
		rb = airplane.GetComponent<Rigidbody>();
		am = airplane.GetComponent<AirplaneManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		CalculateElevatorLift();
	}

	private void CalculateElevatorLift () {
		float elevatorEfficiency;
		float elevatorLift;
		elevatorEfficiency = am.airForwardVelocity / elevatorEfficientSpeed;
		elevatorLift = elevatorAngle * liftPerElevatorAngle * elevatorEfficiency;

		rb.AddForceAtPosition (transform.up * elevatorLift, liftPosition.transform.position);
	}

	public void AdjustElevatorAngle (float input) {
		elevatorAngle = (maxElevatorAngle * input) + elevatorAngleOffset;
	}
}
