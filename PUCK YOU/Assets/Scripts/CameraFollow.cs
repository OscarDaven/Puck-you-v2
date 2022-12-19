using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject Player;
    private Vector3 PlayerPos;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Player != null)
        {
			transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z);
		}

	}
}
