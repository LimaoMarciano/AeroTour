using UnityEngine;
using System.Collections;

public class WheelsBehaviour : MonoBehaviour {

	public WheelCollider frontLeftWheel;
	public WheelCollider frontRightWheel;
	public WheelCollider rearWheel;

	void Awake () {
		frontLeftWheel.motorTorque = 0.000001f;
		frontRightWheel.motorTorque = 0.000001f;
		rearWheel.motorTorque = 0.000001f;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
