using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageBoxScript : MonoBehaviour 
{
	private List<GameObject> dBoxList;

	private string state;
	private int lifetime;
	private float offset;
	
	private int boxHeight;
	private int boxWidth;
	
	private List<Vector2> damageQuantities;
	private List<Vector2> healQuantities;
	
	private List<Transform> damageDrawPositions;
	private List<Transform> healDrawPositions;

	private GUIStyle textStyle;

	// Use this for initialization
	void Start () 
	{
		offset = 20.0f;
		state = "Idle";
		lifetime = 20;
		
		boxHeight = 40;
		boxWidth = 120;
		
		damageDrawPositions = new List<Transform>();
		healDrawPositions = new List<Transform>();
		damageQuantities = new List<Vector2>();
		healQuantities = new List<Vector2>();

		textStyle = new GUIStyle();
		textStyle.fontSize = 24;

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (state != "Idle")
		{
			if (healDrawPositions.Count <= 0 && damageDrawPositions.Count <= 0)
			{
				state = "Idle";
			}
			else
			{
				//remove "dead" damage boxes from lists
				int counter = 0;
				while (counter < damageDrawPositions.Count)
				{
					if (damageQuantities[counter].y <= 0)
					{
						damageDrawPositions.RemoveAt (counter);
						damageQuantities.RemoveAt (counter);
					}
					else
						counter++;
				}
				counter = 0;
				while (counter < healDrawPositions.Count)
				{
					if (healQuantities[counter].y <= 0)
					{
						healDrawPositions.RemoveAt (counter);
						healQuantities.RemoveAt (counter);
					}
					else
						counter++;
				}

				//decrement lifetimes of damage amounts
				for (int i = 0; i < damageDrawPositions.Count; i++)
				{
					Vector2 dqTemp = damageQuantities[i];
					dqTemp.y--;
					damageQuantities[i] = dqTemp;
				}
				
				for (int i = 0; i < healDrawPositions.Count; i++)
				{					
					Vector2 hqTemp = healQuantities[i];
					hqTemp.y--;
					healQuantities[i] = hqTemp;
				}
			}
		}
	}
	
	void OnGUI()
	{

		if (state == "Active")
		{
			GUI.contentColor = Color.red;
			for (int i = 0; i < damageDrawPositions.Count; i++)
			{
				Vector2 screenPos = ConvertToScreen(damageDrawPositions[i].position);
				screenPos.y -= Mathf.Round ((lifetime - damageQuantities[i].y)*15.0f*Time.deltaTime);
				GUI.Label (new Rect(screenPos.x, screenPos.y, screenPos.x + boxWidth, screenPos.y + boxHeight), "<size=24><b>"+damageQuantities[i].x.ToString()+"</b></size>");
			}
			
			GUI.contentColor = Color.green;
			for (int i = 0; i < healDrawPositions.Count; i++)
			{
				Vector2 screenPos = ConvertToScreen(healDrawPositions[i].position);
				screenPos.y -= Mathf.Round ((lifetime - healQuantities[i].y)*15.0f*Time.deltaTime);
				GUI.Label (new Rect(screenPos.x, screenPos.y, screenPos.x + boxWidth, screenPos.y + boxHeight), "<size=24><b>"+healQuantities[i].x.ToString()+"</b></size>");
			}
		}
	}

	private Vector2 ConvertToScreen(Vector3 worldPosition)
	{
		//converts a target's world position to a screen position for easy viewing
		Vector2 screenPosition = Vector2.zero;
		Vector3 screenPos3d = Camera.main.WorldToScreenPoint(worldPosition);
		screenPos3d.y = Camera.main.pixelHeight - screenPos3d.y;
		
		screenPosition.x = screenPos3d.x;
		screenPosition.y = screenPos3d.y - (float)offset;
		
		return screenPosition;
	}


	//Players and enemies call these in their respective scripts' TakeDamage() methods
	public void DisplayDamage(int damage, Transform recipientPosition)
	{
		Vector2 dmgAndTtlTuple = Vector2.zero;
		dmgAndTtlTuple.x = damage;
		dmgAndTtlTuple.y = lifetime;
		damageQuantities.Add(dmgAndTtlTuple);

		damageDrawPositions.Add(recipientPosition);
		
		if (state == "Idle")
			state = "Active";
	}
	
	public void DisplayHeal(int damage, Transform recipientPosition)
	{
		Vector2 dmgAndTtlTuple = Vector2.zero;
		dmgAndTtlTuple.x = damage;
		dmgAndTtlTuple.y = lifetime;
		healQuantities.Add(dmgAndTtlTuple);

		healDrawPositions.Add(recipientPosition);
		
		if (state == "Idle")
			state = "Active";
	}
}
