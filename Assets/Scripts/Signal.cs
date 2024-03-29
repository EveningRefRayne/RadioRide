﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour {

	public int id;
	public bool active = true;

	private Animator animator;
	private SpriteRenderer sRenderer;
	private CircleCollider2D circleCollider;
	private bool inRange = false;

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		animator.enabled = active;
		sRenderer = gameObject.GetComponent<SpriteRenderer> ();
		sRenderer.enabled = active;
		circleCollider = this.gameObject.GetComponent<CircleCollider2D> ();
		circleCollider.enabled = active;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static Signal FindSignalById(int signalId){
		foreach (GameObject sig in GameObject.FindGameObjectsWithTag("Signal")) {
			if (sig.GetComponent<Signal> ().id == signalId) {
				return sig.GetComponent<Signal> ();
			}
		}
		return null;
	}

	public static List<Signal> FindSignalsById(int signalId){
		if (GameObject.FindGameObjectsWithTag ("Signal").Length == 0) {
			return new List<Signal>();
		}
		GameObject[] signalObjects = GameObject.FindGameObjectsWithTag ("Signal");
		List<Signal> signals = new List<Signal>();
		Debug.Log (signalObjects.Length);
		for(int x = 0; x < signalObjects.Length; x++) {
			if (signalObjects [x].GetComponent<Signal> ().id == signalId) {
				signals.Add(signalObjects [x].GetComponent<Signal> ());
			}
		}
		Debug.Log (signals.Count);
		return signals;
	}

	public void SetActive(bool isActive){
		active = isActive;
		circleCollider.enabled = isActive;
		sRenderer.enabled = active;
		animator.enabled = active;
	}

	public void SetInRange(bool rangeSet){
		inRange = rangeSet;
		/*
		if (rangeSet) {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.green;
		} else {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.red;
		}
		*/
	}

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log ("Enter");
		if (other.gameObject.tag == "Player" && active) {
			SetInRange (true);
			//TODO: replace with player script
			other.gameObject.GetComponent<PlayerController>().SetInSignal(true);
		}
	}

	//TODO: Make it so that of the player is in range of another, it will still
	void OnTriggerExit2D(Collider2D other){
		Debug.Log ("Exit");
		if (other.gameObject.tag == "Player") {
			SetInRange (false);
		}
	}
}
