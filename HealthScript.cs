using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{	
	public float currentHealth;	
	public float currentMana;	
	public RectTransform healthTransform;
	public Text healthText;
	public Image visualHealth;
	public RectTransform manaTransform;
	public Image visualMana;
	private float cachedY;
	private float minXValue;
	private float maxXValue;
	private float currentXValue;
	private float cachedYMana;
	private float minXValueMana;
	private float maxXValueMana;
	private float currentXValueMana;
	private StatsScript playerScript;
	private float maxHealth;
	private float maxMana;
	private float goalPositionHealth;
	private float goalPositionMana;
	private string bar;

	public float currentTTA;
	private float maxTTA;
	public RectTransform TTATransform;
	public Text TTAText;
	public Image VisualTTA;
	private float cachedYTTA;
	private float minXValueTTA;
	private float maxXValueTTA;
	private float currentXValueTTA;
	private float goalPositionTTA;
	
	private const float inverseChangeSpeed = 10.0f;
	private float healthInterval;
	private float manaInterval;
	private float TTAInterval;

	// Use this for initialization
	void Start()
	{
		maxHealth = GameObject.Find ("Menu Cursor").GetComponent<StatsScript>().GetMaxHP (gameObject.name);
		maxMana = GameObject.Find ("Menu Cursor").GetComponent<StatsScript>().GetMaxMP (gameObject.name);

		if (this.gameObject.tag == "Player") 
		{
			maxTTA = gameObject.GetComponent<PlayerScript> ().GetAttackDelay();
		}
		else if (this.gameObject.tag == "Enemy") 
		{
			maxTTA = this.GetComponent<EnemyScript>().attackDelay;
		}
		//Sets all start values
		cachedY = healthTransform.position.y; //Caches the healthbar's start pos
		maxXValue = healthTransform.position.x; //The max value of the xPos is the start position
		minXValue = healthTransform.position.x - healthTransform.rect.width*healthTransform.lossyScale.x; //The minValue of the xPos is startPos - the width of the bar
		
		cachedYMana = manaTransform.position.y;
		maxXValueMana = manaTransform.position.x;
		minXValueMana = manaTransform.position.x - manaTransform.rect.width*manaTransform.lossyScale.x;
		maxXValueTTA = 0.0f;
		minXValueTTA = 0.0f;
		cachedYTTA = 0.0f;
		if (TTATransform != null)
		{
			cachedYTTA = TTATransform.position.y;
			maxXValueTTA = TTATransform.position.x;
			minXValueTTA = TTATransform.position.x - TTATransform.rect.width*TTATransform.lossyScale.x;
		}

		currentHealth = maxHealth; //Sets the current healt to the maxHealth
		currentMana = maxMana;
		currentTTA = maxTTA;

		currentXValueMana = maxXValueMana;
		currentXValue = maxXValue;
		currentXValueTTA = maxXValueTTA;
		
		goalPositionHealth = maxXValue;
		goalPositionMana = maxXValueMana;
		goalPositionTTA = maxXValueTTA;
		healthInterval = 1.0f;
		manaInterval = 1.0f;
		TTAInterval = 1.0f;
	}

	/// <summary>
	/// Property for accessing the players health
	/// </summary>
	public float Health
	{
		get { return currentHealth; }
		set
		{
			/*float counter = currentHealth;
			if (value < currentHealth)
			{
				while(counter != value)
				{
					currentHealth = counter;
					HandleHealthbar();
					counter -= 1;
				}
			}
			else
			{
				while(counter != value)
				{
					currentHealth = counter;
					HandleHealthbar();
					counter += 1;
				}
			}*/
			bar = "health";
			currentHealth = value;
			float barPosition = Map(currentHealth, 0, maxHealth, minXValue, maxXValue);
			float minPosition = Mathf.RoundToInt (barPosition);
			float maxPosition = Mathf.RoundToInt (barPosition) + 1.0f;
			
			goalPositionHealth = Mathf.Clamp (barPosition, minPosition, maxPosition);
			healthInterval = Mathf.Abs ((currentXValue-goalPositionHealth)/inverseChangeSpeed);

			HandleHealthbar();
		}
	}
	public float TTA
	{
		get { return currentTTA; }
		set
		{

			/*currentTTA = value;
			float barPosition = Map(currentTTA, 0, maxTTA, minXValueTTA, maxXValueTTA);
			float minPosition = Mathf.RoundToInt (barPosition);
			float maxPosition = Mathf.RoundToInt (barPosition) + 1.0f;
			
			goalPositionTTA = Mathf.Clamp (barPosition, minPosition, maxPosition);
			healthInterval = Mathf.Abs ((currentXValueTTA-goalPositionTTA)/inverseChangeSpeed);*/
			bar = "timeToAttack";
			currentTTA = value;
			currentXValueTTA = Map(maxTTA-currentTTA, 0, maxTTA, minXValueTTA, maxXValueTTA);
			HandleHealthbar();
		}
	}
	public float Mana
	{
		get { return currentMana; }
		set
		{
			bar = "mana";
			currentMana = value;
			float barPosition = Map(currentMana, 0, maxMana, minXValueMana, maxXValueMana);
			float minPosition = Mathf.RoundToInt (barPosition);
			float maxPosition = Mathf.RoundToInt (barPosition) + 1.0f;
			
			goalPositionMana = Mathf.Clamp (barPosition, minPosition, maxPosition);
			manaInterval = Mathf.Abs ((currentXValueMana-goalPositionMana)/inverseChangeSpeed);
			HandleHealthbar();
		}
	}

	void Update()
	{
		if (currentHealth <= 0 && gameObject.tag == "Enemy")
		{
			healthText.gameObject.transform.root.transform.root.gameObject.SetActive(false);
		}

		if (goalPositionHealth > currentXValue)
		{
			currentXValue += healthInterval;
		}
		else if (currentXValue > goalPositionHealth)
		{
			currentXValue -= healthInterval;
		}

		if (goalPositionMana > currentXValueMana)
		{
			currentXValueMana += manaInterval;
		}
		else if (goalPositionMana < currentXValueMana)
		{
			currentXValueMana -= manaInterval;
		}
		/*if (goalPositionTTA > currentXValueTTA)
		{
			currentXValueTTA += TTAInterval;
		}
		else if (goalPositionTTA < currentXValueTTA)
		{
			currentXValueTTA -= TTAInterval;
		}*/
	}
	
	/// <summary>
	/// Handles the healthbar by moving it and changing color
	/// </summary>
	private void HandleHealthbar()
	{   
		//Writes the current health in the text field
		if (gameObject.name == "Dark Troll")
			healthText.text = "Dark Troll";// + currentHealth;
		else if (gameObject.name == "Green Spider")
			healthText.text = "Green Spider";// + currentHealth;
		else if (gameObject.name == "skeletonMage")
			healthText.text = "Skelemage";
		else if (gameObject.name == "Zebulon")
			healthText.text = "Zebulon";// + currentHealth.ToString () + " / " + maxHealth.ToString ();
		else if (gameObject.name == "Zebulon (Clone)")
			healthText.text = "Darius";// + currentHealth.ToString () + " / " + maxHealth.ToString ();
		else if (gameObject.name == "skeletonSpearman")
			healthText.text = "Hell Lancer";
		
		
		//Maps the min and max position to the range between 0 and max health
		//currentXValue = Map(currentHealth, 0, maxHealth, minXValue, maxXValue);
		
		//Sets the position of the health to simulate reduction of health
		if (bar == "health")
		{
			healthTransform.position = new Vector3(currentXValue, cachedY);
			
			if (gameObject.tag == "Player")
			{
				if (currentHealth > maxHealth / 2) //If we have more than 50% health we use the Green colors
				{
					visualHealth.color = new Color32((byte)Map(currentHealth, maxHealth / 2,maxHealth, 255, 0), 255, 0, 255);
				}
				else //If we have less than 50% health we use the red colors
				{
					visualHealth.color = new Color32(255, (byte)Map(currentHealth, 0, maxHealth / 2, 0, 255), 0, 255);
				}
			}
		}
		
		else if (bar == "mana")
		{
			manaTransform.position = new Vector3(currentXValueMana, cachedYMana);
		
			if (gameObject.tag == "Player")
			{
				visualMana.color = new Color32(0, 0, 255, 255);
				/*if (currentMana > maxMana / 2) //If we have more than 50% health we use the Green colors
				{
					visualMana.color = new Color32(0, 255,(byte)Map(currentMana, maxMana / 2,maxMana, 255, 0), 255);
				}
				else //If we have less than 50% health we use the red colors
				{
					visualMana.color = new Color32(255, (byte)Map(currentMana, 0, maxMana / 2, 0, 255), 0, 255);
				}*/
			}
		}
		else if (bar == "timeToAttack" && TTATransform != null)
		{
			TTATransform.position = new Vector3(currentXValueTTA, cachedYTTA);
			
			if (gameObject.tag == "Player")
			{
				//visualMana.color = new Color32(255, 223, 0, 255);
				/*if (currentMana > maxMana / 2) //If we have more than 50% health we use the Green colors
				{
					visualMana.color = new Color32(0, 255,(byte)Map(currentMana, maxMana / 2,maxMana, 255, 0), 255);
				}
				else //If we have less than 50% health we use the red colors
				{
					visualMana.color = new Color32(255, (byte)Map(currentMana, 0, maxMana / 2, 0, 255), 0, 255);
				}*/
			}
		}
	}
	
	
	/// <summary>
	/// This method maps a range of number into another range
	/// </summary>
	/// <param name="x">The value to evaluate</param>
	/// <param name="in_min">The minimum value of the evaluated variable</param>
	/// <param name="in_max">The maximum value of the evaluated variable</param>
	/// <param name="out_min">The minum number we want to map to</param>
	/// <param name="out_max">The maximum number we want to map to</param>
	/// <returns></returns>
	public float Map(float x, float in_min, float in_max, float out_min, float out_max)
	{
		return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}
}
