using UnityEngine;
using System.Collections;

public class MainBattleMenuScript : MonoBehaviour 
{
	private CursorScript uiScript;
	
	// Use this for initialization
	void Start ()
	{
		uiScript = GameObject.Find ("Menu Cursor").GetComponent<CursorScript>();
		//Fade ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
	
	public void Fade()
	{
		gameObject.SetActive (false);
	}
	
	public void Appear()
	{
		gameObject.SetActive (true);
	}
	
	public void FightButtonClicked()
	{
		uiScript.ShiftToAttackMenu ();
		Fade ();
	}
	
	public void BattleItemsButtonClicked()
	{
		//Nothing yet
	}
	
	public void EscapeButtonClicked()
	{
		//Nothing yet
	}
}
