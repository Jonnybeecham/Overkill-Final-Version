using UnityEngine;
using System.Collections;

public class LightScript : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (transform.parent.GetComponent<PlayerScript>().lightColor == "Red") light.color = Color.red;
		else if (transform.parent.GetComponent<PlayerScript>().lightColor == "Green") light.color = Color.green;
		else if (transform.parent.GetComponent<PlayerScript>().lightColor == "Blue") light.color = Color.blue;
		else light.color = Color.white;
	}
}
