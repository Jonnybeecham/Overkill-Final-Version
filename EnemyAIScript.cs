using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyAIScript : MonoBehaviour {
	private CursorScript uiScript;
	public List<EnemyScript> enemy;
	private bool isHealing;
	private bool isBuffing;
	private EnemyScript healTarget;
	// Use this for initialization
	void Start () {
		uiScript = (CursorScript) GameObject.Find ("Menu Cursor").GetComponent<CursorScript>();
		isHealing = false;
		enemy = new List<EnemyScript>();
		if (Application.loadedLevelName == "GrasslandsBattle")
		{
			EnemyScript enemySc1 = (EnemyScript) GameObject.Find("Green Spider").GetComponent<EnemyScript>();
			EnemyScript enemySc2 = (EnemyScript) GameObject.Find("Dark Troll").GetComponent<EnemyScript>();
			enemy.Add(enemySc1);
			enemy.Add (enemySc2);
		}
		else if (Application.loadedLevelName == "snowScene")
		{
			EnemyScript enemySc1 = (EnemyScript) GameObject.Find("skeletonMage").GetComponent<EnemyScript>();
			EnemyScript enemySc2 = (EnemyScript) GameObject.Find("skeletonSpearman").GetComponent<EnemyScript>();
			enemy.Add(enemySc1);
			enemy.Add (enemySc2);
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public string MakeTurnChoice(){
		isHealing = false;
		string decision;
		//check to see if healing is already selected for enemy
		for (int i = 0; i < enemy.Count; i++) {
			if(enemy[i].nextTurnChoice == "Healing"){
				isHealing = true;
			}
		}
		// if so, attack
		if (isHealing == true) {
			decision = "Attacking";
			return decision;
		}
		else {
			bool needsHealing = false;
			// check to see if healing is needed
			for (int i = 0; i < enemy.Count; i++) {
				if(enemy[i].health < 0.6f*enemy[i].maxHealth){
					needsHealing = true;
				}
			}
			// if so, heal
			if(needsHealing == true){
				decision = "Healing";
				return decision;
			}
			// if not, attack
			else{
				decision = "Attacking";
				return decision;
			}
		}
		
	}
	public EnemyScript SelectHealTarget(){
		float lowest = 100.0f;
		for (int i = 0; i < uiScript.enemy.Count; i++) 
		{
			if (uiScript.enemy[i].health < lowest)
			{
				lowest = uiScript.enemy[i].health;
				healTarget = uiScript.enemy[i];
			}
		}
		return healTarget;
	}
}
