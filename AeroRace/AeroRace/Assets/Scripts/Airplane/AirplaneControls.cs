using UnityEngine;
using System.Collections;

public class AirplaneControls : MonoBehaviour {

	public AirplaneBehaviour airplane;



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("IncreaseSpeed")) {
			airplane.ChangeThrustLevel(0.5f);
			Debug.Log("up");
		}
		if (Input.GetButton("DecreaseSpeed")) {
			airplane.ChangeThrustLevel(-0.5f);
			Debug.Log("down");
		}

		if (Input.GetButtonDown("Flap")) {
			airplane.ToggleFlaps();
		}
	}
}
