using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraScript : MonoBehaviour 
{
	/*
	Some general trends found with experimentation:
	1) 7 seems to be the "magic number" for the x-coordinate offset for 
	good close-up shots of human-sized characters

	2) 4 seems to be a good y coordinate when doing said close-up

	3) (-10, 87, 0) works well for the rotation vector for hero close-ups,
	while (-10, 267, 0) works well for said vector with enemy close-ups

	4) The position of (60, 7, 60) works well for a side view.  The rotation
	should go between (10, -30, 0) and (10, 30, 0) for enemies and players, 
	respectively.

	5) The following are the position coordinates:
	Enemy1 position: (50, 0, 80)
	Enemy2 position: (50, 0, 90)

	Player2 position: (70, 0, 70)
	Player1 position: (70, 0, 80)

	6) Battle "ring" looks like the following:
	E1<--W-->E2
	^         ^ 
	|         |
	L         L
	|         |
	v         v
	P2<--W-->P1


	Width = 10
	Length = 20

	 */


	private readonly Vector3 naturalPositionGrass = new Vector3(80.0f,10.0f,65.0f);
	private readonly Vector3 naturalRotationGrass = new Vector3(27.0f,293.0f,0.0f);
	private readonly Vector3 naturalScale = new Vector3(0.6f,1.2f,-1.0f);

	private readonly Vector3 naturalPositionSnow = new Vector3(137.0f, 8.0f, 69.0f);
	private readonly Vector3 naturalRotationSnow = new Vector3(9.0f, 290.0f, 0.0f);
	private readonly Vector3 naturalScaleSnow = new Vector3(0.97f, 0.97f, 0.97f);

	private readonly Vector3 playerCloseUpRotation = new Vector3(350.0f, 87.0f, 0.0f);
	private readonly Vector3 enemyCloseUpRotation = new Vector3(350.0f, 267.0f, 0.0f);

	private readonly Vector3 swivelBaseRotation = new Vector3(10.0f, 0.0f, 0.0f);

	private Vector3 swivelPosition;

	private const float rotationLimit = 70.0f;

	public const float rotateSpeed = 1.6f;
	private Vector3 rotation;
	private float swivelSignMultiplier;
	private string state;

	private ParticleEffectScript magicAttackTarget;

	private const float xCloseupOffset = 7.0f;
	private const float yCloseup = 4.0f;
	private const float zSwivelOffset = 20.0f;
	private const float ySwivel = 7.0f;

	private float battlefieldMidpointX;

	private const int closeUpTime = 0;
	private int closeUpTimer;

	private GameObject turnTaker;
	private bool turnTakerIsPlayer;

	// Use this for initialization
	void Start () 
	{
		swivelSignMultiplier = 1.0f;
		if (Application.loadedLevelName == "GrasslandsBattle")
			rotation = new Vector3(naturalRotationGrass.x, naturalRotationGrass.y, naturalRotationGrass.z);
		else if (Application.loadedLevelName == "snowScene")
		{
			rotation = new Vector3(naturalRotationSnow.x, naturalRotationSnow.y, naturalRotationSnow.z);
		}
		closeUpTimer = closeUpTime;

		state = "Default";

		battlefieldMidpointX = GetMidpoint ();
		swivelPosition = new Vector3(battlefieldMidpointX, ySwivel, 0.0f);
	}

	private float GetMidpoint()
	{
		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		
		float avgPlayerxPosition = 0.0f;
		float avgEnemyxPosition = 0.0f;

		for (int i = 0; i < enemies.Length; i++)
		{
			avgEnemyxPosition += enemies[i].transform.position.x;
		}
		avgEnemyxPosition /= enemies.Length;

		for (int j = 0; j < players.Length; j++)
		{
			avgPlayerxPosition += players[j].transform.position.x;
		}
		avgPlayerxPosition /= players.Length;

		return (avgPlayerxPosition+avgEnemyxPosition)/2.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (state == "Default")
		{
			SlowRotation ();
		}
		else if (state == "TrackingMelee")
		{
			MeleeTrackingBehaviour();
		}
		else if (state == "TrackingMagicAttack")
		{
			MagicTrackingBehaviour();
		}
	}

	private void SlowRotation()
	{
		float rotationAmount = 0.0f;
		if (Application.loadedLevelName == "GrasslandsBattle")
		{
			rotationAmount = Mathf.Abs (naturalRotationGrass.y - rotation.y);
		}
		else if (Application.loadedLevelName == "snowScene")
		{
			rotationAmount = Mathf.Abs (naturalRotationSnow.y - rotation.y);
		}

		if (rotationAmount > rotationLimit)
		{
			swivelSignMultiplier *= -1.0f;
		}

		float rotateToY = rotation.y + swivelSignMultiplier * rotateSpeed;

		rotation.y = rotateToY;

		transform.Rotate (Vector3.up, Time.deltaTime * rotateSpeed * swivelSignMultiplier, Space.World);
	}

	private void MeleeTrackingBehaviour()
	{
		Swivel (turnTaker.transform.position);
	}

	private void MagicTrackingBehaviour()
	{
		if (closeUpTimer <= 0)
		{
			Swivel (magicAttackTarget.transform.position);
		}
		else
		{
			CloseUpShot();
		}
	}

	private void CloseUpShot()
	{
		closeUpTimer--;
		if (closeUpTimer <= 0)
		{
			transform.position = swivelPosition;
			transform.eulerAngles =  (swivelBaseRotation);
			if (magicAttackTarget != null)
			{
				Swivel (magicAttackTarget.transform.position);
			}
			else
			{
				Swivel (turnTaker.transform.position);
			}
		}
	}

	private void SideCloseUpShot()
	{
		closeUpTimer--;
	}

	private void Swivel(Vector3 location)
	{
		Vector3 pointDirection = location - transform.position;
		transform.rotation = Quaternion.LookRotation(pointDirection);

		/*float angleBetween = Vector3.Angle (pointDirection, transform.forward);
		transform.Rotate (Vector3.up, angleBetween, Space.World);*/
	}

	public void FollowMagicAttack(GameObject caster, ParticleEffectScript spellEffect)
	{
		closeUpTimer = closeUpTime;
		turnTaker = caster;
		magicAttackTarget = spellEffect;

		if (caster.tag == "Player")
		{
			float xPosition = caster.transform.position.x - xCloseupOffset;
			swivelPosition.z = caster.transform.position.z - zSwivelOffset;

			/*Vector3 newPosition = new Vector3(xPosition, yCloseup, caster.transform.position.z);
			transform.position = newPosition;*/
			transform.eulerAngles =  (playerCloseUpRotation);

			transform.position = swivelPosition;

			swivelPosition.z = caster.transform.position.z - zSwivelOffset;
		}
		else if (caster.tag == "Enemy")
		{
			float xPosition = caster.transform.position.x + xCloseupOffset;
			Vector3 newPosition = new Vector3(xPosition, yCloseup, caster.transform.position.z);
			transform.position = newPosition;
			transform.eulerAngles =  (enemyCloseUpRotation);

			swivelPosition.z = caster.transform.position.z - zSwivelOffset;
		}

		state = "TrackingMagicAttack";
	}

	public void FollowProjectileAttack(GameObject shooter)
	{
		turnTaker = shooter;
		closeUpTimer = closeUpTime;
		if (shooter.tag == "Player")
		{
			float xPosition = shooter.transform.position.x - xCloseupOffset;
			Vector3 newPosition = new Vector3(xPosition, yCloseup, shooter.transform.position.z);
			transform.eulerAngles =  (enemyCloseUpRotation);
			
			swivelPosition.z = shooter.transform.position.z - zSwivelOffset;
		}
		else if (shooter.tag == "Enemy")
		{
			float xPosition = shooter.transform.position.x + xCloseupOffset;
			Vector3 newPosition = new Vector3(xPosition, yCloseup, shooter.transform.position.z);
			transform.eulerAngles =  (enemyCloseUpRotation);
			
			swivelPosition.z = shooter.transform.position.z - zSwivelOffset;
		}

		state = "TrackingProjectile";
	}

	public void FollowMeleeAttack(GameObject attacker)
	{
		turnTaker = attacker;
		swivelPosition.z = attacker.transform.position.z - zSwivelOffset;
		transform.position = swivelPosition;
		transform.eulerAngles =  (swivelBaseRotation);
		Swivel (attacker.transform.position);
		state = "TrackingMelee";
	}

	public void ReturnToStandardMode()
	{
		if (state != "Default")
		{
			magicAttackTarget = null;
			turnTaker = null;
			if (Application.loadedLevelName == "GrasslandsBattle")
			{
				transform.position = naturalPositionGrass;
				transform.eulerAngles =  (naturalRotationGrass);
				transform.localScale = naturalScale;
			}
			else if (Application.loadedLevelName == "snowScene")
			{
				transform.position = naturalPositionSnow;
				transform.eulerAngles =  (naturalRotationSnow);
				transform.localScale = naturalScaleSnow;
			}


			state = "Default";
		}
	}
}
