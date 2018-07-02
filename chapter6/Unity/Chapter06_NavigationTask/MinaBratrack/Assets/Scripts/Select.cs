using UnityEngine;
using System.Collections;

public class Select : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnMouseDown() {
		if(this.guiText.material.color == Color.green){
			EnableAll();
			EsconderAvatar();
		}else{
			DesableAll();
			this.guiText.material.color = Color.green;
			
			DesativaTodos();
			
			switch(this.guiText.name){
				case "OpIntin":
					EsconderAvatar();
					MostrarAvatar("Intinerario");
					break;
				case "OpObst":
					EsconderAvatar();
					MostrarAvatar("Posicao");
					break;
				case "OpAlerta":
					EsconderAvatar();
					MostrarAvatar("Alerta");
					break;
				case "OpNon":
					EsconderAvatar();
					MostrarAvatar("Default");
					break;
			}
		}
    }
	
	public static void DesableAll(){
		string[] opcoes = {"OpAlerta", "OpIntin", "OpObst", "OpNon"};
		GameObject opcao;
		
		for (int i=0; i < 4; i++) {
			opcao = GameObject.Find(opcoes[i]);
			opcao.guiText.material.color = Color.gray;
		}
	} 
	
	public static void EnableAll(){
		string[] opcoes = {"OpAlerta", "OpIntin", "OpObst", "OpNon"};
		GameObject opcao;
		
		for (int i=0; i < 4; i++) {
			opcao = GameObject.Find(opcoes[i]);
			opcao.guiText.material.color = Color.white;
		}
	}
	
	public static void EsconderAvatar(){
		GameObject plane = GameObject.Find("Plane");
		GameObject setas = GameObject.Find("Setas");
		GameObject rosaVentos = GameObject.Find("RosaVentos");
		GameObject course = GameObject.Find("Fluxo");
		GameObject personagem = GameObject.Find("Personagem");
		GameObject title = GameObject.Find("Posicao");
		
		EsconderMotores();
		plane.renderer.enabled = false;
		personagem.renderer.enabled = false;
		title.guiText.enabled = false;
		setas.renderer.enabled = false;
		course.renderer.enabled = false;
		rosaVentos.renderer.enabled = false;
	}
	
	public static void EsconderMotores(){
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7", "Sphere8"};
		string[] lines = {"Cylinder1", "Cylinder2", "Cylinder3", "Cylinder4", "Cylinder5", "Cylinder6", "Cylinder7", "Cylinder8"};
		
		for(int i=0;i<8;i++){
			GameObject esfera = GameObject.Find(motors[i]);
			esfera.renderer.enabled = false;
			GameObject line = GameObject.Find(lines[i]);
			line.renderer.enabled = false;
		}
	}
	
	public static void DesativaTodos() {
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7", "Sphere8"};
		for(int i=0;i<8;i++){
			GameObject esfera = GameObject.Find(motors[i]);
			esfera.renderer.material.color = Color.white;
		}
	}
	
	public static void MostrarAvatar(string opcao){
		GameObject plane = GameObject.Find("Plane");
		GameObject personagem = GameObject.Find("Personagem");
		GameObject title = GameObject.Find("Posicao");
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7", "Sphere8"};
		string[] lines = {"Cylinder1", "Cylinder2", "Cylinder3", "Cylinder4", "Cylinder5", "Cylinder6", "Cylinder7", "Cylinder8"};
		GameObject setas = GameObject.Find("Setas");
		GameObject rose = GameObject.Find("RosaVentos");
		GameObject course = GameObject.Find("Fluxo");
		
		GameObject esfera;
		GameObject line;
		
		plane.renderer.enabled = true;
		personagem.renderer.enabled = true;
		title.guiText.enabled = true;
		
		switch(opcao){
			case "Intinerario":
				for(int i=0;i<8;i+=2){
					esfera = GameObject.Find(motors[i]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[i]);
					line.renderer.enabled = true;
				}	
				esfera = GameObject.Find(motors[4]);
				esfera.renderer.material.color = Color.red;
				setas.renderer.enabled = true;
				title.guiText.text = "Marque a direção";
				break;
			
			case "Alerta":
				esfera = GameObject.Find(motors[3]);
				esfera.renderer.material.color = Color.red;
				esfera.renderer.enabled = true;
				esfera = GameObject.Find(motors[5]);
				esfera.renderer.material.color = Color.red;
				esfera.renderer.enabled = true;
				line = GameObject.Find(lines[3]);
				line.renderer.enabled = true;
				line = GameObject.Find(lines[5]);
				line.renderer.enabled = true;
								
				title.guiText.text = "";
				break;
			
			case "Posicao":
				for(int i=0;i<8;i++){
					if(i<3 || i>5){
						esfera = GameObject.Find(motors[i]);
						esfera.renderer.enabled = true;
						line = GameObject.Find(lines[i]);
						line.renderer.enabled = true;
					}
				}				
				rose.renderer.enabled = true;
				title.guiText.text = "Marque a posição";
				break;

			case "Default":
				for(int i=0;i<8;i++){
					esfera = GameObject.Find(motors[i]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[i]);
					line.renderer.enabled = true;
				}				
				rose.renderer.enabled = true;
				title.guiText.text = "Marque a posição";
				break;
		}
	}
}
