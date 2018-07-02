using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Threading;

public class Select : MonoBehaviour {
	// Serial - Belt
	public static string COMPortBelt = "COM4";
	public static SerialPort _SerialPortBelt = new SerialPort(COMPortBelt, 57600); //COM port and baudrate
	public static bool portopenBelt = false; //if port is open or not
	// Timer - Print tactons;
	public int patternKey = 0;
	public int enableDraw = 1;
	public int enableDrawInt = 1;
	public int enableDrawModifier = 1;
	public System.Timers.Timer timerDrawWaveInt = new System.Timers.Timer(200);
	public System.Timers.Timer timerDrawWave = new System.Timers.Timer(1000);
	public System.Timers.Timer timerDrawWaveModifier = new System.Timers.Timer(500);
	public static int contCicle = 0;
	// Flags
	public static int logFlag = 0;
	public static int ativa = 0;
	public static int obstF = 0;
	public static int destF = 0;
	public static int fluxF = 0;
	public static int alerF = 0;
	public static int intnF = 0;
	// Intinerary patterns
	public int contWave = 0;
	public int toRight = 7;
	public int toLeft = 3;
	// Textures
	public Texture2D obstFrame1;
	public Texture2D obstFrame2;
	public Texture2D obstFrame3;
	GameObject planoAnim;
	
	public static int modifier; 
	public static char[] Info = new char[] {'0','0','0','0','0','0'}; // Informacoes por Tacton
	
	void Start(){
		if (! _SerialPortBelt.IsOpen)
			OpenConnectionBelt();	
		
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);

		planoAnim = GameObject.Find("Plane");
		modifier = PlayerPrefs.GetInt("Modifier");
		
