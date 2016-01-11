using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Airspeed : MonoBehaviour {

	public AerodynamicBehaviour airplane;
	public Image needle;
	[Header("Scale")]
	public float minScale = 0;
	public float maxScale = 270;
	[Header("Needle angle")]
	public float minAngle = 0;
	public float maxAngle = 360;

	private float airspeed;
	private float degrees;
	private Vector3 rotation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		airspeed = airplane.GetAirSpeed().z;
		airspeed = airspeed * 1.943f;

		degrees = ((maxAngle / maxScale) * airspeed) + minAngle;
		degrees = Mathf.Clamp(degrees, minAngle, maxAngle);

		rotation = new Vector3 (0, 0, -degrees);
		needle.rectTransform.localRotation = Quaternion.Euler(rotation);
	}
}
