using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Threading;

public class GuiArduinoSerialScript : MonoBehaviour{
	// Serial - Belt
	public static string COMPortBelt = "COM3";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not
	// Timer - Print tactons;
	public int enableDraw = 1;
	public int patternKey = 30;
	public System.Timers.Timer timerDrawWave = new System.Timers.Timer(1000);
	public System.Timers.Timer timerDrawWaveInt = new System.Timers.Timer(200);
	public static int contCicle = 0;
	public static int logFlag = 0;
	public static int regf = 0;
	// Textures
	public Texture2D resp1;
	public Texture2D resp2;
	public Texture2D resp3;
	public Texture2D resp4;
	public Texture2D resp5;
	public Texture2D resp6;
	public Texture2D resp7;
	public Texture2D resp8;
	GameObject answerPlane;
	
	public static char[] Tacton = new char[] {'0','0','0','0','0','0','0','0'}; // Tacton
	public static char[] Info = new char[] {'0','0','0','0','0','0'}; // Informacoes por Tacton
	
	void Start(){
		answerPlane = GameObject.Find("Answer");
		answerPlane.renderer.enabled = false;

		OpenConnectionBelt();
		EsconderAvatar();
	}

	void Update(){
		if(Input.GetKey(KeyCode.Escape))
            Application.Quit();
		
		if(portopenBelt && _SerialPortBelt.IsOpen)
			TactonToBelt();
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
	
	public static void ProcessaTacton() {
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7", "Sphere8"};
		GameObject esfera;
		
		for (int i=0; i < 8; i++) {
			esfera = GameObject.Find(motors[i]);
			if(esfera.renderer.material.color == Color.red){
				Tacton[i]='1';
			}else{
				Tacton[i]='0';
			}
		}
	}
	
	public static void ProcessaInfo() {
		string[] opcoes = {"OpAlerta", "OpIntin", "OpObst", "OpNon"};
		GameObject opcao;
		
		for (int i=0; i < 4; i++) {
			opcao = GameObject.Find(opcoes[i]);
			if(opcao.guiText.material.color == Color.green){
				Info[i]='1';
			}else{
				Info[i]='0';
			}
		}
	}
	
	public static void DesativaTodos() {
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7", "Sphere8"};
		Tacton = new char[] {'0','0','0','0','0','0','0','0'};
		for(int i=0;i<8;i++){
			GameObject esfera = GameObject.Find(motors[i]);
			esfera.renderer.material.color = Color.white;
		}
	}
	
	public static void DesmarcaOpcoes() {
		string[] opcoes = {"OpAlerta", "OpIntin", "OpObst", "OpNon"};
		Info = new char[] {'0','0','0','0','0','0'};
		for(int i=0;i<4;i++){
			GameObject opcao = GameObject.Find(opcoes[i]);
			opcao.guiText.material.color = Color.white;
		}
	}
	
	public static string InfoToString() {
		string[] opcoes = {"Alerta", "Intinerario", "Obstaculo", "NaoEntedi"};
		string infoString = "";
		
		for(int i=0;i<4;i++){
			if(Info[i] == '1'){
				infoString += opcoes[i] + ", ";
			}
		}
		
		return infoString;
	}
	
	public static void OpenConnectionBelt(){
		if (_SerialPortBelt != null){
			if (_SerialPortBelt.IsOpen){
				_SerialPortBelt.Close();
				print("Closing port, because it was already open!");
			}
			else{
				//very important!, this opens the connection
				_SerialPortBelt.Open();
				//sets the timeout value (how long it takes before timeout error occurs)
				//zet de timeout niet te hoog, dit vertraagd Unity enorm en kan zelfs voor crashes zorgen.
				//_SerialPortBelt.ReadTimeout = 1000;
				_SerialPortBelt.WriteTimeout = 1000;
				portopenBelt = true;
			}
		}
		else{
			if (_SerialPortBelt.IsOpen){
				print("Port is already open");
			}else{
				print("Port == null");
			}
		}
	}
	
	void EnableDraw(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		enableDraw = 1;
	}
	
	void TactonToBelt(){		
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		char[] SendToArduinoZero = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		GameObject output = GameObject.Find("Output");
		output.guiText.material.color = Color.gray;

		if(regf == 0)
			output.guiText.text = "Padrão #" + (patternKey+1);
		
		switch(patternKey){
			case 0:
				SendToArduino[SendToArduinoPosition[0]]='1';
				answerPlane.renderer.material.mainTexture = resp1;
				break;
			case 1:
				SendToArduino[SendToArduinoPosition[1]]='1';
				SendToArduino[SendToArduinoPosition[5]]='1';
				answerPlane.renderer.material.mainTexture = resp2;
				break;
			case 2:
				SendToArduino[SendToArduinoPosition[0]]='1';
				SendToArduino[SendToArduinoPosition[2]]='1';
				answerPlane.renderer.material.mainTexture = resp3;
				break;
			case 3:
				SendToArduino[SendToArduinoPosition[4]]='1';
				SendToArduino[SendToArduinoPosition[6]]='1';
				answerPlane.renderer.material.mainTexture = resp4;
				break;
			case 4:
				SendToArduino[SendToArduinoPosition[5]]='1';
				SendToArduino[SendToArduinoPosition[7]]='1';
				answerPlane.renderer.material.mainTexture = resp5;
				break;
			case 5:
				SendToArduino[SendToArduinoPosition[1]]='1';
				SendToArduino[SendToArduinoPosition[3]]='1';
				answerPlane.renderer.material.mainTexture = resp6;
				break;
			case 6:
				SendToArduino[SendToArduinoPosition[4]]='1';
				SendToArduino[SendToArduinoPosition[6]]='1';
				answerPlane.renderer.material.mainTexture = resp7;
				break;	
			case 7:
				SendToArduino[SendToArduinoPosition[5]]='1';
				answerPlane.renderer.material.mainTexture = resp8;
				break;
			case 8:
				output.guiText.text = "FIM DO TESTE";
				enableDraw = 0; 
				DesativaTodos();
				DesmarcaOpcoes();
				EsconderAvatar();
				_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
				break;
		}
		
		if(enableDraw == 1){			
			enableDraw = 0;
			contCicle++;
			switch(contCicle){	
				case 1:
					_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
					Debug.Log("Imprimindo novo padrao...");					
					DesativaTodos();
					DesmarcaOpcoes();
					EsconderAvatar();
					Debug.Log("Tacton impresso: " + SendToArduino[SendToArduinoPosition[0]] + SendToArduino[SendToArduinoPosition[1]] + SendToArduino[SendToArduinoPosition[2]] + SendToArduino[SendToArduinoPosition[3]] + SendToArduino[SendToArduinoPosition[4]] + SendToArduino[SendToArduinoPosition[5]] + SendToArduino[SendToArduinoPosition[6]] + SendToArduino[SendToArduinoPosition[7]]);
					break;
				case 3:	
				case 4:	
				case 5:	
				case 6:	
				case 7:	
				case 8:	
				case 9:		
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					break;
				case 10:
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					regf = 1;
					output.guiText.text = "";
					break;
				case 11:
					output.guiText.text = "3";
					break;
				case 12:
					output.guiText.text = "2";
					break;		
				case 13:
					output.guiText.text = "1";
					break;
				case 14:
					ProcessaTacton();
					ProcessaInfo();
					Debug.Log("Tacton percebido: " + Tacton[7] + Tacton[0] + Tacton[1] + Tacton[2] + Tacton[3] + Tacton[4] + Tacton[5] + Tacton[6]);			
					Debug.Log("Info marcada: " + Info[0] + Info[1] + Info[2] + Info[3] + Info[4] + Info[5]);
					Debug.Log("InfoSt marcada: " + InfoToString());

					if(SendToArduino[SendToArduinoPosition[0]] == Tacton[7] && 
					   SendToArduino[SendToArduinoPosition[1]] == Tacton[0] && 
					   SendToArduino[SendToArduinoPosition[2]] == Tacton[1] && 
					   SendToArduino[SendToArduinoPosition[3]] == Tacton[2] && 
					   SendToArduino[SendToArduinoPosition[4]] == Tacton[3] && 
					   SendToArduino[SendToArduinoPosition[5]] == Tacton[4] && 
					   SendToArduino[SendToArduinoPosition[6]] == Tacton[5] && 
					   SendToArduino[SendToArduinoPosition[7]] == Tacton[6]){
						output.guiText.text = "Certo!";
					}else{
						output.guiText.text = "Errado! Resposta:";
						answerPlane.renderer.enabled = true;
					}
					break;
				case 17:
					regf = 0;
					contCicle = 0;
					answerPlane.renderer.enabled = false;
					patternKey++;					
					break;				
			}
			timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
			timerDrawWave.Start();
		}
	}
	
	void OnApplicationQuit() {
		//Debug.Log("Ultimo Tacton: " + Tacton[0] + Tacton[1] + Tacton[2] + Tacton[3] + Tacton[4] + Tacton[5] + Tacton[6] + Tacton[7]);
		
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
		
		_SerialPortBelt.Close();
		portopenBelt = false;
		
		print ("saiu");
	}	
}