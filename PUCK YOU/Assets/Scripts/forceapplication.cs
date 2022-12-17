using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;

public class forceapplication : MonoBehaviour
{
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
		//force = new Vector2((int)Random.Range(-1000, 1000), (int)Random.Range(-1000, 1000));
	}

 //   void OnMouseDown()
 //   {
 //       noforceapp = true;
	//	force = new Vector2((int)Random.Range(-1000, 1000), (int)Random.Range(-1000, 1000));
 //       GameObject scorobj = GameObject.FindGameObjectWithTag("Score");
 //       addscore script = scorobj.GetComponent<addscore>();
 //       script.updatescore();
	//}

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
		if (finished)//triggered when someone reaches the goal
		{
			SceneManager.LoadScene(2);//move to win screen
			finished = false;
		}
        if (isstopped || isdead)
        {
			if (Input.GetMouseButtonDown(0))
			{
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
				isstopped = false;
				forcevec = transform.position;
			}
			//if (noforceapp)
			//{
			//	rb.AddForce(force);
			//	noforceapp = false;
			//}
		}
    }

	IEnumerator WaitToSwitch()
	{
		yield return new WaitForSeconds(.4f);
		finished = true;
	}

	IEnumerator WaitToDie()
	{
		yield return new WaitForSeconds(.9f);
		isdead = false;
		transform.position = initpos;
		this.spriteRenderer.enabled = true;
		//Destroy(this); Change to reset to original position
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
		}
	}
}
