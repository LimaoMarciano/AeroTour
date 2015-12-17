using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugUI : MonoBehaviour {

	public GameObject airplane;
	public Text frontAirSpeed;
	public Text upAirSpeed;
	public Text rightAirSpeed;

	private AeroplaneBehavior airplaneComponent;

	// Use this for initialization
	void Start () {
		airplaneComponent = airplane.GetComponent<AeroplaneBehavior> ();
	}
	
	// Update is called once per frame
	void Update () {
		frontAirSpeed.text = airplaneComponent.airForwardVelocity.ToString();
		upAirSpeed.text = airplaneComponent.airUpVelocity.ToString();
		rightAirSpeed.text = airplaneComponent.airRightVelocity.ToString();
	}
}
