using UnityEngine;
using System.Collections;

public class AirplaneControls : MonoBehaviour {

	public EngineBehaviour engine;
	public AerodynamicBehaviour leftWing;
	public AerodynamicBehaviour rightWing;
	public AerodynamicBehaviour tailWing;

	public float engineIncrementPerSecond = 0.5f;

	private float engineIncrement;
	private float hInput;
	private float vInput;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {

		hInput = Input.GetAxis("Horizontal");
		vInput = Input.GetAxis("Vertical");

		leftWing.SetControlSurfaceInput(hInput);
		rightWing.SetControlSurfaceInput(-hInput);
		tailWing.SetControlSurfaceInput(vInput);

		//Engine control
		if (Input.GetButton("IncreaseSpeed"))
			ChangeThrustInput(engineIncrementPerSecond);
		if (Input.GetButton("DecreaseSpeed"))
			ChangeThrustInput(-engineIncrementPerSecond);

//		if (Input.GetButtonDown("Flap")) {
//			airplane.ToggleFlaps();
//		}
	}

	void ChangeThrustInput (float increment) {
		float engineLevel;
		float engineInput;

		engineLevel = engine.GetEngineLevel();
		engineInput = engineLevel + (increment * Time.deltaTime);
		Debug.Log(engineInput);

		engine.SetEngineLevel(engineInput);
	}
		
}
