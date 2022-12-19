using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class forceapplication : NetworkBehaviour
{

	public GameObject puck;
	// [SerializeField] public Transform puckTransform;

    public Vector2 force = new Vector2(10.0f, 10.0f);
    Rigidbody2D rb;
    // bool noforceapp = true;
    LineRenderer lr;
    Vector3 savepos;
    public new Camera camera;
    Vector3 camOffset;
    Vector3 curPos;
    Vector3 lastpos;
    bool isstopped = true;
		bool cool = true;
	public float maxlength = 5;
	public float powerscale = 100;
	private Vector3 forcevec;
	public float stopvel = 0.5f;
	public bool isdecreasing;
  private Vector3 lastvel;
	 float curx;
	 float cury;
	 float pastx;
	 float pasty;
	public float maxTime;
	 float curTime;


	void Start()
    {

			if (!IsOwner) return;

			curTime = maxTime;
			Debug.Log("START");
			// puckTransform = (-0.66, -0.21, 0);

			// SpawnPuckServerRpc();

		lastvel = new Vector3(0,0, transform.position.z);
		isdecreasing = false;
		forcevec = transform.position;
        lastpos = transform.position;
        lr = puck.AddComponent<LineRenderer>();
		camOffset = new Vector3(0, 0, transform.position.z);
		camera = Camera.main;

			GetrbServerRpc();
			rb = puck.GetComponent<Rigidbody2D>();

		// SetlastvelServerRpc();

				// puck = rb.gameObject;
				// puckTransform = transform;


		//force = new Vector2((int)Random.Range(-1000, 1000), (int)Random.Range(-1000, 1000));
	}

	void OnTriggerEnter2D (Collider2D colliderObject){
		if (colliderObject.gameObject.tag == "Spike"){
			Debug.Log("Puck collided with spike");
			// Destroy(rb.gameObject);
			SpikeCollideServerRpc();
		}
	}

 //   void OnMouseDown()
 //   {
 //       noforceapp = true;
	//	force = new Vector2((int)Random.Range(-1000, 1000), (int)Random.Range(-1000, 1000));
 //       GameObject scorobj = GameObject.FindGameObjectWithTag("Score");
 //       addscore script = scorobj.GetComponent<addscore>();
 //       script.updatescore();
	//}

    // void FixedUpdate()
    // {

		// 	if (!IsOwner) return;

		// 	GetrbServerRpc();
		// 	// rb = puck.GetComponent<Rigidbody2D>();

		// 	// lastvel = puck.GetComponent<forceapplication>().lastvel;
		// 	// stopvel = puck.GetComponent<forceapplication>().stopvel;



		// 	SetcurvelServerRpc();

		// 	// float curx = Mathf.Abs(rb.velocity.x);
		// 	// float cury = Mathf.Abs(rb.velocity.y);


		// 	if ((curx < pastx) && (cury < pasty))//checks to see if the velocity vector of the puck is decreasing
		// 	{
		// 		isdecreasing = true;
		// 	}
		// 	// Debug.Log("decreasing " + isdecreasing);
		// 	// Debug.Log("stopped" + isstopped);
		// 	if ((isstopped == false)&&(isdecreasing)&&(Mathf.Abs(rb.velocity.x) < stopvel)&&(Mathf.Abs(rb.velocity.y) < stopvel)){ //sometimes triggers right after you shoot, way too early
				
		// 		isstopped = true;

		// 		SetrbZeroVelocityServerRpc();
		// 		// rb.velocity = new Vector3(0,0,transform.position.z);
		// 		isdecreasing = false;
		// 	}

		// 	// lastvel = rb.velocity;
		// 	SetlastvelServerRpc();
		// 	// printrbvelServerRpc(rb.velocity);
		// }

    // Update is called once per frame
    void Update()
    {

			if (!cool){
				curTime -= Time.deltaTime;
			}
			if (curTime <= 0){
				cool = true;
			}

			if (!IsOwner) return;

			GetrbServerRpc();
			rb = puck.GetComponent<Rigidbody2D>();


      if (cool)
      {
				
			if (Input.GetMouseButtonDown(0))
			{
				Debug.Log("MBDOWN");
				lr.enabled = true;
				lr.positionCount = 2;
				savepos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset;
				lr.useWorldSpace = true;
				lr.SetPosition(0, transform.position);
			}
			if (Input.GetMouseButton(0))
			{
				//problem with snapping to corners
				Vector3 dif;
				curPos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset;
				dif = curPos - savepos;
				float linelen = Mathf.Sqrt((dif.x * dif.x) + (dif.y * dif.y)); //calculates length of line between cursor and start point
				if (linelen > maxlength)//checks to see if cursor is out of circle area
				{
					Vector3 refangle = new Vector3(linelen, 0, 0);
					float angle;
					if (dif.y < 0) //calculates the angle using a flat like at 0 degrees as a reference
					{
						angle =  (360 - Vector3.Angle(refangle, dif)) * (Mathf.PI / 180);
					}
					else
					{
						angle = Vector3.Angle(refangle, dif) * (Mathf.PI / 180);
					}
					float newx = transform.position.x + (maxlength * Mathf.Cos(angle));//calc x value
					float newy = transform.position.y + (maxlength * Mathf.Sin(angle));//calc y value
					Vector3 fullpos = new Vector3(newx, newy, 0);
					forcevec = fullpos;
					lr.SetPosition(1, fullpos);
				}
				else //handles in-circle positioning
				{
					lr.SetPosition(1, transform.position + dif);
					forcevec = (transform.position + dif);
				}
				

			}
			if (Input.GetMouseButtonUp(0))
			{
				//forces sometimes move the ball in the wrong direction
				forcevec = forcevec - transform.position;
				// changed it to negative so it goes in the opposite direction from where the mouse is pulled 
				forcevec.x = -(forcevec.x * powerscale); //scales up the power based on the mutable powerscale variable
				forcevec.y = -(forcevec.y * powerscale);
				forcevec = forcevec + transform.position;
				// rb.AddForce(forcevec);
				MovePuckServerRpc(forcevec);
				lr.enabled = false;
				isstopped = false;
				forcevec = transform.position;
				cool = false;
				curTime = maxTime;
			}
			//if (noforceapp)
			//{
			//	rb.AddForce(force);
			//	noforceapp = false;
			//}
		}
    }   

	[ServerRpc(RequireOwnership = false)]
	private void SpikeCollideServerRpc(){
		Debug.Log("HELLLLLLLLLLLLO");
		// Destroy(puck);
		puck.GetComponent<NetworkObject>().Despawn();
	}

	[ServerRpc(RequireOwnership = false)]
	private void MovePuckServerRpc(Vector3 forcevec){
		rb = puck.GetComponent<Rigidbody2D>();
		Debug.Log("MOVING");
		rb.AddForce(forcevec);
				// lr.enabled = false;
				// isstopped = false;
				// forcevec = transform.position;
	}

	[ServerRpc(RequireOwnership = false)]
	private void GetrbServerRpc(){
		rb = puck.GetComponent<Rigidbody2D>();
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetrbZeroVelocityServerRpc(){
		Debug.Log("SETTING ZERO VEL");
		rb.velocity = new Vector3(0,0,transform.position.z);
		isstopped = true;
	}



	[ServerRpc(RequireOwnership = false)]
	private void SetlastvelServerRpc(){
		lastvel = rb.velocity;
		pastx = Mathf.Abs(lastvel.x);
		pasty = Mathf.Abs(lastvel.y);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetcurvelServerRpc(){
		curx = Mathf.Abs(rb.velocity.x);
		cury = Mathf.Abs(rb.velocity.y);	
		Debug.Log(curx);
	}



	[ServerRpc(RequireOwnership = false)]
	private void printrbvelServerRpc(Vector3 pastx){
		Debug.Log(pastx);
	}

	
	
	// [ServerRpc]
	// private void SpawnPuckServerRpc(){
	// 	// Instantiate(puck, new Vector3(0, 0, 0), Quaternion.identity);
	// 	puck.GetComponent<NetworkObject>().Despawn();
	// }

}



				//if (Mathf.Abs(dif.x) > Mathf.Abs(dif.y)) //abs is used to handle negative edge cases
				//{
				//	if (dif.x > maxlength)
				//	{
				//		dif.y = (Mathf.Abs((maxlength / dif.x)) * dif.y);
				//		dif.x = maxlength;
				//	}
				//	else if (dif.x < (maxlength * -1))
				//	{
				//		dif.y = (Mathf.Abs((maxlength / dif.x)) * dif.y);
				//		dif.x = -1 * maxlength;
				//	}
				//}else if (Mathf.Abs(dif.y) > Mathf.Abs(dif.x))
				//{
				//	if (dif.y > maxlength)
				//	{
				//		dif.x = (Mathf.Abs((maxlength / dif.y)) * dif.x);
				//		dif.y = maxlength;
				//	}
				//	else if (dif.y < (maxlength * -1))
				//	{
				//		dif.x = (Mathf.Abs((maxlength / dif.y)) * dif.x);
				//		dif.y = (maxlength * -1);
				//	}
				//}
				//else
				//{
				//	float midlen = maxlength / 2;
				//	float linelen = (midlen * midlen) + (midlen * midlen);
				//	if ((dif.x > maxlength) && (dif.y > maxlength))
				//	{
				//		dif.x = midlen;
				//		dif.y = midlen;
				//	}
				//	else if ((dif.x > maxlength) && (dif.y < -1 * maxlength))
				//	{
				//		dif.x = midlen;
				//		dif.y = -1 * midlen;
				//	}
				//	else if ((dif.x < -1 * maxlength) && (dif.y > maxlength))
				//	{
				//		dif.y = midlen;
				//		dif.x = -1 * midlen;
				//	}
				//	else if ((dif.x < -1 * maxlength) && (dif.y < -1 * maxlength))
				//	{
				//		dif.x = -1 * midlen;
				//		dif.y = -1 * midlen;
				//	}
				//}


				//if ((dif.x > maxlength)&&(dif.y > maxlength)){
				//	dif.x = maxlength;
				//	dif.y = maxlength;
				//}
				//else if((dif.x > maxlength) && (dif.y < -1* maxlength))
				//{
				//	dif.x = maxlength;
				//	dif.y = -1* maxlength;
				//}
				//else if ((dif.x < -1* maxlength) && (dif.y > maxlength))
				//{
				//	dif.y = maxlength;
				//	dif.x = -1 * maxlength;
				//}
				//else if ((dif.x < -1* maxlength) && (dif.y < -1 * maxlength))
				//{
				//	dif.x = -1 * maxlength;
				//	dif.y = -1 * maxlength;
				//}
				//else if (dif.x > maxlength)
				//{
				//	dif.y = (Mathf.Abs((maxlength / dif.x)) * dif.y);
				//	dif.x = maxlength;
				//}
				//else if (dif.x < (maxlength * -1))
				//{
				//	dif.y = (Mathf.Abs((maxlength / dif.x)) * dif.y);
				//	dif.x = -1 * maxlength;
				//}
				//if (dif.y > maxlength)
				//{
				//	dif.x = (Mathf.Abs((maxlength / dif.y)) * dif.x);
				//	dif.y = maxlength;
				//}
				//if (dif.y < (maxlength * -1))
				//{
				//	dif.x = (Mathf.Abs((maxlength / dif.y)) * dif.x);
				//	dif.y = (maxlength * -1);
				//}