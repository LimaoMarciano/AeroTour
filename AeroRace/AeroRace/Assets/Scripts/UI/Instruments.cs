using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Instruments : MonoBehaviour {

	public AirplaneBehaviour airplane;
	public Text airSpeed;
	public Text altimeter;
	public Image engineBar;
	public Image flapsLight;
	public Image stallLight;
	public Image lowAltitudeLight;
	public Image stallSpeedLight;

	private float airSpeedMeters;
	private float airSpeedKnots;
	private float altitude;
	private float thrustLevel;
	private bool isFlapOn;
	private Vector3 angleOfAttack;

	private float stallTimer = 0;
	private float stallSpeedTimer = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Air speed
//		airSpeedMeters = airplane.GetAirSpeed()[0].z;
//		airSpeedKnots = airSpeedMeters * 1.943f;
//		airSpeed.text = airSpeedKnots.ToString("#");
//
//		//Altimeter
//		altitude = airplane.transform.position.y * 3.28f;
//		altimeter.text = altitude.ToString("#");
//
//		//Thrust level
//		thrustLevel = airplane.GetEngineLevel();
//		engineBar.fillAmount = thrustLevel;
//
//		//Flap
//		isFlapOn = airplane.GetFlapsStatus();
//		if (isFlapOn)
//			flapsLight.color = new Color(0,1,0,0.8f);
//		else
//			flapsLight.color = new Color(0,0,0,0.5f);
//
//		//Stall
//		angleOfAttack.x = airplane.GetAngleOfAttack()[0];
//		angleOfAttack.y = airplane.GetAngleOfAttack()[1];
//		angleOfAttack.z = airplane.GetAngleOfAttack()[2];
//
//		if (angleOfAttack.x > 12 || angleOfAttack.y > 12 || angleOfAttack.z > 12 || angleOfAttack.x < -12 || angleOfAttack.y < -12 || angleOfAttack.z < -12) {
//			stallTimer += 3 * Time.deltaTime;
//			if (stallTimer > 0)
//				stallLight.color = new Color(1,0,0,0.8f);
//			if (stallTimer > 1)
//				stallLight.color = new Color(0,0,0,0.5f);
//			if (stallTimer > 2)
//				stallTimer = 0;
//		}
//		else {
//			stallLight.color = new Color(0,0,0,0.5f);
//			stallTimer = 0;
//		}
//
//		//Stall speed warning
//		if (airSpeedKnots < 90) {
//			stallSpeedTimer += 3 * Time.deltaTime;
//			if (stallSpeedTimer > 0)
//				stallSpeedLight.color = new Color(1,0,0,0.8f);
//			if (stallSpeedTimer > 1)
//				stallSpeedLight.color = new Color(0,0,0,0.5f);
//			if (stallSpeedTimer > 2)
//				stallSpeedTimer = 0;
//		}
//
//		//Low altitude
//		if (altitude < 500) 
//			lowAltitudeLight.color = new Color(1,0,0,0.8f);
//		else
//			lowAltitudeLight.color = new Color(0,0,0,0.5f);
	}
}
