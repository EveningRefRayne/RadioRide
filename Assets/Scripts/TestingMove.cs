using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState{
	Controllable,
	Transition1,
	Transition2,
	Transition3
}

public class TestingMove : MonoBehaviour {

	public float speed = 0.5f;
	public float transitionSpeed = 0.5f;
	public Animation transitionAnimation;


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
	private float scaleFactor = 1f;

	// Use this for initialization
	void Start () {
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
						transitionEndPos = dish.inPos;
						transitionStartDistance = Vector3.Distance (this.transform.position, transitionEndPos);
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
			this.transform.position = Vector3.MoveTowards (this.transform.position, transitionEndPos, transitionSpeed * Time.deltaTime);
			scaleFactor = Mathf.Max (0.25f, Vector3.Distance (this.transform.position, transitionEndPos) / transitionStartDistance);
			this.transform.localScale = new Vector3 (scaleFactor, scaleFactor, 1);
			if (this.transform.position == transitionEndPos) {
				this.transform.parent = dish.transform;
				state = PlayerState.Transition2;
				transitionAnimation.Play ();
			}
			break;

		case PlayerState.Transition2:
			if (!transitionAnimation.isPlaying) {
				this.transform.parent = null;
				this.transform.position = dish.inPos;
				transitionEndPos = dish.outPos;
				transitionStartDistance = Vector2.Distance (this.transform.position, transitionEndPos);
				state = PlayerState.Transition3;
			}
			break;

		case PlayerState.Transition3:
			this.transform.position = Vector3.MoveTowards (this.transform.position, transitionEndPos, transitionSpeed * Time.deltaTime);
			scaleFactor = Mathf.Min (1, 1 - (Vector3.Distance (this.transform.position, transitionEndPos) / transitionStartDistance));
			this.transform.localScale = new Vector3 (scaleFactor, scaleFactor, 1);
			if (this.transform.position == transitionEndPos) {
				gameDriver.GoToNextLevel ();
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
}
