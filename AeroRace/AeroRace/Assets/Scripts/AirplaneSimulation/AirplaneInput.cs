using UnityEngine;
using System.Collections;

public class AirplaneInput : MonoBehaviour {

	private float iVertical;
	public GameObject elevator01;
	public GameObject elevator02;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		iVertical = Input.GetAxis("Vertical");
	}

	void FixedUpdate () {
		elevator01.GetComponent<AirplanePiece>().rotateWing(iVertical);
		elevator02.GetComponent<AirplanePiece>().rotateWing(iVertical);
	}
}
