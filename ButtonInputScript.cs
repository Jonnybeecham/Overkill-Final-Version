using UnityEngine;
using System.Collections;

public class ButtonInputScript : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
			if (Input.GetButtonDown ("Click") && !Input.GetButtonDown ("Cancel"))
			{
				SendMessage("OnClick");
			}
		}
}

