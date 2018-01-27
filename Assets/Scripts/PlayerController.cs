using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float jumpPower = 1;
    public float moveSpeed = 1;
    public float accelTime = 0.1f;
    public bool canJump = false;
    public bool inWifiRange = false;
    private Rigidbody phys;


	// Use this for initialization
	void Awake ()
    {
        phys = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (inWifiRange) {
			phys.velocity = new Vector2 (Mathf.Lerp (phys.velocity.x, Input.GetAxis ("horizontal"), accelTime) * moveSpeed,
				Mathf.Lerp (phys.velocity.y, Input.GetAxis ("vertical"), accelTime) * moveSpeed);
		} 
	}
}
