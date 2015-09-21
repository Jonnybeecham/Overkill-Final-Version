//This is free to use and no attribution is required
//No warranty is implied or given
using UnityEngine;
using System.Collections;

[RequireComponent (typeof(LineRenderer))]

public class LaserScript : MonoBehaviour {
	
	public float laserWidth = 0.4f;
	public float noise = 0.02f;
	public float maxLength = 50.0f;
	public Color color = Color.white;

	private const int ttl = 20;
	private int destroyTimer = ttl;

	private bool initialized = false;
	
	LineRenderer lineRenderer;
	int length;
	Vector3[] position;
	//Cache any transforms here
	Transform myTransform;
	Transform endEffectTransform;
	//The particle system, in this case sparks which will be created by the Laser
	public ParticleSystem endEffect;
	Vector3 offset;
	private const float cachedYvalue = 3.0f;
	private readonly Vector3 basePosition = new Vector3(0.96f, 4.5f, 0.8f);
	

	// Use this for initialization
	void Start () {
		//transform.position = new Vector3(transform.position.x, cachedYvalue, transform.position.z);
		destroyTimer = 0;
		lineRenderer = GetComponent<LineRenderer>();
		lineRenderer.SetWidth(laserWidth, laserWidth);
		myTransform = transform;
		offset = new Vector3(0,0,0);
		endEffect = GetComponentInChildren<ParticleSystem>();
		if(endEffect)
			endEffectTransform = endEffect.transform;
	}

	// Update is called once per frame
	void Update () {
		if (!initialized)
		{
			destroyTimer = ttl;
		}

		if (destroyTimer >= 0)
		{
			destroyTimer--;
			RenderLaser();
		}
		//Debug.Log (destroyTimer);
		else if (initialized)
		{
			//transform.rotation = transform.parent.rotation;
			//Hide ();
			Destroy (this.gameObject);
		}
	}
	
	void RenderLaser(){
		
		//Shoot our laserbeam forwards!
		UpdateLength();
		
		lineRenderer.SetColors(color,color);
		//Move through the Array
		for(int i = 0; i<length; i++){
			//Set the position here to the current location and project it in the forward direction of the object it is attached to
			offset.x =myTransform.position.x+i*myTransform.forward.x+Random.Range(-noise,noise);
			offset.z =i*myTransform.forward.z+Random.Range(-noise,noise)+myTransform.position.z;
			position[i] = offset;
			position[0] = myTransform.position;
			
			lineRenderer.SetPosition(i, position[i]);
			
		}
		
		
		
	}
	
	void UpdateLength(){
		//Raycast from the location of the cube forwards
		RaycastHit[] hit;
		hit = Physics.RaycastAll(myTransform.position, myTransform.forward, maxLength);
		int i = 0;
		while(i < hit.Length){
			//Check to make sure we aren't hitting triggers but colliders
			if(!hit[i].collider.isTrigger && hit[i].collider.tag == "Enemy")
			{
				Vector3 temp = hit[i].point;
				temp.y += cachedYvalue;
				//temp.point.y += cachedYvalue;
				length = (int)Mathf.Round (Vector3.Distance (temp, transform.position))+2;
				//length = (int)Mathf.Round(temp.distance)+2;
				position = new Vector3[length];
				//Move our End Effect particle system to the hit point and start playing it
				if(endEffect){
					endEffectTransform.position = hit[i].point;
					if(!endEffect.isPlaying)
						endEffect.Play();
				}
				lineRenderer.SetVertexCount(length);
				return;
			}
			i++;
		}
		//If we're not hitting anything, don't play the particle effects
		if(endEffect){
			if(endEffect.isPlaying)
				endEffect.Stop();
		}
		length = (int)maxLength;
		position = new Vector3[length];
		lineRenderer.SetVertexCount(length);
		
		
	}
	public void Swivel(Vector3 pointDirection)
	{
		Debug.Log (transform.forward);
		Debug.Log (pointDirection);
		pointDirection.Normalize ();
		float angleBetween = (Vector3.Angle (transform.forward, pointDirection))%360.0f;
		Debug.Log (angleBetween);
		//Vector3 pointDirection = location - transform.position;
		//Debug.Log (pointDirection);
		//transform.rotation = Quaternion.LookRotation(pointDirection);
		//transform.forward = pointDirection;
		//myTransform.forward = pointDirection;
		//float angleBetween = Vector3.Angle (pointDirection, transform.forward);
		transform.Rotate (Vector3.up, angleBetween, Space.World);
	}

	public void Hide()
	{
		gameObject.SetActive (false);
	}

	/*public void Shoot(Vector3 targetVector)
	{
		gameObject.SetActive (true);
		lineRenderer.SetWidth(laserWidth, laserWidth);
		offset = new Vector3(0,0,0);
		Swivel (targetVector);
		length = 0;
	}*/

	public void Shoot(float angleToRotate)
	{
		gameObject.SetActive (true);
		lineRenderer = GetComponent<LineRenderer>();
		initialized = true;
		transform.Rotate (Vector3.up, angleToRotate, Space.World);
		myTransform = transform;
		length = 0;
		lineRenderer.SetWidth(laserWidth, laserWidth);
		destroyTimer = ttl;

		offset = new Vector3(0,0,0);
		endEffect = GetComponentInChildren<ParticleSystem>();
		if(endEffect)
			endEffectTransform = endEffect.transform;

		Debug.Log (transform.forward);
	}
}