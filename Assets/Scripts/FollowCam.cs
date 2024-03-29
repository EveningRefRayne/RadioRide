﻿using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {
    static public FollowCam S;
    public int delay = 25;
    public float ease = 0.15f;
    public float velocityEase = 0.05f;
    public bool wtfIsThis2;

	public float minX;
	//public float maxX;
	public float minY;
	//public float maxY;


    private Vector3 lookAhead;
	private Vector2 minXY;
    public GameObject poi;
    private float camZ;
    private Vector3 velocityOffset = Vector3.zero;
    private float groundBias;
	private bool returning = false;

    void Awake()
	{
		minXY = new Vector2(minX, minY);
        S = this;
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10);
        camZ = transform.position.z;
		//GameObject.DontDestroyOnLoad (gameObject);
		//Camera.main.GetComponent<FollowCam> ().enabled = true;
		//Camera.main.transform.parent = null;
    }

    void FixedUpdate()
    {
		Vector3 destination;
		if (poi == null)
		{
			destination = Vector3.Lerp (transform.position, Vector3.zero, ease);
			destination.z = camZ;
			transform.position = destination;
			if (returning)
			{
				
				Vector3 tempVec = transform.position;
				tempVec.z = 0;
				if (Vector3.Distance(tempVec, Vector3.zero) < 2)
				{
					returning = false;
				}
			}
		}
		else
		{
			velocityOffset.x = Mathf.Lerp (velocityOffset.x, poi.GetComponent<Rigidbody2D> ().velocity.x * 0.75f, velocityEase);
			velocityOffset.y = Mathf.Lerp (velocityOffset.y, poi.GetComponent<Rigidbody2D> ().velocity.y * 0.5f, velocityEase);
			lookAhead = poi.transform.position;
			lookAhead += velocityOffset;
			groundBias = 1 / (1 + Mathf.Exp (-0.1f * (GetComponent<Camera> ().orthographicSize - 10)));
			if (groundBias < .2)	groundBias = .2f;
			lookAhead.y = Mathf.Lerp (lookAhead.y, (poi.transform.position.y / 3) * 2, groundBias);
			destination = Vector3.Lerp (transform.position, lookAhead, ease);
			destination.x = Mathf.Max (minXY.x, destination.x);
			destination.y = Mathf.Max (minXY.y, destination.y);
		}
        destination.z = camZ;
        GetComponent<Camera>().orthographicSize = Mathf.Clamp(velocityOffset.magnitude/3 + 5,5,15) + 5;
        transform.position = destination;
    }
}
