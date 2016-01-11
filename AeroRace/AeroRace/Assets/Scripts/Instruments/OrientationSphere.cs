using UnityEngine;
using System.Collections;

public class OrientationSphere : MonoBehaviour {

	public AerodynamicBehaviour airplane;

	private Vector3 airplaneRotation;
	private Vector3 sphereRotation;
	private float angleOfAttack;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		airplaneRotation = airplane.transform.rotation.eulerAngles;
		sphereRotation = transform.localRotation.eulerAngles;
		angleOfAttack = airplane.GetAngleOfAttack();

		sphereRotation = new Vector3(airplaneRotation.x, transform.localRotation.y, -airplaneRotation.z);

		transform.localRotation = Quaternion.Euler(sphereRotation);
	}
}
