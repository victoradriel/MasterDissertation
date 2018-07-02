using UnityEngine;
using System.Collections;

public class Menu1Behaviour : MonoBehaviour {	
	GameObject Option1;
	GameObject Option2;
	public static int Id;

	// Cenário por usuário	// 24
	public static string[,] sceneConfig = { 
		{ "Scene1", "Scene2", "Scene3" },
		{ "Scene1", "Scene3", "Scene2" },
		{ "Scene2", "Scene1", "Scene3" },
		{ "Scene2", "Scene3", "Scene1" },
		{ "Scene3", "Scene1", "Scene2" },
		{ "Scene3", "Scene2", "Scene1" },
		{ "Scene1", "Scene2", "Scene3" },
		{ "Scene1", "Scene3", "Scene2" },
		{ "Scene2", "Scene1", "Scene3" },
		{ "Scene2", "Scene3", "Scene1" },
		{ "Scene3", "Scene1", "Scene2" },
		{ "Scene3", "Scene2", "Scene1" },
		{ "Scene1", "Scene2", "Scene3" },
		{ "Scene1", "Scene3", "Scene2" },
		{ "Scene2", "Scene1", "Scene3" },
		{ "Scene2", "Scene3", "Scene1" },
		{ "Scene3", "Scene1", "Scene2" },
		{ "Scene3", "Scene2", "Scene1" },
		{ "Scene1", "Scene2", "Scene3" },
		{ "Scene1", "Scene3", "Scene2" },
		{ "Scene2", "Scene1", "Scene3" },
		{ "Scene2", "Scene3", "Scene1" },
		{ "Scene3", "Scene1", "Scene2" },
		{ "Scene3", "Scene2", "Scene1" }
	};

	// Use this for initialization
	void Start () {
		Option1 = GameObject.Find("Option1");
		Option2 = GameObject.Find("Option2");
	}
	
	// Update is called once per frame
	void Update () {
		if((Input.GetKey(KeyCode.A))||(Input.GetButton("joystick button 0"))){
			int playAllow = PlayerPrefs.GetInt("Play");
			if(playAllow == 1){
				Option1.guiText.material.color = Color.green;
				Application.LoadLevel("PracticeScene");
			} else{
				GameObject x = GameObject.Find("Id");
				x.guiText.material.color = Color.red;
			}
		}

		if((Input.GetKey(KeyCode.B))||(Input.GetButton("joystick button 1"))){
			int playAllow = PlayerPrefs.GetInt("Play");
			if(playAllow == 1){
				Id = PlayerPrefs.GetInt("Id");
				Option2.guiText.material.color = Color.green;
				Application.LoadLevel(sceneConfig[Id, 0]);
			} else{
				GameObject x = GameObject.Find("Id");
				x.guiText.material.color = Color.red;
			}
		}
		
		if(Input.GetKey(KeyCode.Escape)) Application.Quit();
	}
}
