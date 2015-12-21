using UnityEngine;
using System.Collections;

public class VerticalStabilizer : MonoBehaviour {

	public GameObject airplane;
	public GameObject liftPosition;
	private AirplaneManager am;
	private Rigidbody rb;

	public float maxRudderAngle = 20;
	public float liftPerRudderAngle = 50;
	public float rudderEfficientSpeed = 50;
	public float rudderAngle = 0;

	private float rudderLift;

	// Use this for initialization
	void Start () {
		am = airplane.GetComponent<AirplaneManager> ();
		rb = airplane.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		CalculateRudderLift ();

	}

	void LateUpdate () {
		DebugForceLines ();
	}

	private void CalculateRudderLift () {
		float rudderEfficiency;
		rudderEfficiency = am.airForwardVelocity / rudderEfficientSpeed;
		rudderLift = liftPerRudderAngle * rudderAngle * rudderEfficiency;
		rb.AddForceAtPosition(transform.right * rudderLift, liftPosition.transform.position);
	}

	public void AdjustRudderAngle(float input) {
		rudderAngle = maxRudderAngle * input;
	}

	private void DebugForceLines () {
		Vector3 rudderOffset = new Vector3 (0, 0, 0f);

		Debug.DrawLine(liftPosition.transform.position, transform.right * (rudderLift / 500) + liftPosition.transform.position, Color.yellow);
		Debug.DrawLine(liftPosition.transform.position, -transform.forward * (am.airForwardVelocity / 100) + liftPosition.transform.position, Color.blue);
	}
}
