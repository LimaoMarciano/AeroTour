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

	private void CalculateWingLift () {

		float currentLift = liftPerSpeed * am.airForwardVelocity;
		rb.AddForceAtPosition(transform.up * currentLift, liftPosition.transform.position);
		Debug.Log("Lift: " + currentLift);
	}

	private void CalculateAileronLift () {
		float aileronEfficiency;
		float aileronLift; 
		aileronEfficiency = am.airForwardVelocity / aileronEfficientSpeed;
		aileronLift = liftPerAileronAngle * aileronAngle * aileronEfficiency;
		rb.AddForceAtPosition(transform.up * aileronLift, liftPosition.transform.position);
	}

	public void adjustAileronAngle(float input) {
		aileronAngle = maxAileronAngle * input;
	}
}
