using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MeasureBar : MonoBehaviour {

	public float scale = 60000;
	public Text valueString;
	public Image fillBar;

	private float value;
	private float percentage;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		value = float.Parse(valueString.text);

		percentage = value / scale;

		if (percentage < 0) {
			percentage *= -1;
			fillBar.color = Color.red;
		}
		else {
			fillBar.color = Color.white;
		}

		fillBar.fillAmount = percentage;
	}
}
