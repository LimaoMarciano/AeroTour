using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnalogIndicator : MonoBehaviour {

	public AerodynamicBehaviour airplane;
	public float value;
	[Header("Needle")]
	public Image needle;
	public bool isDirectionInverted = true;
	public bool isNeedleLimited = false;
	public float needleMaxScale = 270;
	[Header("Scale")]
	public float minScale = -5;
	public float maxScale = 25;
	[Header("Needle range")]
	public float minAngle = -30;
	public float maxAngle = 30;

	private float angleOfAttack;
	private float degrees;
	private Vector3 rotation;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
//		angleOfAttack = airplane.GetAngleOfAttack();

		if (isNeedleLimited)
			value = Mathf.Clamp(value, minScale, needleMaxScale);

		degrees = (((maxAngle - minAngle) / (maxScale - minScale)) * value) + minAngle;

		if (isNeedleLimited)
			degrees = Mathf.Clamp(degrees, minAngle, maxAngle);

		if (isDirectionInverted)
			degrees *= -1;

		rotation = new Vector3 (0, 0, degrees);
		needle.rectTransform.localRotation = Quaternion.Euler(rotation);
	}
}
