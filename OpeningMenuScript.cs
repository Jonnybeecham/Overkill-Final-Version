using UnityEngine;
using System.Collections;

public class OpeningMenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartGameButtonPressed()
	{
		Application.LoadLevel ("GrasslandsBattle");
	}

	public void ExitGameButtonPressed()
	{
		Application.Quit ();
	}

	public void CreditsButtonPressed()
	{
	}
}
