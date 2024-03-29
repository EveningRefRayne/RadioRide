﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState{
	Controllable,
	Transition1,
	Transition2,
	Transition3,
	Transition4,
	Transition5,
	Dying
}

public class PlayerController : MonoBehaviour
{

	public Transform startPoint;
	[Space(6)]

	[Header("Movement")]
	public float jumpPower = 100;
    public float moveSpeed = 10;
	public float wifiSpeed = 10;
    public float accelTime = 0.1f;
    public bool preJump = false;
    public bool canJump = false;
    public float jumpCD = 0f;
	public Rigidbody2D phys;
	public bool canMove = true;
	[Space(8)]

	[Header("Signal")]
    public bool inWifiRange = false;
	[Space(8)]

	[Header("Animation")]
	public float enterDishTime = 0.5f;
	public float transmissionTime = 1.0f;
	public float deathTime = 1.0f;
	public Animator anim;

	//Gamedriver and State
	private GameDriver gameDriver;
	public PlayerState state = PlayerState.Controllable;

	//switch
	private bool overSwitch = false;
	private EmiterSwitch eSwitch;

	//Transition
	private bool inTransitionRange = false;
	private TransitionDish dish;

	//button buffer
	private bool selectIsDown = false;

	//animation
	private Vector3 transitionStartPos;
	private float transTimer;
	private float lerpRate = 1f;
	private Vector3 transitionEndPos;
	private bool doneMoving = true;
	private float transitionTravelDis;
	private float scaleFactor = 1f;
	private float deathTimer = 0f;

	private int ignoreExit = 0;
    


