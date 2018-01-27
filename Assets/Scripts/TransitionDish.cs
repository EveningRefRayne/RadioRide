using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionDish : MonoBehaviour {

	public Transform inPoint;
	public Transform  outPoint;


	private bool playerInRange = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.gameObject.tag == "Player") {
			//TODO: replace with player script
			other.gameObject.GetComponent<TestingMove>().SetInTransitionRange(true, this);
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.gameObject.tag == "Player") {
			//TODO: replace with player script
			other.gameObject.GetComponent<TestingMove>().SetInTransitionRange(false, this);
		}
	}
}
