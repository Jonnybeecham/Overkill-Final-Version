using UnityEngine;
using System.Collections;

public class CircleActivationScript : MonoBehaviour 
{
	private bool isSet;
	private bool isActive;

	// Use this for initialization
	void Start () 
	{
		isSet = false;
		isActive = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isSet)
		{
			gameObject.renderer.material.color = Color.white;
		}
		else
		{
			gameObject.renderer.material.color = Color.gray;
		}
	}

	public void Fade()
	{
		isActive = false;
		gameObject.SetActive(false);
	}

	public void Appear(Vector3 targetPosition)
	{
		gameObject.SetActive (true);
		isActive = true;
		targetPosition.y = 2;
		transform.position = targetPosition;
	}

	public void TargetSet()
	{
		isSet = true;
	}

	public void Refresh()
	{
		isSet = false;
		Fade ();
	}

	public bool IsActive()
	{
		return isActive;
	}
}
