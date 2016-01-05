using UnityEngine;
using System.Collections;

public class WingBehaviour : MonoBehaviour {

	[Header("Airplane")]
	public GameObject airplane;

	[Header("Lift")]
	public float liftMultiplier = 1;
	public float liftCoefficient = 6;
	public float criticalAngleOfAttack = 15;
	public float angleOffset = 0;
	public Transform liftPosition;
	public AirSpeedSensor speedSensorObj;

	[Header("Drag")]
	public float frontDragCoefficient = 1;
	public float sideDragCoefficient = 1;
	public float upDragCoefficient = 1;

	private Rigidbody rb;
	private Vector3 airSpeed;
	private float angleOfAttack;
	private float angleCoefficient;
	private Vector3 liftForce;
	private Vector3 dragForce;

	// Use this for initialization
	void Start () {
		rb = airplane.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		airSpeed = speedSensorObj.AirSpeed();
		angleOfAttack = CalculateAngleOfAttack();
		angleCoefficient = CalculateAngleCoefficient();
		liftForce = CalculateLift();
		dragForce = CalculateParasiticDrag();

		ApplyForces();
	}

	private float CalculateAngleOfAttack () {
		float angleOfAttack;
		float angleSign;
		Vector3 crossProduct;

		//Project velocity vector into wing forward-up plane
		Vector3 planeNormal = Vector3.Cross(transform.up, transform.forward);
		planeNormal.Normalize();
		float distance = -Vector3.Dot(planeNormal, rb.velocity);
		Vector3 projectedVelocity = rb.velocity + planeNormal * distance;

		//Calculate angle between projected velocity vector and object forward
		angleOfAttack = Vector3.Angle(transform.forward, projectedVelocity);

		//Check if angle is positive or negative
		crossProduct = Vector3.Cross(transform.forward, projectedVelocity);
		angleSign = Mathf.Sign(Vector3.Dot(transform.right, crossProduct));

		//Apply angle
		angleOfAttack *= angleSign;
		angleOfAttack += angleOffset;

//		Debug.DrawLine(liftPosition.transform.position, liftPosition.transform.position + projectedVelocity.normalized * 5, Color.blue);
//		Debug.DrawLine(liftPosition.transform.position, liftPosition.transform.position + transform.forward * 5, Color.white);
//		Debug.DrawLine(liftPosition.transform.position, liftPosition.transform.position + crossProduct.normalized * 5, Color.yellow);
		return angleOfAttack;
	}

	private float CalculateAngleCoefficient () {

//		if (useFlap) {
//			criticalAngleOfAttack -= flapCriticalAoAOffset / 2;
//			angleCoefficient = (angleOfAttack + flapCriticalAoAOffset + 5) * 0.07f;
//		}
//		else {
//			angleCoefficient = (angleOfAttack + 5)* 0.07f;
//		}

		angleCoefficient = (angleOfAttack + 5)* 0.07f;

		if (angleOfAttack > criticalAngleOfAttack) {
			float excess = (angleOfAttack - criticalAngleOfAttack) * 0.07f;
			angleCoefficient -= 2 * excess;
			if (angleOfAttack > 25)
				angleCoefficient = 0;
		}
		if (angleOfAttack < -criticalAngleOfAttack) {
			float excess = (angleOfAttack + criticalAngleOfAttack) * 0.07f;
			angleCoefficient -= 2 * excess;
			if (angleOfAttack < -25)
				angleCoefficient = 0;
		}
		return angleCoefficient;
	}

	private Vector3 CalculateLift () {
		float lift;
		float inducedDrag;
		float dragCoefficient;
		Vector3 resultantForce;

		//Lift
		if (airSpeed.z >= 0)
			lift = Mathf.Pow(airSpeed.z, 2) * liftCoefficient * liftMultiplier * 0.5f * angleCoefficient;
		else
			lift = 0;

		if (lift > 80000)
			lift = 80000;

		//Parasitic drag
		dragCoefficient = (Mathf.Pow(angleOfAttack, 2) * 0.00024f) + 0.01f;
		inducedDrag = lift * dragCoefficient;

		//Resultant vector
		resultantForce = new Vector3(0, lift, -inducedDrag);

		return resultantForce;
	}

	private Vector3 CalculateParasiticDrag () {
		Vector3 parasiticDrag;

		parasiticDrag.z = Mathf.Pow(airSpeed.z, 2) * frontDragCoefficient * 0.5f;
		parasiticDrag.y = Mathf.Pow(airSpeed.y, 2) * upDragCoefficient * 0.5f;
		parasiticDrag.x = Mathf.Pow(airSpeed.x, 2) * frontDragCoefficient * 0.5f;


		if (airSpeed.z < 0) parasiticDrag.z *= -1;
		if (airSpeed.y < 0) parasiticDrag.y *= -1;
		if (airSpeed.x < 0) parasiticDrag.x *= -1;

		return parasiticDrag;
	}

	private void ApplyForces () {
//		Vector3 direction;
//		direction = Vector3.Cross(rb.velocity, transform.right);
//		direction.Normalize();
//
////		rb.AddForceAtPosition(liftForce.y * direction, liftPosition.position);
////		rb.AddForceAtPosition(liftForce.z * rb.velocity, liftPosition.position);
//
//		Debug.DrawLine(transform.position, transform.position + (liftForce.y * direction).normalized,Color.white);
//		Debug.DrawLine(transform.position, transform.position + (liftForce.z * rb.velocity).normalized, Color.blue);

		Quaternion rotation = Quaternion.LookRotation(rb.velocity, transform.up);
		Vector3 forceVector = rotation * liftForce;
		Debug.DrawLine(transform.position, transform.position + forceVector.normalized);

		rb.AddForceAtPosition(forceVector, liftPosition.position);
		rb.AddRelativeForce(dragForce);
	}
}