    // Use this for initialization
    void Awake()
    {
		//GameObject.DontDestroyOnLoad (gameObject);

		if (startPoint != null) {
			this.transform.position = startPoint.position;
		}

		gameDriver = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameDriver> ();
        //phys = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

	void Update(){
		switch (state) {
		case PlayerState.Controllable:
			if (Input.GetAxis ("Interact") > 0) {
				if (!selectIsDown) {
					Debug.Log ("KeyPressed");
					selectIsDown = true;
					if (inTransitionRange) {
						Debug.Log ("TRANSMIT");
						if (gameDriver.currentLevel == gameDriver.levelCount) {
							Camera.main.GetComponent<FollowCam> ().enabled = false;
							Camera.main.transform.parent = this.transform;
							Camera.main.transform.localPosition = new Vector3 (0, 0, -10);
						}
						phys.velocity = Vector2.zero;
						phys.gravityScale = 0;
						transitionEndPos = dish.inPoint.position;
						transitionStartPos = transform.position;
						transTimer = enterDishTime;
						state = PlayerState.Transition1;
					} else if (overSwitch) {
						eSwitch.FlipSwitch ();
					}
				}
			} else {
				selectIsDown = false;
			}
			break;

		case PlayerState.Transition1:
			lerpRate = Mathf.Min(1f, 1f - transTimer / enterDishTime);
			this.transform.position = new Vector3 (Mathf.Lerp (transitionStartPos.x, transitionEndPos.x, lerpRate), Mathf.Lerp (transitionStartPos.y, transitionEndPos.y, lerpRate), 0);
			//scaleFactor = Mathf.Max (0.25f, Vector3.Distance (this.transform.position, transitionEndPos) / transitionStartDistance);
			//this.transform.localScale = new Vector3 (scaleFactor, scaleFactor, 1f);
			if (this.transform.position == transitionEndPos) {
				transitionEndPos = dish.transPoint.position;
				transitionStartPos = transform.position;
				//transitionStartDistance = Vector3.Distance (this.transform.position, transitionEndPos);
				//transitionTravelDis = transitionStartDistance / enterDishTime;
				transTimer = transmissionTime;
				state = PlayerState.Transition2;
				//animator.Play ();
			} else {
				transTimer -= Time.deltaTime;
			}
			break;

		case PlayerState.Transition2:
			lerpRate = Mathf.Min(1f, 1f - transTimer / transmissionTime);
			this.transform.position = new Vector3 (Mathf.Lerp (transitionStartPos.x, transitionEndPos.x, lerpRate), Mathf.Lerp (transitionStartPos.y, transitionEndPos.y, lerpRate), 0);
			//this.transform.position = Vector2.MoveTowards ((Vector2)this.transform.position, (Vector2)transitionEndPos , transitionTravelDis * Time.deltaTime);
			//this.transform.position = new Vector3 (transform.position.x, transform.position.y, 1f);
			if ((Vector2)this.transform.position == (Vector2)transitionEndPos) {
				transitionEndPos = dish.outPoint.position;
				transitionStartPos = transform.position;
				//transitionStartDistance = Vector3.Distance (this.transform.position, transitionEndPos);
				//transitionTravelDis = transitionStartDistance / transmissionTime ;
				//Camera.main.GetComponent<FollowCam> ().enabled = false;
				transTimer = enterDishTime;
				state = PlayerState.Transition3;
			}
			else {
				transTimer -= Time.deltaTime;
			}
			break;


		case PlayerState.Transition3:
			lerpRate = Mathf.Min (1, 1 - transTimer / enterDishTime);
			this.transform.position = new Vector3 (Mathf.Lerp (transitionStartPos.x, transitionEndPos.x, lerpRate), Mathf.Lerp (transitionStartPos.y, transitionEndPos.y, lerpRate), 0);
			//this.transform.position = Vector2.MoveTowards ((Vector2)this.transform.position, (Vector2)transitionEndPos, transitionTravelDis * Time.deltaTime);
			//scaleFactor = 0.4f; //Mathf.Min (0.4f, (1 - Vector3.Distance (this.transform.position, transitionEndPos) / transitionStartDistance)*0.4f);
			//this.transform.localScale = new Vector3 (scaleFactor, scaleFactor, 1);
			if (this.transform.position == transitionEndPos) {
				//phys.gravityScale = 1;
				//state = PlayerState.Controllable;
				gameDriver.GoToNextLevel ();
				//transform.position = GameObject.FindGameObjectWithTag ("StartPos").transform.position;
				//Camera.main.transform.parent = null;
				//Camera.main.GetComponent<FollowCam> ().enabled = true;
			}
			else{
				transTimer -= Time.deltaTime;
			}
			break;

		case PlayerState.Dying:
			if (deathTimer > 0) {
				gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.white, deathTimer%0.5f);
				deathTimer -= Time.deltaTime;
			} else {
				gameDriver.ReloadLevel ();
			}
			break;

		}
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if (state == PlayerState.Controllable)
        {
            if(phys.velocity.x>0)GetComponent<SpriteRenderer>().flipX = true;
            if (phys.velocity.x < 0) GetComponent<SpriteRenderer>().flipX = false;
            if (jumpCD > 0) jumpCD--;
			if (inWifiRange)
			{
				phys.velocity = new Vector2(Mathf.Lerp(phys.velocity.x, Input.GetAxis("Horizontal")*wifiSpeed, accelTime),
					Mathf.Lerp(phys.velocity.y, Input.GetAxis("Vertical")*wifiSpeed, accelTime));
				if (phys.velocity.y >= 0)
				{
					anim.SetInteger("animState", 3);//ascend
				}
				else if (phys.velocity.y < 0)
				{
					anim.SetInteger("animState", 4);//descend
				}
			}


			else
			{
				if (phys.velocity.x < 0)
				{
					if (Input.GetAxis("Horizontal") < 0)
					{
						phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed, 0), ForceMode2D.Force);
						if (canJump) anim.SetInteger("animState", 1);//walk
						else anim.SetInteger("animState", 3);//ascend
					}
					else if (Input.GetAxis("Horizontal") > 0)
					{
						phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed + phys.velocity.x*phys.mass*-5f, 0), ForceMode2D.Force);
						if (canJump) anim.SetInteger("animState", 1);//walk
						else anim.SetInteger("animState", 3);//ascend
					}
					else if (Input.GetAxis("Horizontal")==0)
					{
						phys.AddForce(new Vector2(phys.velocity.x*phys.mass*-20f,0), ForceMode2D.Force);
						anim.SetInteger("animState", 0);
					}
				}
				else if (phys.velocity.x > 0)
				{
					if (Input.GetAxis("Horizontal") < 0)
					{
						phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed + phys.velocity.x * phys.mass * -5f, 0), ForceMode2D.Force);
						if (canJump) anim.SetInteger("animState", 1);//walk
						else anim.SetInteger("animState", 3);//ascend
					}
					else if (Input.GetAxis("Horizontal") > 0)
					{
						phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed, 0), ForceMode2D.Force);
						if (canJump) anim.SetInteger("animState", 1);//walk
						else anim.SetInteger("animState", 3);//ascend
					}
					else if (Input.GetAxis("Horizontal") == 0)
					{
						phys.AddForce(new Vector2(phys.velocity.x * phys.mass * -20f, 0), ForceMode2D.Force);
						anim.SetInteger("animState", 0);//idle
					}
				}
				else if (phys.velocity.x == 0 && Input.GetAxis("Horizontal")!=0)
				{
					phys.AddForce(new Vector2(Input.GetAxis("Horizontal") * moveSpeed*20, 0), ForceMode2D.Force);
					if (canJump) anim.SetInteger("animState", 1);//walk
					else anim.SetInteger("animState", 3);//ascend
				}
				else if (phys.velocity.x==0 && Input.GetAxis("Horizontal")==0)
				{
					if (canJump) anim.SetInteger("animState", 0);
					else anim.SetInteger("animState", 3);//ascend
				}

				phys.velocity = Vector3.ClampMagnitude(phys.velocity, moveSpeed);
				//phys.velocity = new Vector2(Mathf.Lerp(phys.velocity.x, Input.GetAxis("Horizontal"), accelTime) * moveSpeed, phys.velocity.y);
				if (preJump == true)
				{
					preJump = false;
					canJump = false;
					jumpCD = 30;
					phys.AddForce(Vector2.up * jumpPower,ForceMode2D.Impulse);
					anim.SetInteger("animState", 3);//ascend
					//Debug.Log("Should be 3: " + anim.GetInteger("animState"));
					//Do the jump animation
				}
				if (Input.GetAxis("Vertical") > 0)
				{
					if (jumpCD > 0)
					{
						phys.AddForce(Vector2.up * jumpCD);
					}
					if (canJump && jumpCD==0)
					{
						if (preJump == false)
						{
							preJump = true;
							//Debug.Log("Jump Pressed, jumping prep");
							anim.SetInteger("animState", 3);//ascend
							//Debug.Log("Anim State: " + anim.GetInteger("animState"));
							//Do the prejump animation
						}
					}
				}
				else if(Input.GetAxis("Vertical")<=0)
				{
					jumpCD = 0;
				}
			}
			if(phys.velocity.y<0)
			{
				phys.AddForce(new Vector2(0, -18f * phys.mass));
				//Debug.Log("Falling, trigger falling anim");
				anim.SetInteger("animState", 4);//descend
				//Do the falling animation
			}
		}
       
    }

	public void SetSwitch(EmiterSwitch targetSwitch){
		Debug.Log ("Over switch");
		overSwitch = true;
        
		eSwitch = targetSwitch;
	}

	public void LeaveSwitch(){
		Debug.Log ("Leaveswitch");
		overSwitch = false;
		eSwitch = null;
	}

	public void SetInSignal(bool isIn){
		if (inWifiRange && isIn) {
			ignoreExit++;
		} else {
			inWifiRange = isIn;
			if (!inTransitionRange) {
				if (isIn) {
					this.gameObject.GetComponent<SpriteRenderer> ().color = Color.cyan;
                    GetComponent<Rigidbody2D>().gravityScale = 0.1f;
                } else {
					this.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
                    GetComponent<Rigidbody2D>().gravityScale = 1;
                }
			}
		}
	}

	public void SetInTransitionRange(bool isIn, TransitionDish targetDish){
		inTransitionRange = isIn;
		if (isIn) {
			dish = targetDish;
		} else if (inWifiRange) {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.cyan;
		} else {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
		}
	}

	public void Die(){
		phys.velocity = Vector2.zero;
		phys.gravityScale = 0;
		deathTimer = deathTime;
		state = PlayerState.Dying;
	}


	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "DeathBox") {
			gameDriver.ReloadLevel ();
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.tag == "Signal") {
			Debug.Log ("Exited Signal");
			ignoreExit--;
			if (ignoreExit <= 0) {
				SetInSignal (false);
			}
		} else if(other.gameObject.tag == "EmiterSwitch"){
			if(!gameObject.GetComponent<Collider2D> ().IsTouchingLayers (11)){
				LeaveSwitch();
			}
		}
	}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer==9)
        {
            bool isAbove = false;
            ContactPoint2D[] points = collision.contacts;
            foreach(ContactPoint2D point in points)
            {
                if(point.point.y<(transform.position.y-GetComponent<SpriteRenderer>().bounds.extents.y*0.5f))
                {
                    //Debug.Log("The point: " + point.point + " Is below: " + (transform.position.y - GetComponent<SpriteRenderer>().bounds.extents.y * 0.9f));
                    isAbove = true;
                    canJump = true;
                    if(Input.GetAxis("Horizontal")!=0)
                    {
                        anim.SetInteger("animState", 1);//walk
                    }
                    //phys.velocity = Vector2.zero;
                    anim.SetInteger("animState", 0);
                    //jumpCD = 0;
                }
            }
            //canJump = true;
            //jumpCD = 0;
            //Do the landing animation
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Debug.Log("Collision Exit");
        if (!GetComponent<CapsuleCollider2D>().IsTouchingLayers(512))
        {
            //Debug.Log("Not touching ground, can't jump.");
            canJump = false;
            anim.SetInteger("animState", 4);//descend
        }
    }
}
