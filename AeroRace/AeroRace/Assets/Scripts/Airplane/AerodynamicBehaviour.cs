using UnityEngine;
using System.Collections;

public class AerodynamicBehaviour : MonoBehaviour {

	[Header("Rigidbody")]
	public Rigidbody airplaneRigidbody;
	public Transform centerOfMass;

	[Header("Lift")]
	public float liftMultiplier = 1;
	public float liftCoefficient = 6;
	public float criticalAngleOfAttack = 15;
	public float maxFunctionalAoA = 25;
	public float perAngleBoost = 0.07f;
	public float angleOffset = 0;
	public Transform liftPosition;
	public AirSpeedSensor speedSensorObj;

	[Header("Wing Control surface")]
	public float cSurfaceLiftCoefficient = 2;
	public float cSurfaceLiftMultiplier = 1;

	[Header("Drag")]
	public float frontDragCoefficient = 1;
	public float sideDragCoefficient = 1;
	public float upDragCoefficient = 1;

	private bool canGenerateLift;

	private float angleOfAttack;
	private float angleCoefficient;
	private float controlSurfaceInput;

	private Vector3 airSpeed;
	private Vector3 liftForce;
	private Vector3 cSurfaceLiftForce;
	private Vector3 totalLiftForce;
	private Vector3 dragForce;

	// Use this for initialization
	void Start () {
		if (liftPosition)
			canGenerateLift = true;
		else
			canGenerateLift = false;

		if (centerOfMass)
			SetCenterOfMass(centerOfMass);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate () {
		airSpeed = speedSensorObj.AirSpeed();
		angleOfAttack = CalculateAngleOfAttack();
		angleCoefficient = CalculateAngleCoefficient();

		if (canGenerateLift)
			liftForce = CalculateLift(liftCoefficient, liftMultiplier);
		else
			liftForce = Vector3.zero;
		cSurfaceLiftForce = CalculateLift(cSurfaceLiftCoefficient, cSurfaceLiftMultiplier);
		dragForce = CalculateParasiticDrag();

		ApplyForces();
	}

	private void SetCenterOfMass (Transform centerOfMass) {
		airplaneRigidbody.centerOfMass = centerOfMass.localPosition;
	}

	private float CalculateAngleOfAttack () {
		float angleOfAttack;
		float angleSign;
		Vector3 crossProduct;

		//Project velocity vector into wing forward-up plane
		Vector3 planeNormal = Vector3.Cross(transform.up, transform.forward);
		planeNormal.Normalize();
		float distance = -Vector3.Dot(planeNormal, airplaneRigidbody.velocity);
		Vector3 projectedVelocity = airplaneRigidbody.velocity + planeNormal * distance;

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

		angleCoefficient = (angleOfAttack + 5)* perAngleBoost;

		if (angleOfAttack > criticalAngleOfAttack) {
			float excess = (angleOfAttack - criticalAngleOfAttack) * perAngleBoost;
			angleCoefficient -= 2 * excess;
			if (angleOfAttack > maxFunctionalAoA)
				angleCoefficient = 0;
		}
		if (angleOfAttack < -criticalAngleOfAttack) {
			float excess = (angleOfAttack + criticalAngleOfAttack) * perAngleBoost;
			angleCoefficient -= 2 * excess;
			if (angleOfAttack < -maxFunctionalAoA)
				angleCoefficient = 0;
		}
		return angleCoefficient;
	}

	private Vector3 CalculateLift (float coefficient, float multiplier) {
		float lift;
		float inducedDrag;
		float dragCoefficient;
		Vector3 resultantForce;

		//Lift calculation. No lift is the plane is moving backwards
		if (airSpeed.z >= 0)
			lift = Mathf.Pow(airSpeed.z, 2) * coefficient * multiplier * 0.5f * angleCoefficient;
		else
			lift = 0;

		//Caps lift force to prevent crazy behaviour at high speeds because... shitty simulation
		if (lift > 80000)
			lift = 80000;

		//Induced drag
		dragCoefficient = (Mathf.Pow(angleOfAttack, 2) * 0.00024f) + 0.01f;
		inducedDrag = lift * dragCoefficient;

		//Resultant vector with lift and induced drag
		resultantForce = new Vector3(0, lift, -inducedDrag);

		return resultantForce;
	}

	private Vector3 CalculateParasiticDrag () {
		Vector3 parasiticDrag;

		//Get air drag for each direction, as each plane of the wing has a different area
		parasiticDrag.z = Mathf.Pow(airSpeed.z, 2) * frontDragCoefficient * 0.5f;
		parasiticDrag.y = Mathf.Pow(airSpeed.y, 2) * upDragCoefficient * 0.5f;
		parasiticDrag.x = Mathf.Pow(airSpeed.x, 2) * frontDragCoefficient * 0.5f;

		//Check if airspeed was negative and invert the direction if so. This is needed because Mathf.Pow always return a positive value
		if (airSpeed.z < 0) parasiticDrag.z *= -1;
		if (airSpeed.y < 0) parasiticDrag.y *= -1;
		if (airSpeed.x < 0) parasiticDrag.x *= -1;

		return parasiticDrag;
	}

	private void ApplyForces () {

		//Sum wing lift and control surface lift to get total lift force
		totalLiftForce = liftForce + (cSurfaceLiftForce * controlSurfaceInput);

		//Rotate lift force vector using wing up vector as reference
		Quaternion rotation = Quaternion.LookRotation(airplaneRigidbody.velocity, transform.up);
		Vector3 forceVector = rotation * totalLiftForce;
		Debug.DrawLine(transform.position, transform.position + forceVector.normalized);

		//Add forces to Rigidbody
		if(canGenerateLift)
			airplaneRigidbody.AddForceAtPosition(forceVector, liftPosition.position);
		airplaneRigidbody.AddRelativeForce(dragForce);
	}

	public void SetControlSurfaceInput (float input) {
		input = Mathf.Clamp(input, -1, 1);
		controlSurfaceInput = input;
	}
}
