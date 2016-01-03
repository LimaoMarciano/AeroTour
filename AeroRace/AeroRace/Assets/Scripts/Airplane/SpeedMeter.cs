using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpeedMeter : MonoBehaviour {

	public GameObject airplaneObject;
	private AirplaneManager am;
	private Text speedometer;

	// Use this for initialization
	void Start () {
		am = airplaneObject.GetComponent<AirplaneManager>();
		speedometer = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		speedometer.text = am.airForwardVelocity.ToString();
	}
}
