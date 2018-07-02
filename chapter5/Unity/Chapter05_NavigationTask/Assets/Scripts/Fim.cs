using UnityEngine;
using System.Collections;

public class Fim : MonoBehaviour {
	public static string user;
	public static int score;

	// Use this for initialization
	void Start () {	
		user = PlayerPrefs.GetString("User");
		score = PlayerPrefs.GetInt("Score");
	}
	
	// Update is called once per frame
	void Update () {
		GameObject userText = GameObject.Find("User");
		GameObject scoreText = GameObject.Find("Pontuacao");
		userText.guiText.text = "Parabéns, " + user + "!";
		scoreText.guiText.text = "Você fez " + score + " pontos.";
	}
}
