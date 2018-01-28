using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDriver : MonoBehaviour {

	private static GameDriver _gameDriver;

	public string[] levels;
	public int currentLevel = 0;

	private static bool resetPressed = false;
	private static float buttonBuffer = 0.5f;
	private static float bufferTimer = 0f;

	public Vector3 cameraPos = Vector3.zero;
	public float cameraSize = 0f;

	void Awake(){
		if (!_gameDriver) {
			_gameDriver = this;
		} else {
			GameObject.Destroy (this.gameObject);
		}
		if (currentLevel > 0) {
			Debug.Log ("Yes");
			Camera.main.transform.position = cameraPos;
			Camera.main.orthographicSize = cameraSize;
		}
	}

	// Use this for initialization
	void Start () {
		GameObject.DontDestroyOnLoad (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("Reset") > 0) {
			if (!resetPressed) {
				resetPressed = true;
				bufferTimer = buttonBuffer;
				ReloadLevel ();
			}
		} else {
			bufferTimer -= Time.deltaTime;
			if (bufferTimer <= 0) {
				resetPressed = false;
			}
		}

	}
			
	public void ReloadLevel(){
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	public void GoToNextLevel(){
		currentLevel++;
		if (currentLevel >= levels.Length) {
			currentLevel = 0;
		}
		SceneManager.LoadScene (levels [currentLevel]);
	}

}
