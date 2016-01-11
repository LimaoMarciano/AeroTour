using UnityEngine;
using System.Collections;

public class InstrumentsManager : MonoBehaviour {

	public AerodynamicBehaviour airplane;
	public AnalogIndicator airspeedIndicator;
	public AnalogIndicator angleOfAttackIndicator;
	public SliderInstrument slipIndicator;
	public AnalogIndicator altimeterThousandsIndicator;
	public AnalogIndicator altimeterHundresIndicator;
	public AnalogIndicator altimeterDecadesIndicator;

	private Vector3 altimeter;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		airspeedIndicator.value = CalculateAirSpeed();
		angleOfAttackIndicator.value = CalculateAngleOfAttack();
		slipIndicator.value = CalculateSlip();
		altimeter = CalculateAltitude();

		altimeterThousandsIndicator.value = altimeter.x;
		altimeterHundresIndicator.value = altimeter.y;
		altimeterDecadesIndicator.value = altimeter.z;
	}

	public float CalculateAirSpeed() {
		float airSpeed;

		airSpeed = airplane.GetAirSpeed().z;
		airSpeed = airSpeed * 1.943f;

		return airSpeed;
	}

	public float CalculateAngleOfAttack() {
		float angleOfAttack;
		if (airplane.GetAirSpeed().z > 0.1f)
			angleOfAttack = airplane.GetAngleOfAttack();
		else
			angleOfAttack = 0;

		return angleOfAttack;
	}

	public float CalculateSlip() {
		float sideAirSpeed;

		sideAirSpeed = airplane.GetAirSpeed().x;

		return sideAirSpeed;
	}

	public Vector3 CalculateAltitude() {
		float altitude;
		float thousands;
		float hundreds;
		float decades;
		Vector3 result;

		altitude = airplane.transform.position.y * 3.28f;
		thousands = altitude / 1000;
		hundreds = altitude / 100;
		decades = altitude / 10;
		result = new Vector3(thousands, hundreds, decades);

		return result;
	}
}
