using UnityEngine;
using System.Collections;

public class Menu1Behaviour : MonoBehaviour {	
	// Update is called once per frame
	void Update () {
		if((Input.GetKey(KeyCode.Space))||(Input.GetButton("joystick button 2"))){
			int playAllow = PlayerPrefs.GetInt("Play");
			if(playAllow == 1){
				guiText.material.color = Color.green;
				Application.LoadLevel("Scene1");
			} else{
				GameObject x = GameObject.Find("Id");
				x.guiText.material.color = Color.red;
			}
		}
		
		if(Input.GetKey(KeyCode.Escape)) Application.Quit();
	}
}
