using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Timers;

public class GuiArduinoSerialScript : MonoBehaviour{
	
	public static int display = 2;	// 1-Hand | 2-Belt
	public GameObject target;
	public GameObject player;
	// Serial
	public static string COMPort = "COM4";
	public static SerialPort _SerialPort = new SerialPort(COMPort, 57600); //COM port and baudrate
	public static bool portopen = false; //if port is open or not
	// Update control		
	public static int updateslower = 0;
	public static int updateinterval = 4; //50 seems interesting
	public static int updateintervalbigger = 50;
	// Moviment
	float rotatationSpeed = 80.0F;
	public float speed = 80.0F;
	public float jumpSpeed = 1.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	public int idleControl = 0;
	// Logs
	public string initDateTime;
	public string endDateTime;
	public int qtdColisoes = 0;
	public string trajeto = "";
	public int end = 0;
	public int start = 0;	
	// Sounds
	public int enableSound = 1;
	public System.Timers.Timer timerMoveSprite = new System.Timers.Timer(1000);
	public AudioClip coinSound;
	// Wave patterns
	public string[] waveDir = {"", "", "", ""};
	public int toRight = 7;
	public int toLeft = 3;
	public int enableDraw = 1;
	public System.Timers.Timer timerDrawWave = new System.Timers.Timer(300);
	// Game/User info
	public static String User = "";
	public static int Id;
	public static int score;
	public int currentPhase = 0;
	public static int mod = 3;
	public static int langObst = 1;
	public static int langDest = 0;
	public static int langCour = 0;
	public static int langIntn = 0;
	// Course
	public int onAlerta = 0;
	public int onFluxo = 0;
	public string courseName = "";
	// Modifier patterns
	public int triggerDest = 0;
	public int ctrlDest = 0;
	public int ctrlIntn = 0;
	public int ctrlCour = 0;
	public int ctrlWarn = 0;
	public int[] modDir = {0, 0, 0, 0};
	public int enablePatMod = 1;
	public System.Timers.Timer timerPattMod = new System.Timers.Timer(900);
		
	void Awake(){
		//DontDestroyOnLoad(this);
	}
	
	void Start (){
		currentPhase = PlayerPrefs.GetInt("Play");
		if(currentPhase == 1) score = 100;
		else score = PlayerPrefs.GetInt("Score");
		GameObject dwnscore = GameObject.Find("Score");
		dwnscore.guiText.text = "Score: " + score;
		
		mod = PlayerPrefs.GetInt("Modifier");
		langDest = PlayerPrefs.GetInt("d");
		langCour = PlayerPrefs.GetInt("f");
		langIntn = PlayerPrefs.GetInt("i");
		
		HidePercurso();
		OpenConnection();
	}
	
	void MoveSprite(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		enableSound = 1;
	}

	void OnControllerColliderHit(ControllerColliderHit obj){
		if((obj.gameObject.name == "barril")||(obj.gameObject.name == "barrel01_prefab")){
			qtdColisoes++; // como versão antiga			
			
			if(enableSound == 1){
				obj.gameObject.audio.Play();
				enableSound = 0;
				timerMoveSprite.Elapsed += new ElapsedEventHandler(MoveSprite);
				timerMoveSprite.Start();
				if(end == 0) score--;
				GameObject dwnscore = GameObject.Find("Score");
				dwnscore.guiText.text = "Score: " + score;
			}
		}
		
		if(obj.gameObject.name == "Coin"){
			player = GameObject.Find("Personagem");			
			if(end == 0) score+=2;
			GameObject dwnscore = GameObject.Find("Score");
			dwnscore.guiText.text = "Score: " + score;			
			player.gameObject.audio.Play();	
			Destroy(obj.gameObject);
		}
		
		switch(obj.gameObject.name){
			case "Alert":
				onAlerta = 1;
				break;
			case "Alert1":
				onAlerta = 2;
				break;
			case "Alert2":
				onAlerta = 3;
				break;
			case "Alert3":
				onAlerta = 4;
				break;
			case "Alert4":
				onAlerta = 5;
				break;
			default:
				onAlerta = 0;
				break;
		}
		
		if(obj.gameObject.name == "CourseH" || obj.gameObject.name == "CourseV"){
			onFluxo = 1;
			courseName = obj.gameObject.name;
		}else{
			onFluxo = 0;
			courseName = "";
		}
	}
	
