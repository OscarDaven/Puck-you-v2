using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class safespot : MonoBehaviour
{
	public const float MIN_DIST_SNAP_TO_SPOT = (float) 0.2;
	public const float IMPULSE_CONST = (float) 2.0;

	ContactFilter2D cf;
	Collider2D cl;
	Collider2D[] results;
	Rigidbody2D rb;
	float dist;
	PuckVars vars;
	int objects;

    // Start is called before the first frame update
    void Start()
	{
        cf = new ContactFilter2D();
        cl = GetComponent(typeof(Collider2D)) as Collider2D;
        results = new Collider2D[8];
    }

    // FixedUpdate is called in const time
    void FixedUpdate()
	{
        objects = cl.OverlapCollider(cf.NoFilter(), results);

		if (objects > 0)
		{
			for (int i = 0; i < objects; i++)
			{
				Debug.Log(results[i]);
				vars = results[i].gameObject.GetComponent(typeof(PuckVars)) as PuckVars;
				if (vars.isFallingIn)
				{
					Debug.Log("Now in loop");
                    float dist = Vector3.Distance(transform.position, results[i].gameObject.transform.position);
					Vector3 dir = transform.position - results[i].gameObject.transform.position;
					dir.Normalize();
					Vector2 impulse = (Vector2) dir;
					impulse *= (float) (IMPULSE_CONST / Mathf.Pow(dist, 2.0f));
					rb = results[i].gameObject.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
					Debug.Log(rb);
					rb.AddForce(impulse, ForceMode2D.Impulse);
					Debug.Log(impulse);
                }
			}
		}

    }

    // Applies a gravitational force to any collider that enters it, then removes the force and sets velocity to zero once it reaches the center of the safespot
    void OnTriggerEnter2D(Collider2D other)
	{
        vars = other.gameObject.GetComponent(typeof(PuckVars)) as PuckVars;
        Debug.Log("Entered " + other);
		vars.isFallingIn = true;
	}

	void OnTriggerStay2D(Collider2D other)
	{
        vars = other.gameObject.GetComponent(typeof(PuckVars)) as PuckVars;
        if (vars.isFallingIn)
		{
			dist = Vector3.Distance(transform.position, other.gameObject.transform.position);
			if (dist < MIN_DIST_SNAP_TO_SPOT)
			{
				other.gameObject.transform.position = transform.position;
				rb = other.GetComponent(typeof(Rigidbody2D)) as Rigidbody2D;
				rb.velocity = Vector2.zero;
				vars.isFallingIn = false;
				other.GetComponent<forceapplication>().timeTillShoot = 0f;
                other.GetComponent<forceapplication>().timeTillReset = 0f;
            }
		}
	}

    private void OnTriggerExit2D(Collider2D other)
    {
        vars = other.gameObject.GetComponent(typeof(PuckVars)) as PuckVars;
        Debug.Log("Exited " + other);
		vars.isFallingIn = false;
		other.GetComponent<forceapplication>().timeTillShoot = forceapplication.SHOOT_COOLDOWN;
    }
}
