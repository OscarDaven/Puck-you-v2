using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class forceapplication : MonoBehaviour
{
	public const float SHOOT_COOLDOWN = 1.0f;
	public const float RESET_TIME = 5.0f;

    public Vector2 force;
    Rigidbody2D rb;
    bool noforceapp = true;
    LineRenderer lr;
    Vector3 savepos;
    public new Camera camera;
    Vector3 camOffset;
    Vector3 curPos;
    Vector3 lastpos;
    bool isstopped = true;
	public float maxlength;
	public float powerscale;
	private Vector3 forcevec;
	public float stopvel;
	private bool isdecreasing;
	private Vector3 lastvel;
	bool finished = false;
	ParticleSystem particles;
	private bool isdead = false;
	private SpriteRenderer spriteRenderer;
	private Vector3 initpos;
	float timeTillShoot;
	float timeTillReset;
	Vector3 lastPos;
	Vector3 currentPos;
	Vector3 displacement;
    bool firstRun;

    void Start()
    {
		initpos = transform.position;
		spriteRenderer = GetComponent<SpriteRenderer>();
		particles = GetComponent<ParticleSystem>();
		particles.Stop();
		lastvel = new Vector3(0,0,transform.position.z);
		isdecreasing = false;
		forcevec = transform.position;
        lastpos = transform.position;
        lr = gameObject.AddComponent<LineRenderer>();
		camOffset = new Vector3(0, 0, transform.position.z);
		camera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
		timeTillShoot = 0f;
		timeTillReset = 0f;
		lastPos = transform.position;
		currentPos = transform.position;
		displacement = Vector3.zero;
		firstRun = true;
	}

    void FixedUpdate()
    {
		float curx = Mathf.Abs(rb.velocity.x);
		float cury = Mathf.Abs(rb.velocity.y);
		float pastx = Mathf.Abs(lastvel.x);
		float pasty = Mathf.Abs(lastvel.y);
		if ((curx < pastx) && (cury < pasty))//checks to see if the velocity vector of the puck is decreasing
		{
			isdecreasing = true;
		}
		//Debug.Log("decreasing " + isdecreasing);
		//Debug.Log("stopped" + isstopped);
		if ((isstopped ==false)&&(isdecreasing)&&(Mathf.Abs(rb.velocity.x) < stopvel)&&(Mathf.Abs(rb.velocity.y) < stopvel)){ //sometimes triggers right after you shoot, way to early
            isstopped = true;
			rb.velocity = new Vector3(0,0,transform.position.z);
			isdecreasing = false;
        }
		lastvel = rb.velocity;
    }

    // Update is called once per frame
    void Update()
    {
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

        if (finished)//triggered when someone reaches the goal
		{
			SceneManager.LoadScene(2);
			//move to win screen
			finished = false;
		}
        if ((isstopped || timeTillShoot <= 0) && !isdead)
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
				rb.AddForce(forcevec);
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

		}
    }

	IEnumerator WaitToSwitch()
	{
		yield return new WaitForSeconds(.4f);
        lr.enabled = false;
        finished = true;
	}

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
			particles.Play();
			isdead = true;
			this.spriteRenderer.enabled = false;
			rb.velocity = new Vector2(0, 0);
			StartCoroutine(WaitToDie());
			timeTillReset = 0f;
			timeTillShoot = 0f;
		}
	}
}
