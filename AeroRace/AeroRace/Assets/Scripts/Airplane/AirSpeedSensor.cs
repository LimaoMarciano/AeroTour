using UnityEngine;
using System.Collections;

public class AirSpeedSensor : MonoBehaviour {

	public Vector3 airSpeed;
	private Vector3 lastPosition;

	// Use this for initialization
	void Start () {
		lastPosition = transform.position;
	}

	void FixedUpdate () {
		CalculateAirSpeed();
	}

	private void CalculateAirSpeed () {
		airSpeed = (transform.position - lastPosition) / Time.fixedDeltaTime;
		lastPosition = transform.position;
		airSpeed = transform.InverseTransformDirection (airSpeed);
	}

	public Vector3 AirSpeed () {
		return airSpeed;
	}
}