	void OnTriggerEnter (Collider myTrigger) {
		GameObject chegadaMsg = GameObject.Find("chegadaMsg");
		// Mudar mensagem de termino da fase a depender da ordem
		switch(myTrigger.gameObject.name){			
			case "chegada1":				
			case "chegada2":
			case "chegada3":
			case "chegada4":
				chegadaMsg.guiText.text = "End of Phase "+ currentPhase +"\nScore: " + score + "\n\nPress any key to continue";
				endDateTime = DateTime.Now.ToString("hh:mm:ss");
				end = 1;
				GravaLog();
				break;	
		}
	}
	
	public static void HidePercurso(){
		// Esconde marcações de alerta
		foreach(GameObject alertas in GameObject.FindGameObjectsWithTag("Alert")){
		    alertas.renderer.enabled = false;
		}
		// Esconde marcações de fluxo
		foreach(GameObject fluxo in GameObject.FindGameObjectsWithTag("Course")){
		    fluxo.renderer.enabled = false;
		}
	}

	void Update (){
		if(Input.GetKey(KeyCode.Escape))
            Application.Quit();
		
		if((end == 1)&&(Input.anyKey))
			CarregarMenu(currentPhase);
		
		//// Movimento ////////////////////////////////////////////////////////////////////////////////////
		CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded) {
            moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;

			var newRotation = Input.GetAxis("Horizontal") * rotatationSpeed;
			transform.Rotate(0, newRotation * Time.deltaTime, 0);
        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
		
		if(Input.GetAxis("Vertical") == 0)
			idleControl++;
		else idleControl = 0;
		
		if(idleControl == 150) triggerDest = 1;		
		
		//// Captura informação do ambiente e envia ao display tátil //////////////////////////////////////
		updateslower++;		
		if(portopen && _SerialPort.IsOpen && updateslower>=updateinterval){
			updateslower = 0;
			
			switch(mod){
				case 0: // Sem modificadores
					if(onAlerta > 1 && langIntn == 1)
						AskForIntinerary(onAlerta);
					else 
						AskForInput();
					break;
				
				case 1:	// Com modificadores
					if(triggerDest == 1 && langDest == 1){
						AskForDestinationMdfr();
					}else if(onAlerta == 1 && langCour == 1)
						AskForWarningMdfr();
					else if(onAlerta > 1 && langIntn == 1)
						AskForIntineraryMdfr(onAlerta);
					else if(onFluxo == 1 && langCour == 1)
						AskForCourseMdfr();
					else
						AskForObstacleMdfr();					
					break;
				
				default:
					Debug.Log("ERRO: Variavel para modificador assumiu valor fora do range.");
					break;				
			}
		}	
		
		//// Registra trajetória do usuário ///////////////////////////////////////////////////////////////
		updateintervalbigger++;
		if(updateintervalbigger >= 50){
			updateintervalbigger = 0;
			trajeto = trajeto + controller.transform.position.ToString() + " - ";
			if(start == 0){
				start = 1;
				initDateTime = DateTime.Now.ToString("hh:mm:ss");
			}
		}		
	}	
	
	public static void OpenConnection(){
		if (_SerialPort != null){
			if (_SerialPort.IsOpen){
				_SerialPort.Close();
				print("Closing port, because it was already open!");
			} else{
				_SerialPort.Open();
				_SerialPort.ReadTimeout = 100;
				portopen = true;
			}
		}
		else{
			if (_SerialPort.IsOpen){
				print("Port is already open");
			} else{
				print("Port == null");
			}
		}
	}
	
	void AskForIntinerary(int onAlerta){
		
		switch(onAlerta){
			case 2:
				waveDir = new string[]{"Forward", "Left", "Backward", "Right"};
				break;
			case 3:
				waveDir = new string[]{"Right", "Forward", "Left", "Backward"};
				break;
			case 4:
				waveDir = new string[]{"Backward", "Right", "Forward", "Left"};
				break;
			case 5:
				waveDir = new string[]{"Left", "Backward", "Right", "Forward"};
				break;
		}
		
		if(transform.localEulerAngles.y > 315 || transform.localEulerAngles.y < 45){
			DrawWave(waveDir[0]);
		}else if(transform.localEulerAngles.y > 45 && transform.localEulerAngles.y < 135){
			DrawWave(waveDir[1]);
		}else if(transform.localEulerAngles.y > 135 && transform.localEulerAngles.y < 225){
			DrawWave(waveDir[2]);
		}else if(transform.localEulerAngles.y > 225 && transform.localEulerAngles.y < 315){
			DrawWave(waveDir[3]);
		}
	}
	
