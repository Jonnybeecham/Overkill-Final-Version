using UnityEngine;
using System.Collections;

public class MagicSkillSelectMenuScript : MonoBehaviour 
{
	private CursorScript uiScript;
	private int backCooldown;
	
	// Use this for initialization
	void Start ()
	{
		uiScript = GameObject.Find ("Menu Cursor").GetComponent<CursorScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetButtonDown ("Cancel"))
		{
			BackButtonClicked ();
		}
	}

	public void Fade()
	{
		gameObject.SetActive (false);
	}
	
	public void Appear()
	{
		gameObject.SetActive (true);
	}

	public void HealButtonClicked()
	{
		uiScript.SetTurnChoice ("Heal");
	}
	
	public void FlameButtonClicked()
	{
		uiScript.SetTurnChoice("Spacetime Friction");
	}

	public void LightningButtonClicked()
	{
		uiScript.SetTurnChoice("Lorentz Blast");
	}
	
	public void IceButtonClicked()
	{
		uiScript.SetTurnChoice("Nanodampen");
	}

	public void SlayerDescentButtonClicked()
	{
		uiScript.SetTurnChoice("Slayer's Descent");
	}
	
	public void BladeRushButtonClicked()
	{
		uiScript.SetTurnChoice ("Blade Rush");
	}
	
	public void HurricaneSlashButtonClicked()
	{
		uiScript.SetTurnChoice("Hurricane Slash");
	}
	
	public void WorldSlayerButtonClicked()
	{
		uiScript.SetTurnChoice("Worldslayer");
	}

	public void BackButtonClicked()
	{
		uiScript.ShiftToAttackMenu ();
	}
}
