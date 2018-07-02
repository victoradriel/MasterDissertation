using UnityEngine;
using System.Collections;

public class ColorirTexto : MonoBehaviour {
	public int ultimaPag;
	public static int modifier; 
	
	// Use this for initialization
	void Start () {
		foreach(GameObject text in GameObject.FindGameObjectsWithTag("Text")){
		    text.guiText.material.color = Color.black;
		}
		
		camera.backgroundColor = Color.white;
		ultimaPag = Application.loadedLevel;
		modifier = PlayerPrefs.GetInt("Modifier");
	}
	
	// Update is called once per frame
	void Update () {
			if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.DownArrow)){
				if(ultimaPag != 0)
					Application.LoadLevel(ultimaPag-1);
			}
			
			if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow)){
				if(ultimaPag != 8)
					Application.LoadLevel(ultimaPag+1);
			}
	}
}
