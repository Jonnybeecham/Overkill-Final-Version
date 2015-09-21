using UnityEngine;
using System.Collections;

public class BattleSkillSelectPanelScript : MonoBehaviour 
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
		if (Input.GetButtonDown ("Cancel"))
		{
			BackToMenuButtonClicked ();
		}
	}
	
	public void MeleeButtonClicked()
	{
		uiScript.SetTurnChoice ("Melee");
	}
	
	public void RangedAllButtonClicked()
	{
		uiScript.SetTurnChoice ("Ranged (All)");
	}
	
	public void RangedSingleButtonClicked()
	{
		uiScript.SetTurnChoice ("Ranged (Single)");
	}

	public void MagicButtonClicked()
	{
		uiScript.ShiftToMagicAttacksMenu();
	}

	public void SwdTechButtonClicked()
	{
		uiScript.ShiftToTechniqueMenu();
	}

	public void BackToMenuButtonClicked()
	{
		uiScript.DisplayMainBattleMenu ();
	}

	public void Fade()
	{
		gameObject.SetActive (false);
	}
	
	public void Appear()
	{
		gameObject.SetActive (true);
	}
}
