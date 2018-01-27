﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jammer : MonoBehaviour {

	//id of linked Signal
	public int id;

	//is turned on
	private bool active = true;
	private bool playerOver = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool Active{
		get { return active; }
	}

	//called from player controller when "space" is pressed
	public void FlipSwitch(){
		Debug.Log ("Flip");
		active = !active;
		foreach(Signal sig in Signal.FindSignalsById(id)){
			sig.SetActive (!active);
		}
		if (active) {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.magenta;
		} else {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.black;
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "Player" && active) {
			//TODO: replace this with player controller
			other.GetComponent<TestingMove> ().SetJammer (this);
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.tag == "Player" && active) {
			//TODO: replace this with player controller
			other.GetComponent<TestingMove> ().LeaveJammer ();
		}
	}

}
