using UnityEngine;
using System.Collections;

public class Placa : MonoBehaviour {
	
	public static int updateintervalbigger = 50;
	public short incLight = 0;
	GameObject[] lights;

	// Use this for initialization
	void Start () {
		lights = GameObject.FindGameObjectsWithTag("refLight");
	}
	
	// Update is called once per frame
	void Update () {
		
		//// Registra trajetória do usuário ///////////////////////////////////////////////////////////////
		updateintervalbigger++;
		if(updateintervalbigger >= 50){
			updateintervalbigger = 0;
			
			if(incLight == 0){
				foreach (GameObject refLight in lights) {
					refLight.light.intensity+=2;
					if (refLight.light.intensity >= 2) incLight = 1;
				}
			}else{
				foreach (GameObject refLight in lights) {
					refLight.light.intensity-=2;
					if (refLight.light.intensity <= 0) incLight = 0;
				}
			}
			
		}

	}
}