	void AskForInput(){	
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		Vector3 positionAbove = new Vector3();
		Vector3[] vectors = new Vector3[24];
		Vector3[] direction = new Vector3[24];
		RaycastHit hit1;
		float Dx, Dz, Mxqd, px, pz, scannAbov;
		float[] dist = {2.82F, 2.40F, 2.10F, 2.0F};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		int rep;
		int j = 0;
		int id = 0;
		int inc = 1;

		// Detecção de obstáculos /////////////////////////////////////////////////////////////////////////
		if(langObst == 1){			
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
			for(int i = 0; i < 24; i++){ direction[i] = transform.TransformDirection(vectors[i]); }		
			rep = 2; //flag
			positionAbove = transform.position;
			positionAbove.y = scannAbov;
						
			for(int i = 0; i < 8; i++){
				while(rep>0){
					if(Physics.Raycast(transform.position, direction[j], out hit1, dist[id])){ 
						if((j < 10)||(j > 20))
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
			if(Physics.Raycast(transform.position, direction[23], out hit1, 2.40F)) 
				SendToArduino[SendToArduinoPosition[0]]='1';
		}
		
		if(langCour == 1){
			// Detecção de alerta /////////////////////////////////////////////////////////////////////////////
			if(onAlerta == 1)
				SendToArduino[SendToArduinoPosition[5]]='1';
			
			
			// Detecção de fluxo //////////////////////////////////////////////////////////////////////////////
			if(onFluxo == 1){
				if(courseName == "CourseH"){
					if((transform.localEulerAngles.y > 280 && transform.localEulerAngles.y < 360) || (transform.localEulerAngles.y > 100 && transform.localEulerAngles.y < 180)){
						SendToArduino[SendToArduinoPosition[6]]='1';
					} else if((transform.localEulerAngles.y > 0 && transform.localEulerAngles.y < 80) || (transform.localEulerAngles.y > 180 && transform.localEulerAngles.y < 260)){
						SendToArduino[SendToArduinoPosition[4]]='1';
					} else{
						SendToArduino[SendToArduinoPosition[4]]='1';
						SendToArduino[SendToArduinoPosition[6]]='1';
					}
				}
				else{ // CourseV
					if((transform.localEulerAngles.y > 10 && transform.localEulerAngles.y < 90) || (transform.localEulerAngles.y > 190 && transform.localEulerAngles.y < 270)){
						SendToArduino[SendToArduinoPosition[6]]='1';
					} else if((transform.localEulerAngles.y > 270 && transform.localEulerAngles.y < 350) || (transform.localEulerAngles.y > 90 && transform.localEulerAngles.y < 170)){
						SendToArduino[SendToArduinoPosition[4]]='1';
					} else{
						SendToArduino[SendToArduinoPosition[4]]='1';
						SendToArduino[SendToArduinoPosition[6]]='1';
					}
				}
			}
		}
		
		if(langDest == 1){		
			// Detecção de destino ////////////////////////////////////////////////////////////////////////////
			target = GameObject.Find("Target");
			player = GameObject.Find("Personagem");
				
			var heading = target.transform.position - player.transform.position;
			Vector3 forward = transform.forward;
			Vector3 referenceRight= Vector3.Cross(Vector3.up, forward);
	        
			float angle = Vector3.Angle(heading, forward);
			float sign = (Vector3.Dot(heading, referenceRight) > 0.0f) ? 1.0f: -1.0f;
			float finalAngle = sign * angle;		
	
			if((finalAngle <= 67.5)&&(finalAngle > 22.5)) SendToArduino[2]='2';
			else if((finalAngle <= 22.5)&&(finalAngle > -22.5)) SendToArduino[0]='2';
			else if((finalAngle <= -22.5)&&(finalAngle > -67.5)) SendToArduino[14]='2';
			else if((finalAngle <= -67.5)&&(finalAngle > -112.5)) SendToArduino[12]='2';
			else if((finalAngle <= -112.5)&&(finalAngle > -157.5)) SendToArduino[10]='2';
			else if((finalAngle <= -157.5)&&(finalAngle >= -180)) SendToArduino[6]='2';
			else if((finalAngle <= 180)&&(finalAngle > 157.5)) SendToArduino[6]='2';
			else if((finalAngle <= 157.5)&&(finalAngle > 112.5)) SendToArduino[8]='2';
			else if((finalAngle <= 112.5)&&(finalAngle > 67.5)) SendToArduino[4]='2';
		}
		
		// Envia para o display tátil /////////////////////////////////////////////////////////////////////
		if(end == 1)
			SendToArduino= new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};

		_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
	}
	
	void AskForDestinationMdfr(){
		// Detecção de destino //
		target = GameObject.Find("Target");
		player = GameObject.Find("Personagem");
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		
		switch(ctrlDest){
			case 0:				
				SendToArduino[SendToArduinoPosition[5]]='1';			
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 1:					
				var heading = target.transform.position - player.transform.position;
				Vector3 forward = transform.forward;
				Vector3 referenceRight= Vector3.Cross(Vector3.up, forward);
		        
				float angle = Vector3.Angle(heading, forward);
				float sign = (Vector3.Dot(heading, referenceRight) > 0.0f) ? 1.0f: -1.0f;
				float finalAngle = sign * angle;		
		
				if((finalAngle <= 67.5)&&(finalAngle > 22.5)) SendToArduino[2]='1';
				else if((finalAngle <= 22.5)&&(finalAngle > -22.5)) SendToArduino[0]='1';
				else if((finalAngle <= -22.5)&&(finalAngle > -67.5)) SendToArduino[14]='1';
				else if((finalAngle <= -67.5)&&(finalAngle > -112.5)) SendToArduino[12]='1';
				else if((finalAngle <= -112.5)&&(finalAngle > -157.5)) SendToArduino[10]='1';
				else if((finalAngle <= -157.5)&&(finalAngle >= -180)) SendToArduino[6]='1';
				else if((finalAngle <= 180)&&(finalAngle > 157.5)) SendToArduino[6]='1';
				else if((finalAngle <= 157.5)&&(finalAngle > 112.5)) SendToArduino[8]='1';
				else if((finalAngle <= 112.5)&&(finalAngle > 67.5)) SendToArduino[4]='1';			
			
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 3:				
				SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};			
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 4:	
				triggerDest = 0;
				break;
		}
		
		if(enablePatMod == 1){			
			enablePatMod = 0;
			ctrlDest++;
			timerPattMod.Elapsed += new ElapsedEventHandler(EnablePatMod);
			timerPattMod.Start();			
		}
	}
	
	void AskForWarningMdfr(){
		// Detecção de alerta //
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		
		switch(ctrlWarn){
			case 0:				
				SendToArduino[SendToArduinoPosition[4]]='1';
				SendToArduino[SendToArduinoPosition[6]]='1';
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 1:					
				SendToArduino[SendToArduinoPosition[5]]='1';			
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 3:				
				SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};			
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 4:	
				onAlerta = -1;
				break;
		}
		
		if(enablePatMod == 1){			
			enablePatMod = 0;
			ctrlWarn++;
			timerPattMod.Elapsed += new ElapsedEventHandler(EnablePatMod);
			timerPattMod.Start();			
		}
	}
	
