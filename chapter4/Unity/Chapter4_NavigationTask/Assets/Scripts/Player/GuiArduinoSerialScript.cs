using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class GuiArduinoSerialScript : MonoBehaviour{
	public static string COMPort = "COM3";	
	public static bool portopen = false; //if port is open or not
	public static SerialPort _SerialPort = new SerialPort(COMPort, 57600); //COM port and baudrate
	public static int updateslower = 0;
	public static int updateinterval = 4; //50 seems interesting
	public static int updateintervalbigger = 50;
	float rotatationSpeed = 200.0F;	
	public float speed = 6.0F;
	public float jumpSpeed = 1.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	public string initDateTime;
	public string endDateTime;
	public int qtdColisoes = 0;
	public string trajeto = "";
	public int end = 0;
	public int start = 0;
		
	void Awake(){
		//cc : CharacterController = GetComponent(CharacterController);
	}
	
	void Start (){
		//start the serial connection.
		OpenConnection();
		
	}// Start
	

	void OnControllerColliderHit(ControllerColliderHit obj){
		if(obj.gameObject.name == "barril"){
			qtdColisoes++;
		}	
	}
	
	void OnTriggerEnter (Collider myTrigger) {
		if(myTrigger.gameObject.name == "chegada"){
			endDateTime = DateTime.Now.ToString("hh:mm:ss");
			end = 1;
		}
	}

	void Update (){
		
		//// Fechar aplicação /////////////////////////////////////////////////////////////////////////////
		if(Input.GetKey(KeyCode.Escape)){
            Application.Quit();
        }
		
		//// Movimento ////////////////////////////////////////////////////////////////////////////////////
		CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
			
			var newRotation = Input.GetAxis("Horizontal") * rotatationSpeed;
			transform.Rotate(0, newRotation * Time.deltaTime, 0);
			
			if(Input.GetKeyUp(KeyCode.RightControl))
                moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
		
		updateslower++;
		updateintervalbigger++;
		if(portopen && _SerialPort.IsOpen && updateslower>=updateinterval){
			askforinput();
			updateslower = 0;
		}	
		if(updateintervalbigger >= 50){
			updateintervalbigger = 0;
			trajeto = trajeto + controller.transform.position.ToString() + " - ";
			if(start == 0){
				start = 1;
				initDateTime = DateTime.Now.ToString("hh:mm:ss");
			}
		}
		
	}// Update	
	
	//Start the connection!
	public static void OpenConnection(){
		if (_SerialPort != null){
			if (_SerialPort.IsOpen){
				_SerialPort.Close();
				print("Closing port, because it was already open!");
			}
			else{
				//very important!, this opens the connection
				_SerialPort.Open();
				//sets the timeout value (how long it takes before timeout error occurs)
				//zet de timeout niet te hoog, dit vertraagd Unity enorm en kan zelfs voor crashes zorgen.
				_SerialPort.ReadTimeout = 100;
				portopen = true;
			}
		}
		else{
			if (_SerialPort.IsOpen){
				print("Port is already open");
			}else{
				print("Port == null");
			}
		}
	}// OpenConnection
	
	//Polling the arduino
	void askforinput(){	
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		Vector3 positionAbove = new Vector3();
		Vector3[] vectors = new Vector3[24];
		Vector3[] direction = new Vector3[24];
		RaycastHit hit1, hit2;
		float Dx, Dz, Mxqd, px, pz, scannAbov;
		float[] dist = {2.82F, 2.40F, 2.10F, 2.0F};
		string[] logMessages = {"10:30h", "12:00h", "1:30h", "3:00h", "4:30h", "6:00h", "7:30h", "9:00h"};
		int[] SendToArduinoPosition = {0, 2, 4, 10, 16, 14, 12, 6};
		int rep;
		int j = 0;
		int id = 0;
		int inc = 1;
		
		//if(Input.GetKey(KeyCode.Keypad0)){
		if(Input.GetKey(KeyCode.End)){	
			// Orientação /////////////////////////////////////////////////////////////////////////////////////
			if((transform.localEulerAngles.y >= 292.5)&&(transform.localEulerAngles.y <337.5)) SendToArduino[4]='1';
			else if((transform.localEulerAngles.y >= 337.5)||(transform.localEulerAngles.y <22.5)) SendToArduino[2]='1';
			else if((transform.localEulerAngles.y >= 22.5)&&(transform.localEulerAngles.y <67.5)) SendToArduino[0]='1';
			else if((transform.localEulerAngles.y >= 67.5)&&(transform.localEulerAngles.y <112.5)) SendToArduino[6]='1';
			else if((transform.localEulerAngles.y >= 112.5)&&(transform.localEulerAngles.y <157.5)) SendToArduino[12]='1';
			else if((transform.localEulerAngles.y >= 157.5)&&(transform.localEulerAngles.y <202.5)) SendToArduino[14]='1';
			else if((transform.localEulerAngles.y >= 202.5)&&(transform.localEulerAngles.y <247.5)) SendToArduino[16]='1';
			else if((transform.localEulerAngles.y >= 247.5)&&(transform.localEulerAngles.y <292.5)) SendToArduino[10]='1';
			SendToArduino[8]='1';
		}else{		
			
			// Detecção de obstáculos /////////////////////////////////////////////////////////////////////////
			
			// Vetores, direções e raycasting para detecção de obstáculos altos e baixos
			Mxqd = 2;
			Dx = Mxqd/3;
			Dz = 0;
			px = -Mxqd;
			pz = Mxqd;
			scannAbov = 0.9F; 
			
			// povoa array com vetores
			vectors[0] = new Vector3(px,0,pz);
			for(int i = 1; i < 24; i++){
				px += Dx;
				pz += Dz;
				
				if(px>=Mxqd){ px=Mxqd; }
				else if(px<=Mxqd*(-1)){ px=Mxqd*(-1); }
				if(pz>=Mxqd){ pz=Mxqd; }
				else if(pz<=Mxqd*(-1)){ pz=Mxqd*(-1); }
				
				vectors[i] = new Vector3(px,0,pz);
				
				if(px==Mxqd){
					if(pz==Mxqd){ Dx = 0; Dz = (Mxqd/3)*(-1); }
					else if(pz==Mxqd*(-1)){ Dz = 0; Dx = (Mxqd/3)*(-1); }
				}
				else if(px==Mxqd*(-1)){
					if(pz==Mxqd*(-1)){ Dx = 0; Dz = Mxqd/3; }
				}
			}
			// povoa array com direções
			for(int i = 0; i < 24; i++){
				direction[i] = transform.TransformDirection(vectors[i]);
			}
			
			rep = 2; //flag
			positionAbove = transform.position;
			positionAbove.y = scannAbov;
			
			// Código para colorir raios (...)
						
			for(int i = 0; i < 8; i++){
				while(rep>0){
					if(Physics.Raycast(transform.position, direction[j], out hit1, dist[id])){ 
						if(Physics.Raycast(positionAbove, direction[j], out hit2, dist[id])){ 
							//Debug.Log("Obstaculo as " + logMessages[i]);
						}
						else{
							//Debug.Log("Obstaculo BAIXO as " + logMessages[i]);
						}
						SendToArduino[SendToArduinoPosition[i]]='1';
					}
					if(id==0){ inc=1; }
					else if(id==3){inc= -1;}
					id+=inc;
					rep--;
					j++;
				}
				rep=3;
			}
			if(Physics.Raycast(transform.position, direction[23], out hit1, 2.40F)){ 
				if(Physics.Raycast(positionAbove, direction[23], out hit2, 2.40F)){ 
					//Debug.Log("Obstaculo as " + logMessages[0]);
				}
				else{
					//Debug.Log("Obstaculo baixo as " + logMessages[0]);
				}
				SendToArduino[SendToArduinoPosition[0]]='1';
			}
		}
		//Debug.Log(SendToArduino[0]+" "+SendToArduino[1]+" "+SendToArduino[2]+" "+SendToArduino[3]+" "+SendToArduino[4]+" "+SendToArduino[5]+" "+SendToArduino[6]+" "+SendToArduino[7]+" "+SendToArduino[8]+" "+SendToArduino[9]+" "+SendToArduino[10]+" "+SendToArduino[11]+" "+SendToArduino[12]+" "+SendToArduino[13]+" "+SendToArduino[14]+" "+SendToArduino[15]+" "+SendToArduino[16]);
		//SendToArduino= new char[] {'1', ',', '1', ',', '1', ',', '1', ',', '1', ',', '1', ',', '1', ',', '1', ',', '1'};
		if(end == 1){
			SendToArduino= new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '1', ',', '0', ',', '0', ',', '0', ',', '0'};
		}
		_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
	}
	
	//make sure the connection to the arduino is closed.
	void OnApplicationQuit() {
		if(end == 0){
			endDateTime = DateTime.Now.ToString("hh:mm:ss");			
		}
		Debug.Log("Tempo inicial: " + initDateTime);
		Debug.Log("Tempo final: " + endDateTime);
		Debug.Log("Colisoes em barris: " + qtdColisoes);
		Debug.Log("Trajetória:" + trajeto);
		
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
		
		_SerialPort.Close();
		portopen = false;
	}
}