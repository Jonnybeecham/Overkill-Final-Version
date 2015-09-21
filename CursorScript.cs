using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CursorScript : MonoBehaviour 
{
	public Camera mainCamera;
	private CameraScript cScript;
	string state;
	string[] options;
	int pointerIndex;
	private bool attackUnderway;
	public List<PlayerScript> hero;
	public List<EnemyScript> enemy;
	public List<EnemyScript> deadEnemies;
	public PlayerScript currentHero;
	public EnemyScript enemyTarget;
	public AudioClip battleEndClip;
	bool musicPlayed;
	private BattleSkillSelectPanelScript bSSPscript;
	private BattleSkillSelectPanelScript knightBattleSkillScript;
	private MainBattleMenuScript mBMscript;
	private MagicSkillSelectMenuScript mSSMscript;
	private MagicSkillSelectMenuScript techScript;
	
	public Queue<GameObject> attackOrder;

	private CircleActivationScript selectCircle;
	private StatsScript statScript;
	
	// Use this for initialization
	void Start () 
	{
		statScript = gameObject.GetComponent<StatsScript>();

		bSSPscript = GameObject.Find ("BattleSkillsCanvas").GetComponent<BattleSkillSelectPanelScript>();
		bSSPscript.Fade ();
		knightBattleSkillScript = GameObject.Find ("KnightBattleSkillsCanvas").GetComponent<BattleSkillSelectPanelScript>();
		knightBattleSkillScript.Fade ();
		mBMscript = GameObject.Find ("MainBattleMenuCanvas").GetComponent<MainBattleMenuScript>();
		mBMscript.Fade ();
		mSSMscript = GameObject.Find ("MagicSkillMenuCanvas").GetComponent<MagicSkillSelectMenuScript>();
		mSSMscript.Fade ();
		techScript = GameObject.Find ("SwordTechniqueMenuCanvas").GetComponent<MagicSkillSelectMenuScript>();
		techScript.Fade ();

		cScript = mainCamera.GetComponent<CameraScript>();
		
		musicPlayed = false;
		hero = new List<PlayerScript>();
		enemy = new List<EnemyScript>();

		if (Application.loadedLevelName == "GrasslandsBattle")
		{
			PlayerScript playerSc1 = (PlayerScript) GameObject.Find("Zebulon (Clone)").GetComponent<PlayerScript>();
			EnemyScript enemySc1 = (EnemyScript) GameObject.Find("Green Spider").GetComponent<EnemyScript>();
			
			PlayerScript playerSc2 = (PlayerScript) GameObject.Find("Zebulon").GetComponent<PlayerScript>();
			EnemyScript enemySc2 = (EnemyScript) GameObject.Find("Dark Troll").GetComponent<EnemyScript>();
			
			//PlayerScript playerSc3 = (PlayerScript) GameObject.Find("Dalton").GetComponent<PlayerScript>();
			
			hero.Add(playerSc1);
			hero.Add (playerSc2);
			//hero.Add (playerSc3);
			enemy.Add(enemySc1);
			enemy.Add (enemySc2);
		}
		else if (Application.loadedLevelName == "snowScene")
		{
			PlayerScript playerSc1 = (PlayerScript) GameObject.Find("Zebulon (Clone)").GetComponent<PlayerScript>();
			
			PlayerScript playerSc2 = (PlayerScript) GameObject.Find("Zebulon").GetComponent<PlayerScript>();

			EnemyScript enemySc1 = (EnemyScript) GameObject.Find("skeletonMage").GetComponent<EnemyScript>();

			EnemyScript enemySc2 = (EnemyScript) GameObject.Find("skeletonSpearman").GetComponent<EnemyScript>();

			hero.Add(playerSc1);
			hero.Add (playerSc2);
			enemy.Add (enemySc1);
			enemy.Add (enemySc2);
		}
		
		state = "PlayerSelect";
		
		//Change the length of options later to accommodate more menu options
		options = new string[1];
		options[0] = "Attack";
		//Add more menu options here later as string elements of the options array
		
		pointerIndex = 0; //current menu option to point at
		
		attackOrder = new Queue<GameObject>();


		selectCircle = GameObject.Find ("SelectionCircle").GetComponent<CircleActivationScript>();
		selectCircle.Fade ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		CheckEndOfBattle();
		selectPlayerCharacter ();
		selectEnemyTarget ();
		if (!attackUnderway)
		{
			CheckQueue ();
		}

		
		if (state == "MenuSelect")
		{
			//These 3 if-statements are currently useless
			if (Input.GetButtonDown ("Switch") && !Input.GetButtonDown ("Click") && !Input.GetButtonDown ("Cancel"))
			{
				Debug.Log ("Switch button press detected.");
				SwapBetweenPlayers();
			}
			if (Input.GetKeyDown(KeyCode.Return) && options[pointerIndex] == "Attack")
			{
			}
			else if (Input.GetAxis("Vertical") < 0)
			{
				//cycle downward in the options array
				pointerIndex = (pointerIndex + 1)%options.Length;
			}
			else if (Input.GetAxis("Vertical") > 0)
			{
				//cycle upward in the options array
				pointerIndex = (options.Length + pointerIndex - 1)%options.Length;
			}
		}

		else if ((state == "PlayerSelect" || state == "MenuSelect") && !attackUnderway)
		{
			CheckForMouseOverPlayer();

		}

		else if (state == "AttackTargetSelect" || state == "HealTargetSelect")
		{
			CheckForMouseOverEnemy ();
			if (state == "HealTargetSelect")
				CheckForMouseOverPlayer ();
			if (Input.GetButtonDown ("Cancel") && !Input.GetButtonDown ("Click") && !Input.GetButtonDown ("Switch"))
			{
				if (currentHero.name == "Zebulon (Clone)")
				{
					currentHero.animation.Play ("stand");
				}
				else if (currentHero.name == "Zebulon")
				{
					currentHero.animation.Play ("Idol Taunt");
					currentHero.lightColor = "White";
				}
				ShiftToAttackMenu();
			}
		}
	}
	
	public void CheckEndOfBattle()
	{
		float numberOfEnemies = GetNumberOfEnemies ();
		if (numberOfEnemies == 0.0f && !audio.isPlaying && state == "PlayerSelect" && !musicPlayed) 
		{
			audio.PlayOneShot(battleEndClip);
			musicPlayed = true;
		}
		else if (numberOfEnemies == 0.0f && state == "PlayerSelect" && musicPlayed && !audio.isPlaying)
		{
			if (Application.loadedLevelName == "GrasslandsBattle")
			{
				Application.LoadLevel ("snowScene");
			}
			else if (Application.loadedLevelName == "snowScene")
			{
				Application.LoadLevel ("main menu");
			}
		}
		else if (GetLivingPlayerCount() <= 0.0f)
		{
			Application.LoadLevel ("main menu");
		}
	}

	void CheckForMouseOverPlayer()
	{
		GameObject mouseOver = GetMouseoverGameObject();
		if (mouseOver != null)
		{
			if (mouseOver.tag == "Player")
			{
				if (!selectCircle.IsActive ())
					selectCircle.Appear (mouseOver.transform.position);
			}
		}
		else if (selectCircle.IsActive())
		{
			selectCircle.Refresh ();
		}
	}

	void CheckForMouseOverEnemy()
	{
		GameObject mouseOver = GetMouseoverGameObject();
		if (mouseOver != null)
		{
			if (mouseOver.tag == "Enemy")
			{
				if (!selectCircle.IsActive ())
					selectCircle.Appear (mouseOver.transform.position);
			}
		}
		else if (selectCircle.IsActive())
		{
			selectCircle.Refresh ();
		}
	}

	public float GetLivingPlayerCount()
	{
		float sum = 0.0f;
		for (int i = 0; i < hero.Count; i++)
		{
			if (hero[i].GetState () != "Dead")
				sum += 1.0f;
		}
		return sum;
	}
	
	public float GetNumberOfEnemies()
	{
		return (float) enemy.Count;
	}
	
	public void CheckQueue()
	{
		if (attackOrder.Count > 0)
		{
			GameObject attacker = attackOrder.Dequeue();
			if (attacker.tag == "Enemy")
			{
				EnemyScript es = (EnemyScript) attacker.GetComponent<EnemyScript>();
				es.InitiateAttackSequence();
				attackUnderway = true;
			}
			else if (attacker.tag == "Player")
			{
				PlayerScript ps = (PlayerScript) attacker.GetComponent<PlayerScript>();
				if (ps.actionChoice == "Melee")
				{
					cScript.FollowMeleeAttack (attacker);
				}
				else if (ps.actionChoice == "Slayer's Descent")
				{
					cScript.FollowMeleeAttack (attacker);
				}
				else if (ps.actionChoice == "Blade Rush")
				{
					cScript.FollowMeleeAttack (attacker);
				}
				else if (ps.actionChoice == "Hurricane Slash")
				{
					cScript.FollowMeleeAttack (attacker);
				}
				else if (ps.actionChoice == "Worldslayer")
				{
					cScript.FollowMeleeAttack (attacker);
				}
				else if (ps.actionChoice == "Ranged")
				{
				}
				else if (ps.actionChoice == "Ranged (All)")
				{
				}
				else if (ps.actionChoice == "Heal")
				{
				}
				else
				{
				}
				attackUnderway = true;
				ps.InitiateAttackSequence();

			}
		}
	}
	
	public void WakeUp()
	{
		//This is called by the attacking character's script to notify the cursor to return to default state
		attackUnderway = false;
		//state = "PlayerSelect";
	}
	
	public void selectPlayerCharacter()
	{
		//This is for player character selection
		if (Input.GetButtonDown ("Click") && (state == "PlayerSelect" || state == "MenuSelect"))
		{
			GameObject clickedGameObject = GetMouseoverGameObject ();
			if (clickedGameObject != null)
			{
				selectCircle.TargetSet ();
				if (clickedGameObject.tag == "Player")
				{
					if (clickedGameObject.GetComponent<PlayerScript>().IsReady())
					{
						currentHero = clickedGameObject.GetComponent<PlayerScript>();
						selectCircle.Refresh ();
						SetCircleToPlayer();
						Debug.Log ("Selected "+currentHero.name+" to attack.");
						DisplayMainBattleMenu ();
					}
				}
				
			}
		}
	}
	
	public void selectEnemyTarget()
	{
		//This is for enemy selection
		if (Input.GetButtonDown ("Click") && state == "AttackTargetSelect")
		{
			GameObject clickedGameObject = GetMouseoverGameObject ();
			if (clickedGameObject != null)
			{
				if (clickedGameObject.tag == "Enemy")
				{
					if (clickedGameObject.GetComponent<EnemyScript>().GetState() != "Dead")
					{
						enemyTarget = clickedGameObject.GetComponent<EnemyScript>();
						Debug.Log ("Selected "+enemyTarget.name+" as attack target.");
						currentHero.SetTarget(enemyTarget);
						EnqueuePlayer(currentHero.gameObject);
						selectCircle.Refresh ();
					}
				}
				else if (clickedGameObject.tag == "Player")
				{
					selectCircle.Refresh ();
					SetCircleToPlayer();
					currentHero = clickedGameObject.GetComponent<PlayerScript>();
					Debug.Log ("Selected "+currentHero.name+" to attack.");
					DisplayMainBattleMenu();
				}
			}
		}
		else if (Input.GetButtonDown ("Click") && state == "HealTargetSelect")
		{
			GameObject clickedGameObject = GetMouseoverGameObject ();
			if (clickedGameObject != null)
			{
				if (clickedGameObject.tag == "Enemy")
				{
					if (clickedGameObject.GetComponent<EnemyScript>().GetState() != "Dead")
					{
						enemyTarget = clickedGameObject.GetComponent<EnemyScript>();
						Debug.Log ("Selected "+enemyTarget.name+" as heal target.");
						currentHero.SetTarget(enemyTarget);
						EnqueuePlayer(currentHero.gameObject);
						selectCircle.Refresh ();
					}
				}
				else if (clickedGameObject.tag == "Player")
				{
					if (clickedGameObject.GetComponent<PlayerScript>().GetState() != "Dead")
					{
						PlayerScript pTarget = clickedGameObject.GetComponent<PlayerScript>();
						Debug.Log ("Selected "+pTarget.name+" as heal target.");
						currentHero.SetTarget(pTarget);
						EnqueuePlayer(currentHero.gameObject);
						selectCircle.Refresh ();
					}
				}
			}
		}
	}

	void SetCircleToPlayer()
	{
		selectCircle.Appear(currentHero.transform.position);
		selectCircle.TargetSet ();
	}

	void OnGUI () 
	{
		//display health bars and tta for all enemies, then all players
		/*for (int i = 0; i < enemy.Count; i++)
		{
			string enemyhpDisplay = enemy[i].name + " Health: "+enemy[i].health.ToString ();
			string enemympDisplay = enemy[i].name + " Mana: "+ Mathf.Round (enemy[i].mana).ToString ();
			string enemyTimerDisplay = "TTA: "+enemy[i].attackTimer.ToString ();
			
			Vector3 enemyPosition = enemy[i].transform.position;
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(enemyPosition);
			screenPosition.y = Camera.main.pixelHeight - screenPosition.y;
			GUI.Label (new Rect (screenPosition.x, screenPosition.y, screenPosition.x+150, screenPosition.y+10), enemyTimerDisplay);
			GUI.Label (new Rect (screenPosition.x, screenPosition.y+20, screenPosition.x+150, screenPosition.y+30), enemyhpDisplay);
			GUI.Label (new Rect (screenPosition.x, screenPosition.y+40, screenPosition.x+150, screenPosition.y+50), enemympDisplay);
		}
		
		for (int i = 0; i < hero.Count; i++)
		{
			string herohpDisplay = hero[i].name + " Health: " + hero[i].health.ToString ();
			string herompDisplay = hero[i].name + " Mana: " + Mathf.Round (hero[i].mana).ToString ();
			string heroTimerDisplay = "TTA: "+hero[i].attackTimer.ToString ();
			
			Vector3 heroPosition = hero[i].transform.position;
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(heroPosition);
			screenPosition.y = Camera.main.pixelHeight - screenPosition.y;
			GUI.Label (new Rect (screenPosition.x, screenPosition.y, screenPosition.x+150, screenPosition.y+10), heroTimerDisplay);
			GUI.Label (new Rect (screenPosition.x, screenPosition.y+20, screenPosition.x+150, screenPosition.y+30), herohpDisplay);
			GUI.Label (new Rect (screenPosition.x, screenPosition.y+40, screenPosition.x+150, screenPosition.y+50), herompDisplay);
		}
		*/
	}
	
	GameObject GetMouseoverGameObject()
	{
		// Builds a ray from camera point of view to the mouse position
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		// Casts the ray and get the first game object hit
		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			if (hit.transform.gameObject.tag == "Enemy")
			{
				if (hit.transform.gameObject.GetComponent<EnemyScript>().state != "Dead")
					return hit.transform.gameObject;
				else
					return null;
			}
			else if (hit.transform.gameObject.tag == "Player")
			{
				
				if (hit.transform.gameObject.GetComponent<PlayerScript>().state != "Dead")
					return hit.transform.gameObject;
				else
					return null;
			}
			else
				return null;
		}
		else
			return null;
	}
	
	public void EnqueueEnemy(GameObject nextEnemy)
	{
		attackOrder.Enqueue (nextEnemy);
	}
	
	public void EnqueuePlayer(GameObject nextPlayer)
	{
		attackOrder.Enqueue (nextPlayer);
		state = "PlayerSelect";
	}
	
	public void RemoveMe(EnemyScript deadEnemy)
	{
		enemy.Remove (deadEnemy);
		
		if (attackUnderway)
			attackUnderway = false;
		
		if (attackOrder.Contains (deadEnemy.gameObject))
		{
			Queue<GameObject> temp = new Queue<GameObject>();
			for (int i = 0; i < attackOrder.Count; i++)
			{
				GameObject tempObject  = attackOrder.Dequeue ();
				if (tempObject != deadEnemy)
					temp.Enqueue (tempObject);
			}
			attackOrder = temp;
		}
	}
	
	public string GetState()
	{
		return state;
	}
	
	public void SetTurnChoice(string choice)
	{
		if (currentHero.HasSufficientMP(choice))
		{
			bSSPscript.Fade();
			knightBattleSkillScript.Fade ();
			mSSMscript.Fade ();
			techScript.Fade ();
			
			currentHero.actionChoice = choice;
			
			if (choice == "Ranged (All)")
			{
				currentHero.SetTarget (enemy);
				EnqueuePlayer(currentHero.gameObject);
				if (!currentHero.animation.IsPlaying("AoE Start")) currentHero.animation.Play("AoE Start");
			}
			
			else if (choice == "Heal")
			{
				if (!currentHero.animation.IsPlaying("Heal Start")) 
				{
					if (currentHero.gameObject.name == "Zebulon")
						currentHero.animation.Play("Heal Start");
					else if (currentHero.gameObject.name == "Zebulon (Clone)")
						currentHero.animation.Play("ready B");

				}
				state = "HealTargetSelect";
			}
			
			else if (choice == "Melee")
			{
				if (currentHero.gameObject.name == "Zebulon")
				currentHero.animation.Play("Run Start");
				if (!currentHero.animation.IsPlaying("Run Start")) 
				{
					if (currentHero.gameObject.name == "Zebulon")
						currentHero.animation.Play("Run Ready");
					else if (currentHero.gameObject.name == "Zebulon Clone")
						currentHero.animation.Play("ready B");
				}
				state = "AttackTargetSelect";
			}
			else if (choice == "Slayer's Descent")
			{
				if (currentHero.gameObject.name == "Zebulon Clone")
					currentHero.animation.Play("ready B");
				state = "AttackTargetSelect";
			}
			else if (choice == "Blade Rush")
			{
				if (currentHero.gameObject.name == "Zebulon Clone")
					currentHero.animation.Play("ready B");
				state = "AttackTargetSelect";
			}
			else if (choice == "Hurricane Slash")
			{
				if (currentHero.gameObject.name == "Zebulon Clone")
					currentHero.animation.Play("ready B");
				state = "AttackTargetSelect";
			}
			else if (choice == "Worldslayer")
			{
				if (currentHero.gameObject.name == "Zebulon Clone")
					currentHero.animation.Play("ready B");
				state = "AttackTargetSelect";
			}
			else
			{
				if (!currentHero.animation.IsPlaying("Ranged Start")) currentHero.animation.Play("Ranged Start");
				state = "AttackTargetSelect";
			}
		}
	}
	
	public void DisplayMainBattleMenu()
	{
		bSSPscript.Fade ();
		knightBattleSkillScript.Fade ();
		state = "MenuSelect";
		mBMscript.Appear ();
	}
	
	public void ShiftToAttackMenu()
	{
		techScript.Fade ();
		mSSMscript.Fade ();
		state = "SelectAction";
		if (currentHero.name == "Zebulon")
			bSSPscript.Appear ();
		else if (currentHero.name == "Zebulon (Clone)")
			knightBattleSkillScript.Appear ();
	}

	public void ShiftToMagicAttacksMenu()
	{
		bSSPscript.Fade ();
		mSSMscript.Appear ();
	}

	public CameraScript GetCameraScript()
	{
		return cScript;
	}

	public StatsScript GetStatsScript()
	{
		return statScript;
	}

	public void ShiftToTechniqueMenu()
	{
		knightBattleSkillScript.Fade ();
		techScript.Appear ();
	}

	public PlayerScript SelectRandomPlayer()
	{
		int roll = Random.Range (0, hero.Count);
		if (hero[roll].GetState () == "Dead" && hero.Count > 1)
		{
			roll = -1*roll + 1;
		}
		else if (hero[roll].GetState () == "Dead")
		{
			return null;
		}
		return hero[roll];
	}

	private void SwapBetweenPlayers()
	{
		int prevIndex = hero.IndexOf (currentHero);
		int newIndex = (prevIndex+1) % hero.Count;

		if (hero[newIndex].IsReady ())
		{
			currentHero = hero[newIndex];
			
			selectCircle.Refresh ();
			SetCircleToPlayer();
			
			Debug.Log ("Selected "+currentHero.name+" to attack.");
			DisplayMainBattleMenu ();
		}

	}
}
