using UnityEngine;
using System.Collections;

public class ParticleEffectScript : MonoBehaviour {
	private CameraScript cScript;
	private int timeToDestroy;
	private bool initialized;
	private Vector3 attackLocation;
	private PlayerScript playerCaster;
	private EnemyScript attackTarget;
	CharacterController controller;
	private bool foundTarget;
	public float speed;
	public AudioClip fireballImpact;
	private AudioSource audioPlayer;
	// Use this for initialization
	void Start () {
		audioPlayer = gameObject.AddComponent<AudioSource>();
		initialized = false;
		//SelectAttackLocation ();
		controller = GetComponent<CharacterController>();

		if (attackLocation == null)
			foundTarget = false;

		speed = 15.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.gameObject.tag == "Explosion")
			explosion ();
		else if (this.gameObject.tag == "FireBall")
			Fireball ();
		else if (this.gameObject.tag == "IceBall")
			Iceball ();
		else if (this.gameObject.tag == "Lightning")
			Lightning ();

	}

	private void SelectAttackLocation()
	{
		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 150.0f);
		for (int i = 0; i < hitColliders.Length; i++) 
		{
			if(hitColliders[i].tag == "Player")
			{
				if(hitColliders[i].GetComponent<PlayerScript>().state != "Standby")
				{
					attackLocation = hitColliders[i].GetComponent<PlayerScript>().transform.position - this.transform.position;
					attackLocation.Normalize();
					foundTarget = true;
				}
			}
		}

	}

	private void CheckForCollision()
	{
		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, 1.0f);
		for (int i = 0; i < hitColliders.Length; i++) 
		{
			if(hitColliders[i].tag == "Enemy" || hitColliders[i].GetComponent<EnemyScript>() == attackTarget)
			{
				this.gameObject.SetActive(false);
			}
		}
	}

	private void MoveToTarget()
	{
		if (foundTarget == true) 
		{
			transform.position = Vector3.MoveTowards (transform.position, attackTarget.transform.position, speed*Time.deltaTime);
			//controller.Move (attackLocation * 3.0f * Time.deltaTime);

			if(this.tag == "FireBall" && audio.isPlaying == false){

			}

			if (Vector3.Distance (attackLocation, transform.position) < 1.0f) 
			{

				if(this.tag == "FireBall"){

				}
				Debug.Log ("Done");
				foundTarget = false;
				StatsScript sScript = GameObject.Find ("Menu Cursor").GetComponent<StatsScript>();
				float dmgRoll = sScript.MagicDamageRoll(playerCaster.gameObject.name, attackTarget.gameObject.name, playerCaster.actionChoice);
				attackTarget.TakeDamage (dmgRoll);
				playerCaster.endRangedTurn();
				cScript.ReturnToStandardMode();
				Destroy (gameObject, 1.5f);
			}
		}
	}

	private void DecrementLifetime()
	{
		if (initialized == false) 
		{
			timeToDestroy = 30;
			initialized = true;
		}
		if (timeToDestroy > 0) 
		{
			timeToDestroy--;
		}
		if (timeToDestroy == 0) 
		{
			StatsScript sScript = GameObject.Find ("Menu Cursor").GetComponent<StatsScript>();
			float dmgRoll = sScript.MagicDamageRoll(playerCaster.gameObject.name, attackTarget.gameObject.name, playerCaster.actionChoice);
			attackTarget.TakeDamage (dmgRoll);
			playerCaster.endRangedTurn();
			initialized = false;
			this.gameObject.SetActive(false);
		}
	}

	private void explosion()
	{
		DecrementLifetime ();
	}

	private void Fireball()
	{
		MoveToTarget();
	}

	private void Iceball()
	{
		MoveToTarget();
	}

	private void Lightning()
	{
		DecrementLifetime ();
	}

	public void SetTarget(PlayerScript caster, EnemyScript target)
	{
		attackTarget = target;
		playerCaster = caster;
		attackLocation = target.transform.position;
		initialized = true;
		foundTarget = true;
	}

	public void SetCamera(CameraScript cs)
	{
		cScript = cs;
	}
}
