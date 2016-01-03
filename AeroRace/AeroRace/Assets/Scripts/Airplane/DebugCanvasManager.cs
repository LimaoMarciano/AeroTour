using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugCanvasManager : MonoBehaviour {

	public AirplaneBehaviour airplane;
	[Header("Left Wing")]
	public Text leftWingLift;
	public Text leftWingSpeedX;
	public Text leftWingSpeedY;
	public Text leftWingSpeedZ;
	public Text leftWingAoA;
	public Text leftWingcoefficient;

	[Header("Right Wing")]
	public Text rightWingLift;
	public Text rightWingSpeedX;
	public Text rightWingSpeedY;
	public Text rightWingSpeedZ;
	public Text rightWingAoA;
	public Text rightWingcoefficient;

	[Header("Tail wing")]
	public Text tailWingLift;
	public Text tailWingSpeedX;
	public Text tailWingSpeedY;
	public Text tailWingSpeedZ;
	public Text tailWingAoA;
	public Text tailWingcoefficient;

	[Header("Drag")]
	public Text frontDrag;
	public Text upDrag;
	public Text sideDrag;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		rightWingLift.text = airplane.GetLiftForce()[0].ToString("0.0");
		leftWingLift.text = airplane.GetLiftForce()[1].ToString("0.0");
		tailWingLift.text = airplane.GetLiftForce()[2].ToString("0.0");

		sideDrag.text = airplane.GetDragForce()[0].ToString("0.0");
		upDrag.text = airplane.GetDragForce()[1].ToString("0.0");
		frontDrag.text = airplane.GetDragForce()[2].ToString("0.0");

		leftWingSpeedX.text = airplane.GetAirSpeed()[0].x.ToString("0.0");
		leftWingSpeedY.text = airplane.GetAirSpeed()[0].y.ToString("0.0");
		leftWingSpeedZ.text = airplane.GetAirSpeed()[0].z.ToString("0.0");

		rightWingSpeedX.text = airplane.GetAirSpeed()[1].x.ToString("0.0");
		rightWingSpeedY.text = airplane.GetAirSpeed()[1].y.ToString("0.0");
		rightWingSpeedZ.text = airplane.GetAirSpeed()[1].z.ToString("0.0");

		tailWingSpeedX.text = airplane.GetAirSpeed()[2].x.ToString("0.0");
		tailWingSpeedY.text = airplane.GetAirSpeed()[2].y.ToString("0.0");
		tailWingSpeedZ.text = airplane.GetAirSpeed()[2].z.ToString("0.0");

		leftWingAoA.text = airplane.GetAngleOfAttack()[0].ToString("0");
		rightWingAoA.text = airplane.GetAngleOfAttack()[1].ToString("0");
		tailWingAoA.text = airplane.GetAngleOfAttack()[2].ToString("0");

		leftWingcoefficient.text = airplane.GetAngleCoefficients()[0].ToString("#.##");
		rightWingcoefficient.text = airplane.GetAngleCoefficients()[1].ToString("#.##");
		tailWingcoefficient.text = airplane.GetAngleCoefficients()[2].ToString("#.##");
	}
}
