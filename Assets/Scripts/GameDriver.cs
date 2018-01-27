using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDriver : MonoBehaviour {

	public string[] levels;
	public int currentLevel = 0;

	private bool resetPressed = false;

	// Use this for initialization
	void Start () {
		GameObject.DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (!resetPressed) {
			if (Input.GetAxis ("Reset") > 0) {
				resetPressed = true;
				ReloadLevel ();
					
			}
		} else {
			resetPressed = false;
		}

	}
			
	public void ReloadLevel(){
		SceneManager.LoadScene (levels [currentLevel]);
	}

	public void GoToNextLevel(){
		currentLevel++;
		if (currentLevel >= levels.Length) {
			currentLevel = 0;
		}
		SceneManager.LoadScene (levels [currentLevel]);
	}

}
