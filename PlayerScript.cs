using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {
	private readonly Dictionary<string, float> manaCostDictionary = new Dictionary<string, float>()
	{
		{"Spacetime Friction", 35.0f}, 
		{"Nanodampen", 32.0f}, 
		{"Lorentz Blast", 39.0f},
		{"Worldslayer", 70.0f},
		{"Blade Rush", 25.0f},
		{"Hurricane Slash", 45.0f},
		{"Slayer's Descent", 48.0f}
	};
	private readonly Dictionary<string, string> zebulonAnimationNameDictionary = new Dictionary<string, string>()
	{
		{"Slash","Slash"},
		{"Dead","Dead"},
		{"AoE Start","AoE Start"},
		{"Aoe Done","Aoe Done"},
		{"Ranged Start","Ranged Start"},
		{"Ranged Done","Ranged Done"},
		{"Run", "Run"},
		{"Idol Taunt", "Idol Taunt"},
		{"Hit", "Hit"},
		{"Heal", "Heal"},
		{"Heal Done", "Heal Done"}
	};
	private readonly Dictionary<string, string> knightAnimationNameDictionary = new Dictionary<string, string>()
	{
		{"Slash","Slash"},
		{"Dead","Dead"},
		{"AoE Start","AoE Start"},
		{"Aoe Done","Aoe Done"},
		{"Ranged Start","Ranged Start"},
		{"Ranged Done","Ranged Done"},
		{"Run", "Run"},
		{"Idol Taunt", "Idol Taunt"},
		{"Hit", "Hit"},
		{"Heal", "Heal"},
		{"Heal Done", "Heal Done"}
	};

	public AudioClip fireballImpact;
	private bool usedMagicAttack;
	public float health;
	public CharacterController heroControl;
	private Vector3 prevLocation;
	public string state;
	private bool successfullyAttacked;
	private Vector3 attackDestination;
	public EnemyScript attackTarget;
	private PlayerScript healTarget;
	private EnemyScript enemyHealTarget;
	private CursorScript uiScript;
	private HealthScript hpScript;
	public float speed;
	public AudioClip battleEndClip;
	public float numberOfEnemies;
	public float blackTimer;
	public float healTimer;
	public DamageBoxScript dmgBox;
	public string actionChoice;
	private List<EnemyScript> targetsToDamage;
	public string lightColor;
	public GameObject fireball;
	public GameObject iceball;
	public GameObject lightning;
	public GameObject dmgLaser;
	public AudioClip slash;
	public AudioClip laser;
	public AudioClip fireballClip;
	public AudioClip iceClip;
	public AudioClip thunderClip;
	public AudioClip swordAttack1;
	public AudioClip swordAttack2;
	public AudioClip swordAttack3;
	public AudioClip swordAttack4;
	public AudioClip hurricaneSwooshSound;
	private AudioSource audioPlayer;
	private LaserScript lasScript;
	
	private float attackRotationAngle;
	
	public float attackDelay = 150.0f;
	public float attackTimer;
	
	public float maxHealth;

	public float maxMana;
	public float mana;

	private float manaRegenIncrement;

	Vector3 cachedForward;
	private bool reachedLocation = false;
	
	// Use this for initialization
	void Start () 
	{
		usedMagicAttack = false;
		hpScript = gameObject.GetComponent<HealthScript>();
		lightColor = "White";
		uiScript = (CursorScript) GameObject.Find("Menu Cursor").GetComponent<CursorScript>();
		state = "Standby";
		prevLocation = transform.position;
		speed = 15.0f;
		successfullyAttacked = false;

		attackTimer = 0.0f;
		
		blackTimer = 0;
		
		dmgBox = (DamageBoxScript) GameObject.Find ("DamageBox").GetComponent<DamageBoxScript>();

		maxHealth = maxMana = 0.0f;
		maxHealth = GameObject.Find ("Menu Cursor").GetComponent<StatsScript>().GetMaxHP (gameObject.name);
		health = maxHealth;
		maxMana = GameObject.Find ("Menu Cursor").GetComponent<StatsScript>().GetMaxMP (gameObject.name);
		mana = maxMana;

		audioPlayer = gameObject.AddComponent<AudioSource>();
		if (gameObject.name == "Zebulon")
		{
			lasScript = gameObject.GetComponentInChildren<LaserScript>();
			lasScript.Hide();
		}
		cachedForward = transform.forward;
	}

	public float GetAttackDelay()
	{
		return attackDelay;
	}

	void FixedUpdate()
	{
		ManaRegen ();
		if (attackTimer > 0)
			attackTimer--;
	}

	// Update is called once per frame
	void Update ()
	{		
		DebugStuff ();
		PerformTurn ();

		
		// Attack Melee Fix
		if(animation.IsPlaying("Slash")) 
		{
			if(animation["Slash"].normalizedTime >= 0.9)
			{
				PerformAttack();
			}
		}
		hpScript.Health = health;
		hpScript.Mana = mana;
		hpScript.TTA = attackTimer;
	}
	
	public void InitiateAttackSequence()
	{
		if (state != "Dead")
		{
			successfullyAttacked = false;
			state = "MakingMove";
			prevLocation = transform.position;
			if (actionChoice != "Ranged (All)" && actionChoice != "Heal")
			{
				RotateTowardsTarget(attackTarget.transform.position);
			}
		}
	}
	
	
	
	public void DebugStuff()
	{
		if (blackTimer > 0)
			blackTimer--;
		if (health <= 0)
		{
			if (state != "Dead")
			{
				state = "Dead";
				animation.Play ("Dead");
			}


			if(animation["Dead"].normalizedTime >= 0.9)	animation.Stop ();
		//	else animation.Play ("Dead");
		}
		if (blackTimer <= 0) 
		{
			gameObject.renderer.material.color = Color.blue;
		}
	}
	
	public void PerformTurn()
	{
		if (state != "Standby" && state != "Dead")
		{
			if (actionChoice == "Melee")
			{
				lightColor = "Red";
				MeleeAttackUpdate();
			}
			
			else if (actionChoice == "Heal")
			{
				lightColor = "Green";
				HealUpdate();
			}

			else if (actionChoice == "Slayer's Descent")
			{
				SwordTechBehaviour();
			}
			else if (actionChoice == "Blade Rush")
			{
				SwordTechBehaviour();
			}
			else if (actionChoice == "Hurricane Slash")
			{
				SwordTechBehaviour();
			}
			else if (actionChoice == "Worldslayer")
			{
				SwordTechBehaviour();
			}

			else
			{				
				if (actionChoice == "Ranged (All)")
				{
					if(!animation.IsPlaying("AoE Start")) animation.Play ("AoE");
				}
				else	
				{
					if(!animation.IsPlaying("Ranged")) animation.Play("Ranged");
				}
				
				lightColor = "Blue";
				RangedAttackUpdate();
			}
		}
		else if (!animation.isPlaying && gameObject.name == "Zebulon (Clone)")
		{
			animation.Play ("stand");
		}
	}
	
	public void doAttack()
	{
		animation.Play("Slash");
		audioPlayer.clip = slash;
		audio.volume = 0.5f;
		audio.PlayOneShot(slash);
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Enemy") doAttack();
	}
	
	public void MoveToAttack()
	{
		if (Vector3.Distance (transform.position, attackDestination) > 4)
			transform.position = Vector3.MoveTowards (transform.position, attackDestination, speed*Time.deltaTime);
		if(!animation.IsPlaying("Slash")) animation.Play("Run");
	}
	
	public void PerformAttack()
	{
		//attack animation stuff happens her
		transform.Rotate (Vector3.up, 180, Space.World);
		animation.Play("Run");		
		successfullyAttacked = true;
		float dmgRoll = uiScript.GetStatsScript().MeleeDamageRoll(gameObject.name, attackTarget.name);
		if (gameObject.name == "Zebulon")
			attackTarget.TakeDamage (dmgRoll);
		else
			attackTarget.TakeDamage (0.15f*dmgRoll);
		attackTimer = attackDelay;
		uiScript.GetCameraScript().ReturnToStandardMode();
	}
	
	public void PerformRangedAttack()
	{
		if (state == "Attacking")
		{
			state = "AttackingFromRange";
			blackTimer = animation.clip.length/Time.deltaTime;;
		}
		else if (state == "AttackingFromRange" && blackTimer > 0)
		{
			gameObject.renderer.material.color = Color.black;
			blackTimer--;
		}
		else if (state == "AttackingFromRange")
		{
			successfullyAttacked = true;
			if (actionChoice == "Ranged (Single)")
			{
				float dmgRoll = uiScript.GetStatsScript().ProjectileDamageRoll(gameObject.name, attackTarget.name);
				attackTarget.TakeDamage (dmgRoll);
				Debug.Log ("Successfully damaged "+attackTarget.name);
			}
			else if (actionChoice == "Ranged (All)")
			{
				for (int i = 0; i < targetsToDamage.Count; i++)
				{
					float dmgRoll = uiScript.GetStatsScript().ProjectileDamageRoll(gameObject.name, attackTarget.name);
					dmgRoll /= targetsToDamage.Count;
					targetsToDamage[i].TakeDamage (dmgRoll);
					Debug.Log ("Successfully damaged "+targetsToDamage[i].name);
				}
			}
			
			attackTimer = attackDelay;
		}
	}
	
	public void ReceiveHeal(float amt)
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
	
	public void MoveBackToStartPosition()
	{
		animation.Play ("Run");
		transform.position = Vector3.MoveTowards (transform.position, prevLocation, speed*Time.deltaTime);
		if (Vector3.Distance (transform.position, prevLocation) <= 0)
		{
			//made it back to starting position
			state = "Standby";
			successfullyAttacked = false;
			reachedLocation = false;
			lightColor = "White";
			animation.Play ("Idol Taunt");
			transform.Rotate(Vector3.up, 180-attackRotationAngle, Space.World);
			uiScript.WakeUp ();
		}
	}
	
	public void TakeDamage(float amount)
	{
		health -= Mathf.RoundToInt (amount);
		animation.Play ("Hit");
		if (health < 0)
			health = 0.0f;
		dmgBox.DisplayDamage((int)amount, this.transform);
	}
	
	public void SetTarget(EnemyScript target)
	{
		if (actionChoice != "Heal")
			attackTarget = target;
		else
			enemyHealTarget = target;
		attackDestination = target.transform.position;
	}
	
	public void SetTarget(PlayerScript toHeal)
	{
		healTarget = toHeal;
	}
	
	public void SetTarget(List<EnemyScript> targets)
	{
		targetsToDamage = targets;
	}
	
	public bool IsReady()
	{
		return (attackTimer <= 0);
	}
	
	void MeleeAttackUpdate()
	{
		if (!successfullyAttacked)
		{
			MoveToAttack();
		}
		else
		{
			MoveBackToStartPosition();
		}
	}
	
	void RangedAttackUpdate()
	{
		if (state == "MakingMove")
		{
			state = "AttackingFromRange";
			blackTimer = animation.clip.length/Time.deltaTime;
		}
		else if (state == "AttackingFromRange" && blackTimer > 0)
		{
			gameObject.renderer.material.color = Color.black;
			blackTimer--;
		}
		else if (state == "AttackingFromRange")
		{
			successfullyAttacked = true;
			if (!usedMagicAttack)
			{
				audioPlayer.clip = laser;
				audio.volume = 0.15f;
				audio.PlayOneShot(laser);
			}
			if (actionChoice == "Ranged (All)")
			{
				for (int i = 0; i < targetsToDamage.Count; i++)
				{
					attackTarget = targetsToDamage[i];
					CreateLaser(attackTarget);
					float dmgRoll = uiScript.GetStatsScript().ProjectileDamageRoll(gameObject.name, attackTarget.name);
					dmgRoll /= targetsToDamage.Count;
					targetsToDamage[i].TakeDamage (dmgRoll);
					Debug.Log ("Successfully damaged "+targetsToDamage[i].name);
				}
				animation.Play("AoE Done");
				attackTimer = attackDelay;
				state = "Standby";
				lightColor = "White";
				uiScript.WakeUp ();
			}
			else
			{
				if(!usedMagicAttack)
				{
					if(actionChoice == "Spacetime Friction")
					{
						mana -= manaCostDictionary["Spacetime Friction"];
						GameObject clone;
						clone = (GameObject) Instantiate(fireball, this.transform.position,Quaternion.identity);
						ParticleEffectScript fb = clone.GetComponent<ParticleEffectScript>();
						fb.SetTarget(this, attackTarget);
						fb.SetCamera(uiScript.GetCameraScript());
						uiScript.GetCameraScript().FollowMagicAttack(this.gameObject, fb);
						audioPlayer.clip = fireballImpact;
						audio.volume = 1.0f;
						audio.PlayOneShot(fireballImpact);
					}
					else if(actionChoice == "Nanodampen")
					{
						mana -= manaCostDictionary["Nanodampen"];
						GameObject clone;
						clone = (GameObject) Instantiate(iceball, this.transform.position,Quaternion.identity);
						ParticleEffectScript fb = clone.GetComponent<ParticleEffectScript>();
						fb.SetTarget(this, attackTarget);
						fb.SetCamera(uiScript.GetCameraScript());
						uiScript.GetCameraScript().FollowMagicAttack(this.gameObject, fb);
						audioPlayer.clip = iceClip;
						audio.volume = 1.0f;
						audio.PlayOneShot(iceClip);
					}
					else if(actionChoice == "Lorentz Blast")
					{
						mana -= manaCostDictionary["Lorentz Blast"];
						GameObject clone;
						clone = (GameObject) Instantiate(lightning, attackTarget.transform.position,Quaternion.identity);
						ParticleEffectScript fb = clone.GetComponent<ParticleEffectScript>();
						fb.SetTarget(this, attackTarget);
						audioPlayer.clip = thunderClip;
						audio.volume = 1.0f;
						audio.PlayOneShot(thunderClip);
					}
				}
				if (actionChoice == "Ranged (Single)")
				{
					CreateLaser(attackTarget);

					float dmgRoll = uiScript.GetStatsScript().ProjectileDamageRoll(gameObject.name, attackTarget.name);
					attackTarget.TakeDamage (dmgRoll);
				}
				else
				{
					//Nothing here yet
				}
				animation.Play("Ranged Done");
				attackTimer = attackDelay;
				if (actionChoice != "Lorentz Blast" && actionChoice != "Nanodampen" && actionChoice != "Spacetime Friction")
				{
					transform.Rotate(Vector3.up, -attackRotationAngle, Space.World);
					state = "Standby";
					uiScript.WakeUp ();
				}
				else
				{
					usedMagicAttack = true;
				}
				lightColor = "White";
			}
			
			if (!animation.IsPlaying("Idol Taunt"))
			{
				animation.Play ("Idol Taunt");
			}
		}
	}
	
	void HealUpdate()
	{
		bool healReady = false;
		if (state == "MakingMove")
		{
			if (gameObject.name == "Zebulon")
			{
				if (!animation.IsPlaying("Heal"))	animation.Play("Heal");
			}
			else if (gameObject.name == "Zebulon (Clone)")
			{
				if (!animation.IsPlaying("Ranged"))	animation.Play("Ranged");
			}
			
			state = "Healing";
			healTimer = animation.clip.length/Time.deltaTime;
		}
		else if (state == "Healing")
		{

			if (gameObject.name == "Zebulon")
			{
				if(animation["Heal"].normalizedTime >= 0.9)
					healReady = true;
			}
			else if (gameObject.name == "Zebulon (Clone)")
			{
				if(animation["Ranged"].normalizedTime >= 0.9)
					healReady = true;
			}

			gameObject.renderer.material.color = Color.green;
			healTimer--;
		}

		if (state == "Healing" && healReady)
		{
			Debug.Log (this.name+" successfully healed.");
			successfullyAttacked = true;
			if (enemyHealTarget != null)
			{
				if (enemyHealTarget.enabled == true)
				{
					float healRoll = uiScript.GetStatsScript().HealRoll(gameObject.name);
					enemyHealTarget.ReceiveHeal(healRoll);
					attackTimer = attackDelay;
					if (gameObject.name == "Zebulon")
						animation.Play("Heal Done");
				}
				enemyHealTarget = null;
			}
			else if (healTarget != null)
			{
				if (healTarget.GetState () != "Dead")
				{
					float healRoll = uiScript.GetStatsScript().HealRoll(gameObject.name);
					healTarget.ReceiveHeal(healRoll);
					attackTimer = attackDelay;
					
					if (gameObject.name == "Zebulon")
						animation.Play("Heal Done");
				}
				healTarget = null;
			}
			state = "Standby";
			lightColor = "White";
			uiScript.WakeUp ();
		}
	}
	
	private void RotateTowardsTarget(Vector3 loc)
	{
		Vector3 dir = loc-transform.position;
		
		attackRotationAngle = Vector3.Angle (transform.forward, dir);
		transform.Rotate (Vector3.up, attackRotationAngle, Space.World);
	}
	public void endRangedTurn()
	{
		transform.Rotate(Vector3.up, -attackRotationAngle, Space.World);
		state = "Standby";
		uiScript.WakeUp ();
		usedMagicAttack = false;
	}

	public string GetState()
	{
		return state;
	}

	private void ManaRegen()
	{
		manaRegenIncrement = Time.deltaTime*0.6f;
		float projectedMana = manaRegenIncrement + mana;
		if (projectedMana > maxMana)
		{
			mana = maxMana;
		}
		else
		{
			mana = projectedMana;
		}
	}

	private Dictionary<string, string> GetAnimationName(string characterName)
	{
		if (characterName == "Zebulon" || characterName == "Zebulon (Clone)")
			return zebulonAnimationNameDictionary;
		else if (characterName == "Dalton")
			return knightAnimationNameDictionary;
		else
			return null;
	}

	private void CreateLaser(EnemyScript targetDude)
	{
		Vector3 direction = targetDude.transform.position-transform.position;
		float angleBetween = Vector3.Angle (direction, cachedForward);

		Vector3 laserPosition = transform.position;
		laserPosition.x += 0.96f;
		laserPosition.y += 4.5f;
		laserPosition.z += 0.8f;

		GameObject clone;
		clone = (GameObject) Instantiate(dmgLaser, laserPosition, Quaternion.LookRotation (cachedForward));
		clone.GetComponent<LaserScript>().Shoot (angleBetween);
		//lasScript.Shoot (angleBetween);
	}

	private void SwordTechBehaviour()
	{
		if (!successfullyAttacked && !reachedLocation)
		{
			MoveToAttackTech();
		}
		else if (!successfullyAttacked)
		{
			PerformTechAttack();
		}
		else if (reachedLocation && successfullyAttacked)
		{
			MoveBackToStartPosition();
		}
	}

	private void BladeRushBehaviour()
	{
	}

	private void HurricaneSlashBehaviour()
	{
	}

	private void WorldslayerBehaviour()
	{
	}

	private void MoveToAttackTech()
	{
		if (!reachedLocation)
		{
			if (Vector3.Distance (transform.position, attackDestination) > 4)
			{
				transform.position = Vector3.MoveTowards (transform.position, attackDestination, speed*Time.deltaTime);
			}
			else
				reachedLocation = true;

			if(!animation.IsPlaying("Run")) animation.Play("Run");
		}
	}

	private void PerformTechAttack()
	{

		if (actionChoice == "Slayer's Descent")
		{
			if (!animation.IsPlaying ("attack 5"))
			{
				animation.Play ("attack 5");
				blackTimer = animation.clip.length/Time.deltaTime;
			}
			else
			{
				blackTimer--;
				if(animation["attack 5"].normalizedTime >= 0.91)
				{
					mana -= manaCostDictionary[actionChoice];
					float dmgRoll = uiScript.GetStatsScript().MeleeDamageRoll(gameObject.name, attackTarget.name);
					audioPlayer.clip = swordAttack1;
					audio.volume = 0.4f;
					audio.PlayOneShot(swordAttack1);
					attackTarget.TakeDamage (0.5f*dmgRoll);
					attackTimer = attackDelay;
					uiScript.GetCameraScript().ReturnToStandardMode();
					successfullyAttacked = true;
					transform.Rotate (Vector3.up, 180, Space.World);
				}
			}
		}
		else if (actionChoice == "Blade Rush")
		{
			if (!animation.IsPlaying ("attack 3"))
			{
				animation.Play ("attack 3");
				blackTimer = animation.clip.length/Time.deltaTime;
			}
			else
			{
				blackTimer--;

				if (animation["attack 3"].normalizedTime >= 0.9)
				{
					mana -= manaCostDictionary[actionChoice];
					float dmgRoll = uiScript.GetStatsScript().MeleeDamageRoll(gameObject.name, attackTarget.name);
					audioPlayer.clip = swordAttack2;
					audio.volume = 0.4f;
					audio.PlayOneShot(swordAttack2);
					attackTarget.TakeDamage (0.4f*dmgRoll);
					attackTimer = attackDelay;
					uiScript.GetCameraScript().ReturnToStandardMode();
					successfullyAttacked = true;
					transform.Rotate (Vector3.up, 180, Space.World);
				}
			}
		}
		else if (actionChoice == "Hurricane Slash")
		{
			if (!animation.IsPlaying ("attack 4"))
			{
				animation.Play ("attack 4");
				blackTimer = animation.clip.length/Time.deltaTime;
			}
			else
			{
				blackTimer--;
				if(animation["attack 4"].normalizedTime >= 0.9)
				{
					mana -= manaCostDictionary[actionChoice];
					float dmgRoll = uiScript.GetStatsScript().MeleeDamageRoll(gameObject.name, attackTarget.name);
					audioPlayer.clip = swordAttack3;
					audio.volume = 0.4f;
					audio.PlayOneShot(swordAttack3);
					attackTarget.TakeDamage (0.6f*dmgRoll);
					attackTimer = attackDelay;
					uiScript.GetCameraScript().ReturnToStandardMode();
					successfullyAttacked = true;
					transform.Rotate (Vector3.up, 180, Space.World);
				}
			}
		}
		else if (actionChoice == "Worldslayer")
		{
			if (!animation.IsPlaying ("attack 6"))
			{
				animation.Play ("attack 6");
				blackTimer = animation.clip.length/Time.deltaTime;
			}
			else
			{
				blackTimer--;
				if(animation["attack 6"].normalizedTime >= 0.9)
				{
					mana -= manaCostDictionary[actionChoice];
					float dmgRoll = uiScript.GetStatsScript().MeleeDamageRoll(gameObject.name, attackTarget.name);
					audioPlayer.clip = swordAttack4;
					audio.volume = 0.4f;
					audio.PlayOneShot(swordAttack4);
					attackTarget.TakeDamage (1.1f*dmgRoll);
					attackTimer = attackDelay;
					uiScript.GetCameraScript().ReturnToStandardMode();
					successfullyAttacked = true;
					transform.Rotate (Vector3.up, 180, Space.World);
				}
			}
		}
	}

	public bool HasSufficientMP(string potentialAction)
	{
		if (!manaCostDictionary.ContainsKey (potentialAction))
			return true;
		else
		{
			if (mana - manaCostDictionary[potentialAction] < 0)
				return false;
			else
				return true;
		}
	}
}