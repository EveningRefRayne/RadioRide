using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState{
	Controllable,
	Transition1,
	Transition2,
	Transition3,
	Transition4,
	Transition5,
	Dying
}

public class TestingMove : MonoBehaviour {

	public Transform startPoint;
	public float speed = 0.5f;
	public float enterDishTime = 0.5f;
	public float transmissionTime = 1.0f;
	public float deathTime = 1.0f;
	public Animator animator;
	//public GameObject playerSprite;

	private GameDriver gameDriver;
	private PlayerState state = PlayerState.Controllable;

	private bool inSignal = false;
	private bool overJammer = false;
	private Jammer jammer;
	private bool inTransitionRange = false;
	private TransitionDish dish;

	private bool selectIsDown = false;

	private float transitionStartDistance;
	private Vector3 transitionEndPos;
	private bool doneMoving = true;
	private float transitionTravelDis;
	private float scaleFactor = 1f;

	private float deathTimer = 0f;

	// Use this for initialization
	void Start () {
		this.transform.position = startPoint.position;
		gameDriver = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameDriver> ();
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case PlayerState.Controllable:
			if (Input.GetAxis ("Horizontal") >= 0.001 || Input.GetAxis ("Horizontal") <= -0.001) {
				this.transform.position = new Vector3 (this.transform.position.x + speed * Input.GetAxis ("Horizontal"), this.transform.position.y, this.transform.position.z);
			}

			if (Input.GetAxis ("Interact") > 0) {
				if (!selectIsDown) {
					Debug.Log ("KeyPressed");
					selectIsDown = true;
					if (inTransitionRange) {
						Debug.Log ("TRNAS");
						transitionEndPos = dish.inPoint.position;
						transitionStartDistance = Vector3.Distance (this.transform.position, transitionEndPos);
						transitionTravelDis = transitionStartDistance / enterDishTime;
						state = PlayerState.Transition1;
					} else if (overJammer) {
						jammer.FlipSwitch ();
					}
				}
			} else {
				selectIsDown = false;
			}
			break;

		case PlayerState.Transition1:
			this.transform.position = Vector2.MoveTowards ((Vector2)this.transform.position, (Vector2)transitionEndPos, transitionTravelDis * Time.deltaTime);
			scaleFactor = Mathf.Max (0.25f, Vector3.Distance (this.transform.position, transitionEndPos) / transitionStartDistance);
			this.transform.localScale = new Vector3 (scaleFactor, scaleFactor, 1);
			if (this.transform.position == transitionEndPos) {
				transitionEndPos = dish.outPoint.position;
				transitionStartDistance = Vector3.Distance (this.transform.position, transitionEndPos);
				transitionTravelDis = transitionStartDistance / enterDishTime;
				state = PlayerState.Transition2;
				//animator.Play ();
			}
			break;

		case PlayerState.Transition2:
			this.transform.position = Vector2.MoveTowards ((Vector2)this.transform.position, (Vector2)transitionEndPos , transitionTravelDis * Time.deltaTime);
			this.transform.position = new Vector3 (transform.position.x, transform.position.y, 1f);
			if ((Vector2)this.transform.position == (Vector2)transitionEndPos) {
				transitionEndPos = dish.transPoint.position;
				transitionStartDistance = Vector3.Distance (this.transform.position, transitionEndPos);
				transitionTravelDis = transitionStartDistance / transmissionTime ;
				state = PlayerState.Transition3;
			}
			break;
			

		case PlayerState.Transition3:
			this.transform.position = Vector2.MoveTowards ((Vector2)this.transform.position, (Vector2)transitionEndPos, transitionTravelDis * Time.deltaTime);
			this.transform.position = new Vector3 (transform.position.x, transform.position.y, 1f);
			if ((Vector2)this.transform.position == (Vector2)transitionEndPos) {
				transitionEndPos = dish.inPoint.position;
				transitionStartDistance = Vector3.Distance (this.transform.position, transitionEndPos);
				transitionTravelDis = transitionStartDistance / enterDishTime;
				state = PlayerState.Transition4;
			}
			break;

		case PlayerState.Transition4:
			this.transform.position = Vector2.MoveTowards ((Vector2)this.transform.position, (Vector2)transitionEndPos, transitionTravelDis * Time.deltaTime);
			this.transform.position = new Vector3 (transform.position.x, transform.position.y, 1f);
			if ((Vector2)this.transform.position == (Vector2)transitionEndPos){
				transitionEndPos = dish.outPoint.position;
				transitionStartDistance = Vector3.Distance (this.transform.position, transitionEndPos);
				transitionTravelDis = transitionStartDistance / enterDishTime;
				this.transform.position = new Vector3 (transform.position.x, transform.position.y, 0f);
				state = PlayerState.Transition5;
			}
			break;


		case PlayerState.Transition5:
			this.transform.position = Vector2.MoveTowards ((Vector2)this.transform.position, (Vector2)transitionEndPos, transitionTravelDis * Time.deltaTime);
			scaleFactor = Mathf.Min (1, 1 - Vector3.Distance (this.transform.position, transitionEndPos) / transitionStartDistance);
			this.transform.localScale = new Vector3 (scaleFactor, scaleFactor, 1);
			if (this.transform.position == transitionEndPos) {
				gameDriver.GoToNextLevel ();
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

	public void SetJammer(Jammer targetJammer){
		Debug.Log ("Over Jammer");
		overJammer = true;
		jammer = targetJammer;
	}

	public void LeaveJammer(){
		Debug.Log ("LeaveJammer");
		overJammer = false;
		jammer = null;
	}

	public void SetInSignal(bool isIn){
		inSignal = isIn;
		if (!inTransitionRange) {
			if (isIn) {
				this.gameObject.GetComponent<SpriteRenderer> ().color = Color.cyan;
			} else {
				this.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
			}
		}
	}

	public void SetInTransitionRange(bool isIn, TransitionDish targetDish){
		inTransitionRange = isIn;
		if (isIn) {
			dish = targetDish;
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.blue;
		} else if (inSignal) {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.cyan;
		} else {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "MagneticField") {
			deathTimer = deathTime;
			state = PlayerState.Dying;
		}
	}
}
