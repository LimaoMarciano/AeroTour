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

	void LateUpdate () {
		Debug.DrawLine (transform.position, transform.position + (transform.forward * airSpeed.z) / 50, Color.blue);
		Debug.DrawLine (transform.position, transform.position + (transform.up * airSpeed.y) / 50, Color.blue);
		Debug.DrawLine (transform.position, transform.position + (transform.right * airSpeed.x) / 50, Color.blue);
		Debug.DrawLine (transform.position, transform.position + transform.TransformDirection(airSpeed) / 50, Color.white);
	}

	private void CalculateAirSpeed () {
		airSpeed = (transform.position - lastPosition) / Time.fixedDeltaTime;
		lastPosition = transform.position;
		airSpeed = transform.InverseTransformDirection(airSpeed);

	}

	public Vector3 AirSpeed () {
		return airSpeed;
	}
}
