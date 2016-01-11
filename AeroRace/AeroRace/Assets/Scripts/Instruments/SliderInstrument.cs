using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderInstrument : MonoBehaviour {

	public Image needle;
	public float value;
	[Header("Scale")]
	public float minScale = -1;
	public float maxScale = 1;
	[Header("Needle range")]
	public float minPosition = -10;
	public float maxPosition = 10;

	private float position;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		position = (((maxPosition - minPosition) / (maxScale - minScale)) * value);
		position = Mathf.Clamp(position, minPosition, maxPosition);

		needle.rectTransform.localPosition = new Vector3(position, 0, 0);
	}
}
