using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {
	public string state;
	public string nextTurnChoice;
	private bool successfullyAttacked;
	private Vector3 attackDestination;
	private Vector3 prevLocation;
	private PlayerScript attackTarget;
	private CursorScript uiScript;
	private EnemyAIScript aiScript;
	public DamageBoxScript dmgBox;
	private HealthScript hpScript;
	private EnemyScript healTarget;
	public float speed;
	public float health;
	public bool damagedThisTurn;
	public CharacterController enemyControl;
	public float attackDelay;
	public float attackTimer;
	public float healTimer;
	public bool startOfHeal;

	private float attackRotationAngle;
	public float maxHealth;

	public float mana;
	public float maxMana;

	private Quaternion naturalRotation;
	
	// Use this for initialization
	void Start () 
	{
		hpScript = gameObject.GetComponent<HealthScript>();
		damagedThisTurn = false;
		startOfHeal = true;
		state = "Standby";
		speed = 15.0f;
		
		attackDelay = 700.0f;
		attackTimer = attackDelay;
		nextTurnChoice = "";
		dmgBox = (DamageBoxScript) GameObject.Find ("DamageBox").GetComponent<DamageBoxScript>();
		uiScript = (CursorScript) GameObject.Find ("Menu Cursor").GetComponent<CursorScript>();
		aiScript = (EnemyAIScript) GameObject.Find ("Menu Cursor").GetComponent<EnemyAIScript>();
		maxHealth = GameObject.Find ("Menu Cursor").GetComponent<StatsScript>().GetMaxHP (gameObject.name);
		health = maxHealth;
		maxMana = GameObject.Find ("Menu Cursor").GetComponent<StatsScript>().GetMaxMP (gameObject.name);
		mana = maxMana;

		naturalRotation = transform.rotation;
	}
	
	public string GetState()
	{
		return state;
	}
	
	// Update is called once per frame
	
	void Update () 
	{
		CheckForDeath (); // if enemy health reaches zero he is killed
		CheckTimer();
		PerformTurn ();
		if(!animation.isPlaying) animation.Play ("Idle");
		hpScript.Health = health;
		if (state == "Standby" || state == "WaitingInQueue")
		{
			if (transform.rotation != naturalRotation)
				transform.rotation = naturalRotation;
		}
	}
	
	void CheckTimer()
	{
		if (attackTimer > 0)
		{
			attackTimer--;
		}
		else if (state != "WaitingInQueue" && state != "Attacking" && state != "Healing" && state != "Dead")
		{
			SelectAttackTarget();
			uiScript.EnqueueEnemy (this.gameObject);
			state = "WaitingInQueue";
			Debug.Log ("Enqueued "+this.name);
		}
	}
	
	public void CheckForDeath()
	{
		if (health <= 0)
		{
			state = "Dead";
			animation.Play ("Death");
			if (animation["Death"].normalizedTime >= 0.9)
			{
				uiScript.RemoveMe(gameObject.GetComponent<EnemyScript>());

				uiScript.WakeUp ();
				gameObject.SetActive(false);
				uiScript.deadEnemies.Add (this.GetComponent <EnemyScript>());
			}
		}
	}
	
	public void PerformTurn()
	{
		if (state == "Attacking")
		{
			if (successfullyAttacked)
			{
				MoveBackToStartPosition ();
			}
			else
			{
				MoveToAttack ();
			}
		}
		if (state == "Healing") {
			if(!successfullyAttacked){
				healTarget = aiScript.SelectHealTarget ();
				PerformHeal();
			}
		}
	}
	
	public void SelectAttackTarget()
	{
		float lowest = 250.0f;
		bool foundLowest = false;
		for (int i = 0; i < uiScript.hero.Count; i++) 
		{
			if (uiScript.hero[i].health < lowest)
			{
				//lowest = uiScript.hero[i].health;
				foundLowest = true;
				if (uiScript.hero[i].state != "Dead")
				{
					attackTarget = uiScript.hero[i];
					foundLowest = false;
				}
				attackDestination = uiScript.hero[i].transform.position;
			}
		}
		
		if (foundLowest == true) 
		{
			successfullyAttacked = false;
			prevLocation = transform.position;
		}
		else 
		{
			float maxRoll = uiScript.hero.Count;
			int randomTargetSubscript = (int) Random.Range(0, maxRoll);
			successfullyAttacked = false;
			prevLocation = transform.position;
			if (uiScript.hero[randomTargetSubscript].state != "Dead")
				attackTarget = uiScript.hero[randomTargetSubscript];
			else
				attackTarget = uiScript.hero[randomTargetSubscript*-1 + 1];
			attackDestination = uiScript.hero[randomTargetSubscript].transform.position;
		}
	}
	
	
	public void InitiateAttackSequence()
	{
		if (state != "Dead")
		{
			if (attackTarget.state == "Dead")
				SelectAttackTarget();
			successfullyAttacked = false;
			prevLocation = transform.position;
			if (gameObject.name == "skeletonMage" || gameObject.name == "skeletonSpearman")
			{
				state = "Attacking";
				attackTarget = uiScript.SelectRandomPlayer();
			}
			else
			{
				state = aiScript.MakeTurnChoice ();
			}
			
			attackDestination = attackTarget.transform.position;
			RotateTowardsTarget(attackTarget.transform.position);
		}

	}
	
	public void MoveToAttack()
	{
		if (Vector3.Distance (transform.position, attackDestination) < 5)
		{
			animation.Play("Melee");
			if(animation["Melee"].normalizedTime >= 0.9) PerformAttack();
		}
		else
		{
			animation.Play ("Run");
			transform.position = Vector3.MoveTowards (transform.position, attackDestination, speed*Time.deltaTime);
		}
	}
	
	public void PerformAttack()
	{
		successfullyAttacked = true;

		float dmgRoll = uiScript.GetStatsScript().MeleeDamageRoll(gameObject.name, attackTarget.name);
		attackTarget.TakeDamage (dmgRoll);

		Debug.Log ("Successfully damaged "+attackTarget.name);
		attackTimer = attackDelay;

		transform.Rotate (Vector3.up, 180, Space.World);
	}
	
	public void MoveBackToStartPosition()
	{
		animation.Play ("Run");
		transform.position = Vector3.MoveTowards (transform.position, prevLocation, speed*Time.deltaTime);
		if (Vector3.Distance (transform.position, prevLocation) <= 0)
		{
			//made it back to starting position
			transform.Rotate (Vector3.up, 180-attackRotationAngle, Space.World);
			state = "Standby";
			animation.Play ("Idle");
			uiScript.WakeUp ();
		}
	}
	
	public void TakeDamage(float amount)
	{
		health -= Mathf.RoundToInt (amount);
		if (health < 0)
			health = 0.0f;
		animation.Play ("Hit");
		dmgBox.DisplayDamage((int)amount, this.transform);
	}
	public void PerformHeal()
	{
		if (startOfHeal == true) {
			healTimer = 60;
			startOfHeal = false;
		}
		if (state == "Healing" && healTimer > 0)
		{
			gameObject.renderer.material.color = Color.green;
			healTimer--;
		}
		else
		{
			Debug.Log (this.name+" successfully healed.");
			successfullyAttacked = true;
			if(uiScript.enemy.Count > 0)
			{
				float healRoll = uiScript.GetStatsScript().HealRoll(gameObject.name);
				if (healTarget != null)
				{
					if (!healTarget.gameObject.activeSelf || healTarget.health <= 0)
						healTarget = this;
				}
				else
					healTarget = this;

				healTarget.ReceiveHeal(healRoll);
				attackTimer = attackDelay;
				startOfHeal = true;
				state = "Standby";
				uiScript.WakeUp ();
			}
			else if(uiScript.deadEnemies.Count > 0){
				uiScript.deadEnemies.Clear ();
				SelectAttackTarget();
				InitiateAttackSequence();
				state = "Attacking";
				MoveToAttack();
			}

		}
	}
	public void ReceiveHeal(float amt)
	{
		if (gameObject.name == "skeletonMage" || gameObject.name == "skeletonSpearman")
		{
			TakeDamage (amt);
		}
		else
		{
			if (health + amt > maxHealth)
			{
				health = maxHealth;
				
			}
			else
			{
				health += Mathf.Round (amt);
			}
			dmgBox.DisplayHeal((int)amt, this.transform);
		}
	}

	private void RotateTowardsTarget(Vector3 loc)
	{
		Vector3 dir = loc - transform.position;

		attackRotationAngle = Vector3.Angle (transform.forward, dir);
		transform.Rotate (Vector3.up, -attackRotationAngle, Space.World);
	}
}
