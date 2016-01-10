using UnityEngine;
using System.Collections;

public class EngineBehaviour : MonoBehaviour {

	public Rigidbody airplaneRigidbody;
	public float thrustForce = 3000;
	public Transform thrustPosition;

	private float engineLevel = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		ApplyThrust();
	}

	private void ApplyThrust () {
		Vector3 thrust;
		thrust = engineLevel * thrustForce * transform.forward;
		airplaneRigidbody.AddForceAtPosition (thrust, thrustPosition.position);
	}

	public void SetEngineLevel (float input) {
		input = Mathf.Clamp01(input);
		engineLevel = input;
	}

	public float GetEngineLevel () {
		return engineLevel;
	}
}