		ativa = 0;
		obstF = 0;
		destF = 0;
		fluxF = 0;
		alerF = 0;
		intnF = 0;
	}

	void Update(){
		if(Input.GetKey(KeyCode.Escape))
            Application.Quit();
		
		if(ativa == 1){
			this.guiText.material.color = Color.gray;
			this.guiText.text = "Aguarde a simulação...";
		}else{
			this.guiText.material.color = Color.black;
			this.guiText.text = "Ativar simulação";
		}
		
				
		if(portopenBelt && _SerialPortBelt.IsOpen && ativa > 0)
			TactonToBelt();			
		
		if(Input.GetKey(KeyCode.End)){
			switch(this.guiText.name){
				case "AtivarInt":
					intnF = 1;
					ativa = 1;
					break;
				case "AtivarObst":
					EsconderMotores();
					MostrarAvatar("Obstaculo");
					ativa = 1;
					break;
				case "AtivarAlerta":
					alerF = 1;
					ativa = 1;
					break;
			}
		}
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
	
	public static void AtivaMotores(char[] Tacton) {
		string[] motors = {"Sphere8", "Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7"};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		for(int i=0;i<8;i++){
			if(Tacton[SendToArduinoPosition[i]] == '1' || Tacton[SendToArduinoPosition[i]] == '2'){
				GameObject esfera = GameObject.Find(motors[i]);
				esfera.renderer.material.color = Color.red;
			}
		}
	}
	
	public static void OpenConnectionBelt(){
		if (_SerialPortBelt != null){
			if (_SerialPortBelt.IsOpen){
				_SerialPortBelt.Close();
				print("Closing port, because it was already open!");
			}else{			
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
		GameObject comando = GameObject.Find("Comando");
		comando.guiText.material.color = Color.black;
		
		if(obstF == 1){
			switch(patternKey){
				case 0:	
					SendToArduino[SendToArduinoPosition[2]]='1';
					break;
				case 1:
					SendToArduino[SendToArduinoPosition[0]]='1';
					SendToArduino[SendToArduinoPosition[2]]='1';
					SendToArduino[SendToArduinoPosition[3]]='1';
					break;
				case 2:
					SendToArduino[SendToArduinoPosition[7]]='1';
					SendToArduino[SendToArduinoPosition[1]]='1';
					SendToArduino[SendToArduinoPosition[3]]='1';
					break;
				case 3:
					enableDraw = 0; 
					ativa = 0;
					patternKey = 0;
					this.guiText.material.color = Color.black;
					DesativaTodos();
					_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
					break;
			}
		}
		else if(alerF == 1){
			switch(patternKey){
				case 0:	
					comando.guiText.text = "Caminho errado!";
					SendToArduino[SendToArduinoPosition[4]]='1';
					SendToArduino[SendToArduinoPosition[6]]='1';
					break;
				case 1:
					comando.guiText.text = "";
					enableDraw = 0; 
					ativa = 0;
					patternKey = 0;
					this.guiText.material.color = Color.black;
					DesativaTodos();
					_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
					break;
			}
		}
		else if(intnF == 1){
			//timerDrawWave = new System.Timers.Timer(900);
			switch(patternKey){
			case 0:	
				comando.guiText.text = "Seguir em frente";
				SendToArduino[SendToArduinoPosition[5]]='1';	
				SendToArduino[SendToArduinoPosition[1]]='1';
				break;
				///////////////////////////////////////////
			case 1:	
				comando.guiText.text = "Virar à direita";
				SendToArduino[SendToArduinoPosition[5]]='1';	
				SendToArduino[SendToArduinoPosition[3]]='1';
				break;
				///////////////////////////////////////////
			case 2:	
				comando.guiText.text = "Seguir para trás";
				SendToArduino[SendToArduinoPosition[5]]='1';
				break;
				///////////////////////////////////////////
			case 3:	
				comando.guiText.text = "Virar à esquerda";
				SendToArduino[SendToArduinoPosition[5]]='1';
				SendToArduino[SendToArduinoPosition[7]]='1';
				break;
			case 4:
				enableDraw = 0; 
				ativa = 0;
				patternKey = 0;
				this.guiText.material.color = Color.black;
				DesativaTodos();					
				_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);
				comando.guiText.text = "";
				break;
			}
		}
	
		if(enableDraw == 1){			
			enableDraw = 0;
			contCicle++;
			
			switch(contCicle){	
				case 1:
					_SerialPortBelt.Write(SendToArduinoZero, 0, SendToArduinoZero.Length);				
					DesativaTodos();
					break;
				case 2:	
					if(obstF == 1 && patternKey == 0){
						planoAnim.renderer.material.mainTexture = obstFrame1;
					}else if(obstF == 1 && patternKey == 1){
						planoAnim.renderer.material.mainTexture = obstFrame2;
					}else if(obstF == 1 && patternKey == 2){
						planoAnim.renderer.material.mainTexture = obstFrame3;
					}
							
					AtivaMotores(SendToArduino);
					_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
					break;			
				case 3:
					contCicle = 0;
					patternKey++;
					break;				
			}

			timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
			timerDrawWave.Start();

		}
	}
	
	void OnMouseDown() {
		if(modifier == 0){
			switch(this.guiText.name){
				case "AtivarFlux":
					EsconderMotores();
					MostrarAvatar("Fluxo");
					ativa = 1;
					break;
				case "AtivarInt":
					EsconderMotores();
					MostrarAvatar("Intinerario");
					ativa = 1;
					break;
				case "AtivarDest":
					destF = 1;
					ativa = 1;
					break;
				case "AtivarObst":
					EsconderMotores();
					MostrarAvatar("Obstaculo");
					ativa = 1;
					break;
				case "AtivarAlerta":
					EsconderMotores();
					MostrarAvatar("Alerta");
					ativa = 1;
					break;
			}
		}else{
			switch(this.guiText.name){
				case "AtivarFlux":
					fluxF = 1;
					ativa = 1;
					break;
				case "AtivarInt":
					intnF = 1;
					ativa = 1;
					break;
				case "AtivarDest":
					destF = 1;
					ativa = 1;
					break;
				case "AtivarObst":
					EsconderMotores();
					MostrarAvatar("Obstaculo");
					ativa = 1;
					break;
				case "AtivarAlerta":
					alerF = 1;
					ativa = 1;
					break;
			}
		}
		
    }
	
	public static void MostrarAvatar(string opcao){
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere4", "Sphere5", "Sphere6", "Sphere7", "Sphere8"};
		string[] lines = {"Cylinder1", "Cylinder2", "Cylinder3", "Cylinder4", "Cylinder5", "Cylinder6", "Cylinder7", "Cylinder8"};
		
		GameObject esfera;
		GameObject line;
		
		switch(opcao){
			case "Intinerario":
				for(int i=0;i<8;i++){
					esfera = GameObject.Find(motors[i]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[i]);
					line.renderer.enabled = true;
				}
				intnF = 1;
				break;
			
			case "Alerta":
				esfera = GameObject.Find(motors[4]);
				esfera.renderer.enabled = true;
				line = GameObject.Find(lines[4]);
				line.renderer.enabled = true;
			
				alerF = 1;
				break;
			
			case "Obstaculo":
				for(int i=0;i<3;i++){
					esfera = GameObject.Find(motors[i]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[i]);
					line.renderer.enabled = true;
				}
					esfera = GameObject.Find(motors[6]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[6]);
					line.renderer.enabled = true;
			
					esfera = GameObject.Find(motors[7]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[7]);
					line.renderer.enabled = true;
				obstF = 1;
				break;
			
			case "Fluxo":
				for(int i=3;i<6;i+=2){
					esfera = GameObject.Find(motors[i]);
					esfera.renderer.enabled = true;
					line = GameObject.Find(lines[i]);
					line.renderer.enabled = true;
				}	
				fluxF = 1;
				break;
		}
	}

	void OnApplicationQuit() {
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		_SerialPortBelt.Write(SendToArduino, 0, SendToArduino.Length);
		
		_SerialPortBelt.Close();
		portopenBelt = false;
		
		print ("saiu");
	}
}
