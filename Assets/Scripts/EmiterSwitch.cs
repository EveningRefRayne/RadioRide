using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmiterSwitch : MonoBehaviour {

	//id of linked Signal
	public int id;
	public bool active = true;

	public Sprite onSprite;
	public Sprite offSprite;

	//is turned on
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
		foreach(Jammer jam in Jammer.FindJammersById(id)){
			jam.SetActive (!active);
		}
		if (active) {
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = onSprite;
		} else {
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = offSprite;
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "Player" && active) {
			//TODO: replace this with player controller
			other.GetComponent<PlayerController> ().SetSwitch (this);
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.tag == "Player" && active) {
			//TODO: replace this with player controller
			other.GetComponent<PlayerController> ().LeaveSwitch ();
		}
	}

}
