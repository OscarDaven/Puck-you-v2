using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using Unity.Netcode;

public class forceapplication : NetworkBehaviour
{

	public GameObject puck;
	// [SerializeField] public Transform puckTransform;

    // public Vector2 force = new Vector2(10.0f, 10.0f);

	public const float SHOOT_COOLDOWN = 1.0f;
	public const float RESET_TIME = 5.0f;

    public Vector2 force;
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

  public Canvas winScreenCanvas; 

	public float maxlength = 5;
	public float powerscale = 100;
	private Vector3 forcevec;
	 float curx;
	 float cury;
	 float pastx;
	 float pasty;
	public float maxTime;
	 float curTime;
	public float stopvel;
	private bool isdecreasing;
	private Vector3 lastvel;
	bool finished = false;
	ParticleSystem particles;
	private bool isdead = false;
	private SpriteRenderer spriteRenderer;
	private Vector3 initpos;
  
  
	bool timerRunning = false;
	float fulltime = .9f;
	float curfulltime;
	// public GameObject[] cameras;

	float timeTillShoot;
	float timeTillReset;
	Vector3 lastPos;
	Vector3 currentPos;
	Vector3 displacement;
    bool firstRun;

    void Start()
    {
			winScreenCanvas	= GameObject.FindGameObjectWithTag("Finish").GetComponent<Canvas>();

			winScreenCanvas.enabled = false;

			// if (!IsOwner) return;
		curfulltime = fulltime;
			curTime = maxTime;
		initpos = transform.position;
		spriteRenderer = GetComponent<SpriteRenderer>();
		particles = GetComponent<ParticleSystem>();
		particles.Stop();
		lastvel = new Vector3(0,0,transform.position.z);
		isdecreasing = false;
		forcevec = transform.position;
        lastpos = transform.position;
        lr = puck.AddComponent<LineRenderer>();
		camOffset = new Vector3(0, 0, transform.position.z);
		camera = Camera.main;

			GetrbServerRpc();




        rb = GetComponent<Rigidbody2D>();
		timeTillShoot = 0f;
		timeTillReset = 0f;
		lastPos = transform.position;
		currentPos = transform.position;
		displacement = Vector3.zero;
		firstRun = true;
	}


    // Update is called once per frame
    void Update()
    {
			if(timerRunning){
				curfulltime-=Time.deltaTime;
			}
			if (curfulltime <=0){
				timerRunning = false;
				DieServerRpc();
				transform.position = initpos;
				this.spriteRenderer.enabled = true;
				curfulltime = fulltime;
			}


			if (!IsOwner) return;

			GetrbServerRpc();
			rb = puck.GetComponent<Rigidbody2D>();


				
			if (finished)//triggered when someone reaches the goal
			{
				// SceneManager.LoadScene(2);//move to win screen
				winScreenCanvas.enabled = true;

				finished = false;
			}


		if (timeTillShoot > 0)
		{
			timeTillShoot -= Time.deltaTime;
			if (timeTillShoot <= 0)
			{
				timeTillReset = RESET_TIME;
			}
		}

		if (timeTillReset > 0)
		{
			timeTillReset -= Time.deltaTime;
			if (timeTillReset <= 0)
			{
                particles.Play();
                isdead = true;
                this.spriteRenderer.enabled = false;
                rb.velocity = new Vector2(0, 0);
                StartCoroutine(WaitToDie());
                timeTillReset = 0f;
                timeTillShoot = 0f;
            }
		}

        lastPos = currentPos;
        currentPos = transform.position;
        displacement = currentPos - lastPos;


        if (timeTillShoot <= 0 && !isdead)
        {
			if (Input.GetMouseButton(0))
			{
				if (firstRun)
				{
                    lr.enabled = true;
                    lr.positionCount = 2;
                    savepos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset;
                    lr.useWorldSpace = true;
                    lr.SetPosition(0, transform.position);
					firstRun = false;
                }
                lr.SetPosition(0, transform.position);
                //problem with snapping to corners
                Vector3 dif;
				savepos += displacement;
				curPos = camera.ScreenToWorldPoint(Input.mousePosition) + camOffset + displacement;
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
				cool = false;
				MovePuckServerRpc(forcevec);
				lr.enabled = false;
				forcevec = Vector3.zero;
				lr.SetPosition(1, transform.position);
				lr.SetPosition(0, transform.position);
				isstopped = false;
				forcevec = transform.position;

				timeTillReset = 0f;
				timeTillShoot = SHOOT_COOLDOWN;
				firstRun = true;
        
			}
			//if (noforceapp)
			//{
			//	rb.AddForce(force);
			//	noforceapp = false;
			//}
		}
			camera.transform.position = new Vector3(transform.position.x, transform.position.y, camera.transform.position.z);
    }   

	[ServerRpc(RequireOwnership = false)]
	public void SpikeCollideServerRpc(){
		particles.Play();
	  cool = false;

		this.spriteRenderer.enabled = false;
		rb.velocity = new Vector2(0,0);

		// puck.GetComponent<NetworkObject>().Despawn();
	}

	[ServerRpc(RequireOwnership = false)]
	public void DieServerRpc(){
	  transform.position = initpos;
	  cool = true;
		this.spriteRenderer.enabled = true;


		// puck.GetComponent<NetworkObject>().Despawn();
	}

	[ServerRpc(RequireOwnership = false)]
	public void MovePuckServerRpc(Vector3 forcevec){
		rb = puck.GetComponent<Rigidbody2D>();
		rb.AddForce(forcevec);
				// lr.enabled = false;
				// isstopped = false;
				// forcevec = transform.position;
	}	

	[ServerRpc(RequireOwnership = false)]
	public void GetrbServerRpc(){
		rb = puck.GetComponent<Rigidbody2D>();
	}

	IEnumerator WaitToSwitch()
	{
		yield return new WaitForSeconds(.4f);
        lr.enabled = false;
        finished = true;
	}

	// IEnumerator WaitToDie()
	// {
	// 	yield return new WaitForSeconds(.9f);
	// 	// isdead = false;
	// 	// transform.position = initpos;
	// 	// this.spriteRenderer.enabled = true;
	// 	DieServerRpc();
	// 	//Destroy(this); Change to reset to original position
	// }

	IEnumerator WaitToDie()
	{
		yield return new WaitForSeconds(.9f);
		isdead = false;
		lr.enabled = false;
		transform.position = initpos;
		this.spriteRenderer.enabled = true;
		forcevec = Vector3.zero;
		lr.SetPosition(0, transform.position);
		lr.SetPosition(1, transform.position);
	}

	void OnTriggerEnter2D(Collider2D other) //attempt to reset player position if moved outside of box
	{
		Debug.Log(other);
		if (other.gameObject.CompareTag("Goal")) //if  gameobject collides with another object with goal tag
		{
			StartCoroutine(WaitToSwitch());
		}
		else if (other.gameObject.CompareTag("Spike"))
		{
			Debug.Log("Puck collided with spike");
			// particles.Play();
			// isdead = true;
			// this.spriteRenderer.enabled = false;
			// rb.velocity = new Vector2(0, 0);
			SpikeCollideServerRpc();
			particles.Play();
		  cool = false;
			this.spriteRenderer.enabled = false;
			timerRunning = true;
			rb.velocity = new Vector2(0, 0);
			StartCoroutine(WaitToDie());
			timeTillReset = 0f;
			timeTillShoot = 0f;
		}
	}

}