	void AskForIntineraryMdfr(int onAlerta){
		// Detecção de intinerario //
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		
		switch(ctrlIntn){
			case 0:				
				SendToArduino[SendToArduinoPosition[5]]='2';
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 1:				
				SendToArduino[SendToArduinoPosition[5]]='2';
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 2:					
				if(onAlerta == 2)
					modDir = new int[]{1, 7, 5, 3};
				else if(onAlerta == 3)
					modDir = new int[]{3, 1, 7, 5};
				else if(onAlerta == 4)
					modDir = new int[]{5, 3, 1, 7};
				else if(onAlerta == 5)
					modDir = new int[]{7, 5, 3, 1};				
				
				if(transform.localEulerAngles.y > 315 || transform.localEulerAngles.y < 45){
					SendToArduino[SendToArduinoPosition[modDir[0]]]='1';
				}else if(transform.localEulerAngles.y > 45 && transform.localEulerAngles.y < 135){
					SendToArduino[SendToArduinoPosition[modDir[1]]]='1';
				}else if(transform.localEulerAngles.y > 135 && transform.localEulerAngles.y < 225){
					SendToArduino[SendToArduinoPosition[modDir[2]]]='1';
				}else if(transform.localEulerAngles.y > 225 && transform.localEulerAngles.y < 315){
					SendToArduino[SendToArduinoPosition[modDir[3]]]='1';
				}
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 4:				
				SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};			
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 5:	
				onAlerta = -1;
				break;
		}
		
