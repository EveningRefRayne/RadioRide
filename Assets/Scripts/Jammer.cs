using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jammer : MonoBehaviour {

	public int id;
	public bool active = true;

	private CircleCollider2D circleCollider;
	private Animator animator;
	private SpriteRenderer sRenderer;

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

	public static Jammer FindJammerById(int jammerId){
		foreach (GameObject jam in GameObject.FindGameObjectsWithTag("Jammer")) {
			if (jam.GetComponent<Jammer> ().id == jammerId) {
				return jam.GetComponent<Jammer> ();
			}
		}
		return null;
	}

	public static List<Jammer> FindJammersById(int jammerId){
		if (GameObject.FindGameObjectsWithTag ("Jammer").Length == 0) {
			return new List<Jammer>();
		}
		GameObject[] jammerObjects = GameObject.FindGameObjectsWithTag ("Jammer");
		List<Jammer> jammers = new List<Jammer>();
		Debug.Log (jammerObjects.Length);
		for(int x = 0; x < jammerObjects.Length; x++) {
			if (jammerObjects [x].GetComponent<Jammer> ().id == jammerId) {
				jammers.Add(jammerObjects [x].GetComponent<Jammer> ());
			}
		}
		Debug.Log (jammers.Count);
		return jammers;
	}

	public void SetActive(bool isActive){
		active = isActive;
		circleCollider.enabled = isActive;
		sRenderer.enabled = active;
		animator.enabled = active;
	}

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log ("Enter");
		if (other.gameObject.tag == "Player" && active) {
			other.gameObject.GetComponent<PlayerController> ().Die ();
		}
	}

}
