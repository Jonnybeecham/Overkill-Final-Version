using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatsScript : MonoBehaviour 
{
	private readonly Dictionary<string, float> staminaMap = new Dictionary<string, float>()
	{
		{"Zebulon", 132.0f}, 
		{"Dalton", 172.0f},
		{"Green Spider", 180.0f}, 
		{"Dark Troll", 210.0f},
		{"skeletonMage", 490.0f},
		{"skeletonSpearman", 630.0f}
	};
	
	private readonly Dictionary<string, float> intellectMap = new Dictionary<string, float>()
	{
		{"Zebulon", 85.0f}, 
		{"Dalton", 72.0f},
		{"Green Spider", 58.0f}, 
		{"Dark Troll", 71.0f},
		{"skeletonMage", 135.0f},
		{"skeletonSpearman", 80.0f}
	};
	
	private readonly Dictionary<string, float> strengthMap = new Dictionary<string, float>()
	{
		{"Zebulon", 98.0f}, 
		{"Dalton", 152.0f},
		{"Green Spider", 211.0f}, 
		{"Dark Troll", 219.0f},
		{"skeletonMage", 180.0f},
		{"skeletonSpearman", 240.0f}
	};
	
	private readonly Dictionary<string, float> agilityMap = new Dictionary<string, float>()
	{
		{"Zebulon", 121.0f}, 
		{"Dalton", 132.0f},
		{"Green Spider", 182.0f}, 
		{"Dark Troll", 111.0f},
		{"skeletonMage", 153.0f},
		{"skeletonSpearman", 143.0f}
	};
	
	private readonly Dictionary<string, float> spiritMap = new Dictionary<string, float>()
	{
		{"Zebulon", 78.0f}, 
		{"Dalton", 84.0f},
		{"Green Spider", 55.0f}, 
		{"Dark Troll", 59.0f},
		{"skeletonMage", 50.0f},
		{"skeletonSpearman", 30.0f}
	};
	
	private readonly Dictionary<string, float> weaponDamageMap = new Dictionary<string, float>()
	{
		{"Zebulon", 78.0f}, 
		{"Dalton", 108.0f},
		{"Green Spider", 41.0f}, 
		{"Dark Troll", 48.0f},
		{"skeletonMage", 60.0f},
		{"skeletonSpearman", 80.0f}
	};
	
	private readonly Dictionary<string, float> weaponDefenseMap = new Dictionary<string, float>()
	{
		{"Zebulon", 24.0f}, 
		{"Dalton", 30.0f},
		{"Green Spider", 15.0f}, 
		{"Dark Troll", 14.0f},
		{"skeletonMage", 110.0f},
		{"skeletonSpearman", 135.0f}
	};
	
	private readonly Dictionary<string, float> magicDefenseMap = new Dictionary<string, float>()
	{
		{"Zebulon", 78.0f}, 
		{"Dalton", 65.0f},
		{"Green Spider", 55.0f}, 
		{"Dark Troll", 59.0f},
		{"skeletonMage", 97.0f},
		{"skeletonSpearman", 72.0f}
	};
	
	private readonly Dictionary<string, float> spellPowerMap = new Dictionary<string, float>()
	{
		{"Spacetime Friction", 118.0f}, //avg 91.45
		{"Nanodampen", 93.0f}, //avg 88.35
		{"Lorentz Blast", 138.0f} //avg 91.997
	};
	
	private readonly Dictionary<string, float> spellMinPercentMap = new Dictionary<string, float>()
	{
		{"Spacetime Friction", 0.55f}, 
		{"Nanodampen", 0.9f}, 
		{"Lorentz Blast", 0.3333f}
	};

	/*private float Sta;
	private float Int;
	private float Spr;
	private float Str;
	private float Agi;
	private float Wdam;
	private float Spower;
	private float Adef;*/
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	public float GetMaxHP(string name)
	{
		Debug.Log (name);
		if (name == "Zebulon (Clone)")
		{
			name = "Dalton";
		}
		float hp = 0.0f;
		if (staminaMap.ContainsKey (name))
		{
		    hp = 20.0f + (3.53f * staminaMap[name]);
		}
		else
			Debug.Log (name+" isn't a key");
		return Mathf.RoundToInt (hp);
	}
	
	public float GetMaxMP(string name)
	{
		if (name == "Zebulon (Clone)")
		{
			name = "Dalton";
		}
		float mp = 0.0f;
		if (intellectMap.ContainsKey (name) && spiritMap.ContainsKey(name))
		{
			mp = 5.0f + (1.21f * intellectMap[name]) + (0.46f * spiritMap[name]);
		}
		else
			Debug.Log (name+" isn't a key");
		return Mathf.RoundToInt (mp);
	}
	
	private float GetATK(string name)
	{
		float Atk = weaponDamageMap[name] * (1.0f + strengthMap[name]/100.0f);
		return Atk;
	}
	
	private float GetDEF(string name)
	{
		float Def = weaponDefenseMap[name] * (1.0f + strengthMap[name]/100.0f + agilityMap[name]/100.0f) + 5.0f;
		return Def;
	}
	
	private float GetMATK(string spellName, string attackerName)
	{
		float Matk = spellPowerMap[spellName] * (1.0f + intellectMap[attackerName]/50.0f);
		return Matk;
	}
	
	private float GetMDEF(string name)
	{
		float Mdef = magicDefenseMap[name] * (1.0f + intellectMap[name]/200.0f + spiritMap[name]/75.0f);
		return Mdef;
	}

	private bool IsMeleeCrit(string attackerName, string defenderName)
	{
		float critThreshold = 5.0f;
		critThreshold += 20.0f*(strengthMap[attackerName]/strengthMap[defenderName]);
		critThreshold += 20.0f*(spiritMap[attackerName]/spiritMap[defenderName]);

		if (critThreshold >= 80.0f)
		{
			critThreshold = 80.0f;
		}

		double randomRoll = Random.Range (0,100);

		return (randomRoll <= critThreshold);
	}

	private bool IsProjectileCrit(string attackerName, string defenderName)
	{
		float critThreshold = 3.0f;
		critThreshold += 10.0f*(strengthMap[attackerName]/strengthMap[defenderName]);
		critThreshold += 30.0f*(agilityMap[attackerName]/agilityMap[defenderName]);
		
		if (critThreshold >= 70.0f)
		{
			critThreshold = 70.0f;
		}
		
		double randomRoll = Random.Range (0,100);
		
		return (randomRoll <= critThreshold);
	}

	private bool IsMagicCrit(string attackerName, string defenderName)
	{
		float critThreshold = 3.0f;
		critThreshold += 15.0f*(intellectMap[attackerName]/intellectMap[defenderName]);
		critThreshold += 12.0f*(spiritMap[attackerName]/spiritMap[defenderName]);
		
		if (critThreshold >= 65.0f)
		{
			critThreshold = 65.0f;
		}
		
		double randomRoll = Random.Range (0,100);
		
		return (randomRoll <= critThreshold);
	}

	public float MeleeDamageRoll(string attackerName, string defenderName)
	{
		if (attackerName == "Zebulon (Clone)")
		{
			attackerName = "Dalton";
		}
		if (defenderName == "Zebulon (Clone)")
		{
			defenderName = "Dalton";
		}
		float maxDamage = 0.0f;

		maxDamage += weaponDamageMap[attackerName] * GetATK(attackerName)/GetDEF(defenderName);

		float baseDamage = Random.Range (0.85f * maxDamage, maxDamage);

		if (IsMeleeCrit(attackerName, defenderName))
		{
			Debug.Log ("Crit Proc!");
			return 1.5f*baseDamage;
		}
		else
		{
			return baseDamage;
		}
	}

	public float ProjectileDamageRoll(string attackerName, string defenderName)
	{
		if (attackerName == "Zebulon (Clone)")
		{
			attackerName = "Dalton";
		}
		if (defenderName == "Zebulon (Clone)")
		{
			defenderName = "Dalton";
		}
		float maxDamage = 0.0f;

		maxDamage += weaponDamageMap[attackerName] * GetATK(attackerName) / GetDEF(defenderName);

		float baseDamage = Random.Range (0.7f * maxDamage, maxDamage);

		if (IsProjectileCrit(attackerName, defenderName))
		{
			Debug.Log ("Crit Proc!");
			return 1.5f*baseDamage;
		}
		else
		{
			return baseDamage;
		}
	}

	public float MagicDamageRoll(string attackerName, string defenderName, string spellName)
	{
		if (attackerName == "Zebulon (Clone)")
		{
			attackerName = "Dalton";
		}
		if (defenderName == "Zebulon (Clone)")
		{
			defenderName = "Dalton";
		}
		float maxDamage = 0.0f;

		maxDamage += spellPowerMap[spellName]*GetMATK (spellName, attackerName)/GetMDEF(defenderName);

		float baseDamage = Random.Range (spellMinPercentMap[spellName] * maxDamage, maxDamage);

		if (IsMagicCrit(attackerName, defenderName))
		{
			Debug.Log ("Crit Proc!");
			return 1.5f*baseDamage;
		}
		else
		{
			return baseDamage;
		}
	}

	public float HealRoll(string casterName)
	{
		if (casterName == "Zebulon (Clone)")
		{
			casterName = "Dalton";
		}

		float maxHeal = 2.0f * intellectMap[casterName] + 2.6f * spiritMap[casterName];
		float healAmt = Random.Range (0.35f*maxHeal, maxHeal);
		return healAmt;
	}
}
