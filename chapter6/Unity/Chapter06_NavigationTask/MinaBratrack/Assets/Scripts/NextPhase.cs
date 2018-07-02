using UnityEngine;
using System.Collections;

public class NextPhase : MonoBehaviour {
	public static int[,] userConfig = { 
		{ 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, // 0 - 4 	 << Sem modificadores
		{ 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, // 5 - 9
		{ 5, 1 }, { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, // 10 - 14 << Sem modificadores
		{ 5, 1 }, { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, // 15 - 19
		{ 4, 2 }, { 5, 2 }, { 0, 2 }, { 1, 2 }, { 2, 2 }, // 20 - 24 << Sem modificadores
		{ 4, 2 }, { 5, 2 }, { 0, 2 }, { 1, 2 }, { 2, 2 }, // 25 - 29 
		{ 3, 3 }, { 4, 3 }, { 5, 3 }, { 0, 3 }, { 1, 3 }, // 30 - 34 << Sem modificadores
		{ 3, 3 }, { 4, 3 }, { 5, 3 }, { 0, 3 }, { 1, 3 }, // 35 - 39
		{ 2, 4 }, { 3, 4 }, { 4, 4 }, { 5, 4 }, { 0, 4 }, // 40 - 44 << Sem modificadores
		{ 2, 4 }, { 3, 4 }, { 4, 4 }, { 5, 4 }, { 0, 4 }, // 45 - 49 
		{ 1, 5 }, { 2, 5 }, { 3, 5 }, { 4, 5 }, { 5, 5 }, // 50 - 54 << Sem modificadores
		{ 1, 5 }, { 2, 5 }, { 3, 5 }, { 4, 5 }, { 5, 5 }  // 55 - 59
	};
	
	// Cenário por usuário	
	public static string[,] sceneConfig = { 
		{ "Scene2", "Scene3", "Scene4" },
		{ "Scene2", "Scene4", "Scene3" },
		{ "Scene3", "Scene2", "Scene4" },
		{ "Scene3", "Scene4", "Scene2" },
		{ "Scene4", "Scene2", "Scene3" },
		{ "Scene4", "Scene3", "Scene2" }
	};
	
	// Variação na linguagem por usuário	
	public static string[,] langConfig = { 
		{ "d", "f", "i" },
		{ "d", "i", "f" },
		{ "f", "d", "i" },
		{ "f", "i", "d" },
		{ "i", "d", "f" },
		{ "i", "f", "d" }
	};

	// Use this for initialization
	void Start () {
		PlayerPrefs.SetInt("Play", Application.loadedLevel - 3);
		/*
		//Gravar log
		for(int i = 0; i < 42 ; i++){
			Debug.Log(i + ":" + sceneConfig[userConfig[i, 1], 0] + ":" + langConfig[userConfig[i, 0], 0]+ ":" + sceneConfig[userConfig[i, 1], 1] + ":" + langConfig[userConfig[i, 0], 1]+ ":" + sceneConfig[userConfig[i, 1], 2] + ":" + langConfig[userConfig[i, 0], 2]);
		}*/
		
	}
	
	// Update is called once per frame
	void Update () {
		int userId = PlayerPrefs.GetInt("Id");		
		PlayerPrefs.SetInt(langConfig[userConfig[userId, 0], Application.loadedLevel - 5], 1);
		PlayerPrefs.Save();
		InfUnlocked();
		
		if((Input.GetKey(KeyCode.Space))||(Input.GetButton("joystick button 2"))){
			guiText.material.color = Color.green;
			// In build settings: menuToPhase2 = 4 | menuToPhase3 = 5 | menuToPhase4 = 6
			Application.LoadLevel(sceneConfig[userConfig[userId, 1], Application.loadedLevel - 5]);
		}
		
		if(Input.GetKey(KeyCode.Escape)) Application.Quit();        
	}
	
	void InfUnlocked(){
		GameObject course = GameObject.Find("Course");
		GameObject destination = GameObject.Find("Destination");
		GameObject intinerary = GameObject.Find("Intinerary");
		GameObject warning = GameObject.Find("Warning");
		GameObject obstacle = GameObject.Find("Obstacle");
		
		
		int d = 0;
		int f = 0;
		int i = 0;
		
		d = PlayerPrefs.GetInt("d");
		f = PlayerPrefs.GetInt("f");
		i = PlayerPrefs.GetInt("i");
		
		obstacle.guiText.material.color = Color.green;
		
		if(d == 1)
			destination.guiText.material.color = Color.green;
		else
			destination.guiText.material.color = Color.gray;
		
		if(f == 1){
			course.guiText.material.color = Color.green;
			warning.guiText.material.color = Color.green;
		}else{
			course.guiText.material.color = Color.gray;
			warning.guiText.material.color = Color.gray;
		}
		
		if(i == 1)
			intinerary.guiText.material.color = Color.green;
		else
			intinerary.guiText.material.color = Color.gray;
	}
}