		if(enablePatMod == 1){			
			enablePatMod = 0;
			ctrlIntn++;
			timerPattMod.Elapsed += new ElapsedEventHandler(EnablePatMod);
			timerPattMod.Start();			
		}
	}
	
	void AskForCourseMdfr(){
		// Detecção de fluxo //
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		
		switch(ctrlCour){
			case 0:				
				SendToArduino[SendToArduinoPosition[4]]='1';
				SendToArduino[SendToArduinoPosition[6]]='1';
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 1:					
				if(courseName == "CourseH"){
					if((transform.localEulerAngles.y > 337.5) || (transform.localEulerAngles.y < 22.5) || (transform.localEulerAngles.y > 157.5 && transform.localEulerAngles.y < 202.5)){
						SendToArduino[SendToArduinoPosition[3]]='1';
						SendToArduino[SendToArduinoPosition[7]]='1';
					} 
					else if((transform.localEulerAngles.y > 22.5 && transform.localEulerAngles.y < 67.5) || (transform.localEulerAngles.y > 202.5 && transform.localEulerAngles.y < 247.5)){
						SendToArduino[SendToArduinoPosition[2]]='1';
						SendToArduino[SendToArduinoPosition[6]]='1';
					} 
					else if((transform.localEulerAngles.y > 67.5 && transform.localEulerAngles.y < 112.5) || (transform.localEulerAngles.y > 247.5 && transform.localEulerAngles.y < 292.5)){
						SendToArduino[SendToArduinoPosition[1]]='1';
						SendToArduino[SendToArduinoPosition[5]]='1';
					} 
					else if((transform.localEulerAngles.y > 112.5 && transform.localEulerAngles.y < 157.5) || (transform.localEulerAngles.y > 292.5 && transform.localEulerAngles.y < 337.5)){
						SendToArduino[SendToArduinoPosition[0]]='1';
						SendToArduino[SendToArduinoPosition[4]]='1';
					}
				}
				else{ // CourseV
					if((transform.localEulerAngles.y > 337.5) || (transform.localEulerAngles.y < 22.5) || (transform.localEulerAngles.y > 157.5 && transform.localEulerAngles.y < 202.5)){
						SendToArduino[SendToArduinoPosition[1]]='1';
						SendToArduino[SendToArduinoPosition[5]]='1';
					} 
					else if((transform.localEulerAngles.y > 22.5 && transform.localEulerAngles.y < 67.5) || (transform.localEulerAngles.y > 202.5 && transform.localEulerAngles.y < 247.5)){
						SendToArduino[SendToArduinoPosition[0]]='1';
						SendToArduino[SendToArduinoPosition[4]]='1';
					} 
					else if((transform.localEulerAngles.y > 67.5 && transform.localEulerAngles.y < 112.5) || (transform.localEulerAngles.y > 247.5 && transform.localEulerAngles.y < 292.5)){
						SendToArduino[SendToArduinoPosition[3]]='1';
						SendToArduino[SendToArduinoPosition[7]]='1';
					} 
					else if((transform.localEulerAngles.y > 112.5 && transform.localEulerAngles.y < 157.5) || (transform.localEulerAngles.y > 292.5 && transform.localEulerAngles.y < 337.5)){
						SendToArduino[SendToArduinoPosition[2]]='1';
						SendToArduino[SendToArduinoPosition[6]]='1';
					}
				}
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 3:				
				SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};			
				_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
				break;
			
			case 4:	
				onFluxo = -1;
				break;
		}
		
		if(enablePatMod == 1){			
			enablePatMod = 0;
			ctrlCour++;
			timerPattMod.Elapsed += new ElapsedEventHandler(EnablePatMod);
			timerPattMod.Start();			
		}
	}	
	
	void AskForObstacleMdfr(){	
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		Vector3 positionAbove = new Vector3();
		Vector3[] vectors = new Vector3[24];
		Vector3[] direction = new Vector3[24];
		RaycastHit hit1;
		float Dx, Dz, Mxqd, px, pz, scannAbov;
		float[] dist = {2.82F, 2.40F, 2.10F, 2.0F};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		int rep;
		int j = 0;
		int id = 0;
		int inc = 1;
		ctrlDest = 0;
		ctrlCour = 0;
		ctrlIntn = 0;
		ctrlWarn = 0;

		// Detecção de obstáculos /////////////////////////////////////////////////////////////////////////
		if(langObst == 1){			
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
			for(int i = 0; i < 24; i++){ direction[i] = transform.TransformDirection(vectors[i]); }		
			rep = 2; //flag
			positionAbove = transform.position;
			positionAbove.y = scannAbov;
						
			for(int i = 0; i < 8; i++){
				while(rep>0){
					if(Physics.Raycast(transform.position, direction[j], out hit1, dist[id])){ 
						if((j < 10)||(j > 20))
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
			if(Physics.Raycast(transform.position, direction[23], out hit1, 2.40F)) 
				SendToArduino[SendToArduinoPosition[0]]='1';
		}

		// Envia para o display tátil /////////////////////////////////////////////////////////////////////
		if(end == 1)
			SendToArduino= new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};

		_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
	}
	
	void EnableDraw(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		enableDraw = 1;
	}
	
	void EnablePatMod(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		enablePatMod = 1;
	}
	
	void DrawWave(string wave){
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		int[] SendToArduinoPosition = {14, 0, 2, 4, 8, 6, 10, 12};
		
		if(enableDraw == 1){			
			enableDraw = 0;			
			if(toRight<7) toRight++;
			else toRight=0;
			if(toLeft>0) toLeft--;
			else toLeft=7;
			timerDrawWave.Elapsed += new ElapsedEventHandler(EnableDraw);
			timerDrawWave.Start();			
		}
		
		switch(wave){
			case "Forward":
				if((toLeft>=2)&&(toLeft<=4)){
					SendToArduino[SendToArduinoPosition[toRight]]='1';
					SendToArduino[SendToArduinoPosition[toLeft]]='1';
				}
				break;
			case "Backward":
				if((toRight>=2)&&(toRight<=4)){
					SendToArduino[SendToArduinoPosition[toRight]]='1';
					SendToArduino[SendToArduinoPosition[toLeft]]='1';
				}
				break;
			case "Left":
				if((toLeft>=0)&&(toLeft<=2))
					SendToArduino[SendToArduinoPosition[toLeft]]='1';
				break;
			case "Right":
				if((toRight>=0)&&(toRight<=2))
					SendToArduino[SendToArduinoPosition[toRight]]='1';
				break;
		}		
		_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
	}
	
	void GravaLog(){
		PlayerPrefs.SetInt("Score", score);
		PlayerPrefs.Save();
		string User = PlayerPrefs.GetString("User");
		int Id = PlayerPrefs.GetInt("Id");
		
		Debug.Log("Participante " + Id + ": " + User);
		Debug.Log("Fase: " + currentPhase);
		Debug.Log("Pontuacao: " + score);
		Debug.Log("Tempo inicial: " + initDateTime);
		Debug.Log("Tempo final: " + endDateTime);
		Debug.Log("Colisoes em barris: " + qtdColisoes);
		Debug.Log("Trajetória:" + trajeto);
	}		
			
	void CarregarMenu(int currPhase){
		OnApplicationQuit();
		switch(currPhase){			
			case 1:
				Application.LoadLevel("menuTeste2");
				break;
			case 2:
				Application.LoadLevel("menuTeste3");
				break;
			case 3:
				Application.LoadLevel("menuTeste4");
				break;
			case 4:
				Application.LoadLevel("Fim");
				break;
		}
	}
	
	void OnApplicationQuit() {
		if(end == 0){
			endDateTime = DateTime.Now.ToString("hh:mm:ss");
			GravaLog();
		}
		
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
		
		_SerialPort.Close();
		portopen = false;
		print ("A porta serial foi fechada");
	}	
}