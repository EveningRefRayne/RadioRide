using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingMove : MonoBehaviour {

	public float speed = 0.5f;


	private bool inSignal = false;
	private bool overJammer = false;
	private Jammer jammer;
	private bool selectIsDown = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("Horizontal") >= 0.001 || Input.GetAxis ("Horizontal") <= -0.001) {
			this.transform.position = new Vector3 (this.transform.position.x + speed * Input.GetAxis ("Horizontal"), this.transform.position.y, this.transform.position.z);
		}
			
		if (Input.GetAxis ("Fire1") >= 0.001) {
			Debug.Log ("KeyPressed");
			if (!selectIsDown && overJammer) {
				selectIsDown = true;
				jammer.FlipSwitch ();
			}
		} else {
			selectIsDown = false;
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
		if (isIn) {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.cyan;
		} else {
			this.gameObject.GetComponent<SpriteRenderer> ().color = Color.white;
		}
	}
}
