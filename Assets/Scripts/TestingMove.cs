using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestingMove : MonoBehaviour {

	public float speed = 0.5f;
	public Animation transitionAnimation;

	private GameDriver gameDriver;
	private bool inSignal = false;
	private bool overJammer = false;
	private Jammer jammer;
	private TransitionDish dish;
	private bool selectIsDown = false;
	private bool inTransitionRange = false;
	private bool isTransitioning = false;

	// Use this for initialization
	void Start () {
		gameDriver = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameDriver> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!isTransitioning) {

			if (Input.GetAxis ("Horizontal") >= 0.001 || Input.GetAxis ("Horizontal") <= -0.001) {
				this.transform.position = new Vector3 (this.transform.position.x + speed * Input.GetAxis ("Horizontal"), this.transform.position.y, this.transform.position.z);
			}

			if (Input.GetAxis ("Interact") >= 0.001 && !selectIsDown) {
				Debug.Log ("KeyPressed");
				selectIsDown = true;
				if (inTransitionRange) {
					this.transform.parent = dish.transform;
					isTransitioning = true;
					transitionAnimation.Play ();
				} else if (overJammer) {
					selectIsDown = true;
				}
			} else {
				selectIsDown = false;
			}
		} else {
			if (!transitionAnimation.isPlaying) {
				gameDriver.GoToNextLevel ();
			}
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
