using UnityEngine;
using System.Collections;
using System;
using System.IO.Ports;
using System.Timers;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;


public class BratrackUDPReceiver : MonoBehaviour {

	//UDP variables
	public int UDPport = 3000;
	private UdpClient client;
	private IPEndPoint RemoteIpEndPoint;
	private Regex regexParse;
	private Thread t_udp;

	public static double R1X,R1Y,R1Z,R2X,R2Y,R2Z,R3X,R3Y,R3Z,TX,TY,TZ;
	//



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
	public static float speed = 5.0F; //80.0F
	public float jumpSpeed = 1.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	public int idleControl = 0;
	// Logs
	public string initDateTime;
	public string endDateTime;
	public int qtdColisoes = 0;
	public int qtdColisoesOLD = 0;
	public string trajeto = "";
	public int end = 0;
	public int start = 0;
	// Sounds
	public int enableSound = 1;
	public System.Timers.Timer timerMoveSprite = new System.Timers.Timer(1000);
	// Modifier patterns
	public int[] modDir = {0, 0, 0, 0};
	public int onAlerta = 0;
	// Game/User info
	public static int Id;
	public string currentPhase = "";
	// Practice Trial
	public int practicetrial = 0;
	GameObject personagem;
	
	// Cenário por usuário	// 24
	public static string[,] sceneConfig = { 
		{ "Scene1", "Scene2", "Scene3" },
		{ "Scene1", "Scene3", "Scene2" },
		{ "Scene2", "Scene1", "Scene3" },
		{ "Scene2", "Scene3", "Scene1" },
		{ "Scene3", "Scene1", "Scene2" },
		{ "Scene3", "Scene2", "Scene1" },
		{ "Scene1", "Scene2", "Scene3" },
		{ "Scene1", "Scene3", "Scene2" },
		{ "Scene2", "Scene1", "Scene3" },
		{ "Scene2", "Scene3", "Scene1" },
		{ "Scene3", "Scene1", "Scene2" },
		{ "Scene3", "Scene2", "Scene1" },
		{ "Scene1", "Scene2", "Scene3" },
		{ "Scene1", "Scene3", "Scene2" },
		{ "Scene2", "Scene1", "Scene3" },
		{ "Scene2", "Scene3", "Scene1" },
		{ "Scene3", "Scene1", "Scene2" },
		{ "Scene3", "Scene2", "Scene1" },
		{ "Scene1", "Scene2", "Scene3" },
		{ "Scene1", "Scene3", "Scene2" },
		{ "Scene2", "Scene1", "Scene3" },
		{ "Scene2", "Scene3", "Scene1" },
		{ "Scene3", "Scene1", "Scene2" },
		{ "Scene3", "Scene2", "Scene1" }
	};
	
	GameObject fum0;
	GameObject fum1;
	GameObject fum2;
	GameObject fum3;
	public string trajprev = "";
	public Vector3 previous = Vector3.zero;
	public string coordTriggers = "";
	short flag1 = 0;
	short flag2 = 0;
	short flag3 = 0;
	short flag4 = 0;
	
	
	void Awake(){
		//DontDestroyOnLoad(this);
	}
	
	void Start (){

		//UDP code
		client = new UdpClient(UDPport);
		RemoteIpEndPoint = new IPEndPoint(IPAddress.Any,0);
		regexParse = new Regex(@"\d*$");
		t_udp = new Thread (new ThreadStart (UDPRead));
		t_udp.Name = "Mindtuner UDP thread";
		t_udp.Start();
		//


		fum0 = GameObject.Find("Fumes0");
		fum1 = GameObject.Find("Fumes1");
		fum2 = GameObject.Find("Fumes2");
		fum3 = GameObject.Find("Fumes3");
		personagem = GameObject.Find("Personagem");
		
		Id = PlayerPrefs.GetInt("Id");
		
		currentPhase = Application.loadedLevelName;
		CarregarNomeCaminhada(currentPhase);
		
		TurnOffFumes();
		//OpenConnection();
	


	}
	/*

		//start the serial connection.
		//OpenConnection();
		trajprev = "(164.7, -134.0, -854.8) - (164.3, -134.0, -854.2) - (162.6, -134.0, -852.8) - (161.2, -134.0, -852.0) - (161.2, -134.0, -852.0) - (159.8, -134.0, -849.2) - (157.8, -134.0, -845.7) - (155.7, -134.0, -842.2) - (153.4, -134.0, -838.8) - (151.1, -134.0, -835.3) - (148.8, -134.0, -831.8) - (146.6, -134.0, -828.6) - (143.8, -134.0, -825.8) - (141.3, -134.0, -822.8) - (138.8, -134.0, -819.6) - (136.5, -134.0, -816.1) - (134.5, -134.0, -812.9) - (134.0, -134.0, -810.7) - (131.9, -134.0, -807.6) - (129.2, -134.0, -804.6) - (126.8, -134.0, -801.2) - (124.6, -134.0, -797.7) - (122.4, -134.0, -794.1) - (120.0, -134.0, -790.8) - (117.5, -134.0, -787.8) - (115.2, -134.0, -784.4) - (113.1, -134.0, -780.9) - (111.0, -134.0, -777.3) - (109.3, -134.0, -773.5) - (107.4, -134.0, -770.1) - (105.4, -134.0, -766.4) - (103.3, -134.0, -762.8) - (101.2, -134.0, -759.3) - (98.9, -134.0, -755.9) - (96.6, -134.0, -752.5) - (94.3, -134.0, -749.0) - (92.0, -134.0, -746.1) - (89.3, -134.0, -743.7) - (88.5, -134.0, -741.1) - (88.2, -134.0, -736.9) - (88.5, -134.0, -732.8) - (89.3, -134.0, -729.1) - (91.6, -134.0, -727.1) - (95.1, -134.0, -725.1) - (98.6, -134.0, -723.1) - (102.2, -134.0, -721.1) - (105.7, -134.0, -719.0) - (109.2, -134.0, -717.0) - (112.8, -134.0, -715.0) - (116.4, -134.0, -713.0) - (120.1, -134.0, -711.1) - (123.8, -134.0, -709.2) - (127.5, -134.0, -707.5) - (131.3, -134.0, -705.8) - (134.9, -134.0, -704.0) - (138.4, -134.0, -701.9) - (142.1, -134.0, -699.9) - (145.8, -134.0, -697.9) - (148.0, -134.0, -696.6) - (151.4, -134.0, -694.6) - (155.0, -134.0, -692.8) - (158.5, -134.0, -690.8) - (162.0, -134.0, -688.6) - (165.6, -134.0, -686.4) - (169.1, -134.0, -684.2) - (172.7, -134.0, -682.1) - (176.2, -134.0, -679.9) - (179.8, -134.0, -677.7) - (183.3, -134.0, -675.5) - (186.9, -134.0, -673.3) - (190.4, -134.0, -671.2) - (194.0, -134.0, -669.0) - (197.4, -134.0, -666.8) - (200.6, -134.0, -664.3) - (203.8, -134.0, -661.9) - (207.2, -134.0, -660.5) - (210.9, -134.0, -660.3) - (213.8, -134.0, -661.0) - (216.1, -134.0, -663.0) - (217.7, -134.0, -666.4) - (219.6, -134.0, -670.1) - (221.6, -134.0, -673.7) - (223.7, -134.0, -677.3) - (225.7, -134.0, -681.0) - (227.8, -134.0, -684.6) - (229.8, -134.0, -688.2) - (231.8, -134.0, -691.9) - (233.5, -134.0, -695.7) - (235.2, -134.0, -699.5) - (236.7, -134.0, -703.4) - (238.0, -134.0, -707.3) - (238.6, -134.0, -711.0) - (238.5, -134.0, -713.6) - (240.0, -134.0, -716.2) - (242.7, -134.0, -719.0) - (245.8, -134.0, -721.9) - (248.2, -134.0, -725.1) - (250.4, -134.0, -728.4) - (252.1, -134.0, -732.2) - (254.0, -134.0, -735.2) - (256.7, -134.0, -737.3) - (260.0, -134.0, -738.1) - (263.4, -134.0, -737.7) - (267.1, -134.0, -736.1) - (270.6, -134.0, -734.2) - (274.1, -134.0, -732.0) - (277.6, -134.0, -729.9) - (281.4, -134.0, -727.7) - (285.0, -134.0, -725.6) - (288.6, -134.0, -723.6) - (292.3, -134.0, -721.4) - (295.9, -134.0, -719.4) - (299.6, -134.0, -717.3) - (303.3, -134.0, -715.2) - (306.8, -134.0, -713.1) - (304.5, -134.0, -715.2) - (306.0, -134.0, -713.5) - (307.9, -134.0, -713.4) - (310.0, -134.0, -711.5) - (312.4, -134.0, -709.9) - (317.3, -134.0, -710.0) - (321.2, -134.0, -708.8) - (324.0, -134.0, -706.2) - (326.9, -134.0, -703.4) - (328.8, -134.0, -700.3) - (330.0, -134.0, -697.3) - (330.1, -134.0, -695.9) - (330.3, -134.0, -693.4) - (329.9, -134.0, -690.2) - (329.8, -134.0, -689.9) - (330.3, -134.0, -689.8) - (331.5, -134.0, -685.8) - (330.3, -134.0, -684.3) - (328.2, -134.0, -682.4) - (324.3, -134.0, -680.5) - (319.1, -134.0, -679.3) - (313.7, -134.0, -678.5) - ";
		Vector3 posSq;
		int colisSq = 0;
		if(trajprev != ""){
			previous = new Vector3(164.6801F, -134.0165F, -854.8124F); //mudar pra cada fase
			int cont = 0;
			string [] split = trajprev.Split(new Char [] {'(', ')'});
			//Debug.Log(trajprev);
			
			foreach (string s in split) {
				
				GameObject retaTraj = GameObject.CreatePrimitive(PrimitiveType.Plane);
				retaTraj.transform.localScale = new Vector3(0.03F, 0.03F, 0.03F);
				retaTraj.collider.enabled = false;
				
				if(cont%2 != 0){
					string [] splits = s.Split(new Char [] {',', ' '});
					//Debug.Log(splits[0] + "-" + splits[4]);
					posSq = new Vector3(System.Convert.ToSingle(splits[0]), -134.0f, System.Convert.ToSingle(splits[4]));	

					retaTraj.transform.position = posSq;
					Debug.DrawLine (previous, posSq, Color.white, 200, false);
					previous = posSq;
					
					Collider[] hitColliders = Physics.OverlapSphere(posSq, 0.59F);
					int i = 0;
					while (i < hitColliders.Length) {
						if(hitColliders[i].gameObject.name == "TriggerStart"){
							if(flag1 == 0){
								flag1 = 1;
								coordTriggers = coordTriggers + posSq.ToString() +  (cont/2).ToString() + "\n";
							}
							retaTraj.renderer.material.color = Color.red;
						}else if(hitColliders[i].gameObject.name == "TriggerFume0"){
							if(flag2 == 0){
								flag2 = 1;
								coordTriggers = coordTriggers + posSq.ToString()+  (cont/2).ToString() + "\n";
							}
							retaTraj.renderer.material.color = Color.red;
						}else if(hitColliders[i].gameObject.name == "TriggerFume1"){
							if(flag3 == 0){
								flag3 = 1;
								coordTriggers = coordTriggers + posSq.ToString()+  (cont/2).ToString() + "\n";
							}
							retaTraj.renderer.material.color = Color.red;
						}else if(hitColliders[i].gameObject.name == "TriggerFume2"){
							if(flag4 == 0){
								flag4 = 1;
								coordTriggers = coordTriggers + posSq.ToString()+  (cont/2).ToString() + "\n";
							}
							retaTraj.renderer.material.color = Color.red;
						}
						//Debug.Log("Colisoes em: " + hitColliders[i].gameObject.name);
						i++;
					}
				}
				cont++;
				
				//retaTraj.renderer.material.color = Color.red;	
			}
			
			Debug.Log("Coordenadas: " + coordTriggers + split.Length);
		}
	}*/
	
	void MoveSprite(object source, ElapsedEventArgs e){
		// Código a ser executado quando o intervalo for atingido
		enableSound = 1;
	}
	
	void OnControllerColliderHit(ControllerColliderHit obj){
		
		if(obj.gameObject.name == "RockObstacle"){
			qtdColisoesOLD++; // como versão antiga			
			
			if(enableSound == 1){
				obj.gameObject.audio.Play();
				enableSound = 0;
				timerMoveSprite.Elapsed += new ElapsedEventHandler(MoveSprite);
				timerMoveSprite.Start();
				qtdColisoes++;
			}
		}
		
		// Triggers: dispara a fumaça
		switch(obj.gameObject.name){
		case "TriggerStart":	
			fum0.SetActive(true);
			break;
		case "TriggerFume0":
			fum3.SetActive(true);
			break;
		case "TriggerFume1":
			fum1.SetActive(true);
			break;
		case "TriggerFume2":	
			fum2.SetActive(true);
			break;
		}
		
		// Alertas e Itinerario
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
	}
	
	void OnTriggerEnter (Collider myTrigger) {
		GameObject chegadaMsg = GameObject.Find("chegadaMsg");
		GameObject lanterna = GameObject.Find("lanterna");
		// Mudar mensagem de termino da fase a depender da ordem
		if(myTrigger.gameObject.name == "Chegada"){	
			TurnOffFumes(); // Ao inves de desabilitar, reduzir emissao a zero
			lanterna.light.enabled = false;
			switch(currentPhase){
			case "PracticeScene":
				chegadaMsg.guiText.text = "Welcome\n to your destination.\n\nPress B to continue.";
				break;
			case "Scene1":
			case "Scene2":
			case "Scene3":
				chegadaMsg.guiText.text = "Bem-vindo\n ao seu destino.\n\nPressione B para continuar.";
				break;
			}
			endDateTime = DateTime.Now.ToString("hh:mm:ss");
			end = 1;
			GravaLog();	
		}
	}
	
	void TurnOffFumes(){
		fum0.SetActive (false);
		fum1.SetActive (false);
		fum2.SetActive (false);
		fum3.SetActive (false);
	}
	
	void Update (){


		//UDP data
		if (t_udp != null)
			print ("IT's alive " + t_udp.IsAlive);
		else
			print ("Packets are not comming");



		//// Movimento ////////////////////////////////////////////////////////////////////////////////////
		CharacterController controller = GetComponent<CharacterController>();
		if (controller.isGrounded) {
			moveDirection = new Vector3(0, 0,Input.GetAxis("Vertical"));

			moveDirection = transform.TransformDirection(moveDirection);

			moveDirection *= speed;
			//print("("+ moveDirection.x +", "+ moveDirection.y +", "+ moveDirection.z +")");
			
			var newRotation = Input.GetAxis("Horizontal") * rotatationSpeed;
			transform.Rotate(0, newRotation * Time.deltaTime, 0);

			//Vector3 oneRotation = new Vector3((float)R1X,(float)R1Y,(float)R1Z);
			//transform.Rotate(oneRotation);

		}

		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move(moveDirection * Time.deltaTime);
		
		//// Captura informação do ambiente e envia ao display tátil //////////////////////////////////////
		updateslower++;		
		if(portopen && _SerialPort.IsOpen && updateslower>=updateinterval){
			updateslower = 0;
			if(currentPhase != sceneConfig[Id, 1])
				AskForInput();
		}else if(! _SerialPort.IsOpen){
			//print ("Não tá aberta, tosco");
		}
		
		updateintervalbigger++;
		if(updateintervalbigger >= 50){
			updateintervalbigger = 0;
			//// Registra trajetória do usuário ///////////////////////////////////////////////////////////
			trajeto = trajeto + controller.transform.position.ToString() + " - ";
			if(start == 0){
				start = 1;
				initDateTime = DateTime.Now.ToString("hh:mm:ss");
			}
			
			//// Controle da Fumaça ///////////////////////////////////////////////////////////////////////
			if(fum0.activeSelf == true){
				foreach(GameObject fume in GameObject.FindGameObjectsWithTag("fumes")){
					if(fume.particleEmitter.maxEmission <3){
						fume.particleEmitter.maxEmission+=0.1f;
					}else if(fume.particleEmitter.maxEnergy <5){
						fume.particleEmitter.maxEnergy+=0.1f;
					}
				}
			}
			if(fum1.activeSelf == true){
				foreach(GameObject fume in GameObject.FindGameObjectsWithTag("fumes")){
					if(fume.particleEmitter.maxEmission <20){
						fume.particleEmitter.maxEmission+=0.5f;
					}
				}
			}
		}	
		
		//// Input ////////////////////////////////////////////////////////////////////////////////////////
		if(Input.GetKey(KeyCode.Escape))
			Application.Quit();
		
		if(Input.GetKey(KeyCode.F))
			Screen.SetResolution(3840, 1080, false);
		
		if(Input.GetKey(KeyCode.J))
			JumpPhase();
		
		if(Input.GetKey(KeyCode.R))
			ReturnPhase();
		
		if((Input.GetKey(KeyCode.A))||(Input.GetButton("joystick button 0"))){
			if(currentPhase == "PracticeScene" && end == 1){
				GameObject chegadaMsg = GameObject.Find("chegadaMsg");
				GameObject lanterna = GameObject.Find("lanterna");
				chegadaMsg.guiText.text = "";
				lanterna.light.enabled = true;

				switch(practicetrial){
				case 0:
					personagem.transform.position = new Vector3 (483.8059f, -134.0165f, -585.5862f);
					//posicionar alerta unico
					practicetrial++;
					break;
				case 1:
					personagem.transform.position = new Vector3 (590.4622f, -134.0165f, -668.4197f);
					//posicionar alerta unico
					practicetrial++;
					break;
				case 2:
					personagem.transform.position = new Vector3 (538.8873f, -134.0165f, -687.32f);
					//posicionar alerta unico
					practicetrial++;
					break;
				default:
					practicetrial = 0;
					break;
				}
				end = 0;
			}
		}
		
		if((Input.GetKey(KeyCode.B))||(Input.GetButton("joystick button 1"))){
			if(end == 1) CarregarMenu(currentPhase);
		}






	}	
	
	void JumpPhase(){
		end = 1;
		GravaLog();
		
		CarregarMenu(currentPhase);
	}
	
	void ReturnPhase(){
		//reset
		end = 1;
		GravaLog();		
		OnApplicationQuit();		
		Application.LoadLevel("Inicio");
	}
	
	void CarregarNomeCaminhada(string currPhase){
		GameObject caminhadaName = GameObject.Find("Caminhada");
		if(currentPhase == sceneConfig[Id, 0]){
			caminhadaName.guiText.text = "Primeira Caminhada";
		}
		else if(currentPhase == sceneConfig[Id, 1]){
			caminhadaName.guiText.text = "Segunda Caminhada";
		}
		else if(currentPhase == sceneConfig[Id, 2]){
			caminhadaName.guiText.text = "Terceira Caminhada";
			
		}
	}
	
	void CarregarMenu(string currPhase){
		OnApplicationQuit();
		
		if(currentPhase == "PracticeScene"){
			GravaLog();
			Application.LoadLevel("Inicio");
		} 
		else if(currentPhase == sceneConfig[Id, 0]){
			Application.LoadLevel(sceneConfig[Id, 1]);
		}
		else if(currentPhase == sceneConfig[Id, 1]){
			Application.LoadLevel(sceneConfig[Id, 2]);
		}
		else if(currentPhase == sceneConfig[Id, 2]){
			Application.LoadLevel("Inicio");
			
		}
	}
	
	public static void OpenConnection(){
		if (_SerialPort != null){
			if (_SerialPort.IsOpen){
				_SerialPort.Close();
				print("Closing port, because it was already open!");
			} else{
				//print (_SerialPort);
				_SerialPort.Open();
				_SerialPort.ReadTimeout = 100;
				portopen = true;
			}
		}
		else{
			_SerialPort = new SerialPort(COMPort, 57600);
			print ("kjhsakjhgakd");
			if (_SerialPort.IsOpen){
				print("Port is already open");
			} else{
				//print("Port == null");
				_SerialPort.Open();
			}
		}
	}
	
	public static void CloseConnection(){
		if (_SerialPort.IsOpen){
			_SerialPort.Close();
			print("Closing port!");
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
		
		// Detecção de alerta /////////////////////////////////////////////////////////////////////////////
		if(onAlerta == 1){
			SendToArduino[SendToArduinoPosition[4]]='1'; 
			SendToArduino[SendToArduinoPosition[6]]='1';
		}
		
		// Detecção de itinerario /////////////////////////////////////////////////////////////////////////
		if(onAlerta > 1){
			SendToArduino= new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
			SendToArduino[SendToArduinoPosition[5]]='1';
			
			if(onAlerta == 2)
			modDir = new int[]{1, 7, 5, 3};
			else if(onAlerta == 3)
			modDir = new int[]{3, 1, 7, 5};
			else if(onAlerta == 4)
			modDir = new int[]{5, 3, 1, 7};
			else if(onAlerta == 5)
			modDir = new int[]{7, 5, 3, 1};				
			
			if(transform.localEulerAngles.y > 105 && transform.localEulerAngles.y < 195){
				SendToArduino[SendToArduinoPosition[modDir[0]]]='1';
			}else if(transform.localEulerAngles.y > 195 && transform.localEulerAngles.y < 285){
				SendToArduino[SendToArduinoPosition[modDir[1]]]='1';
			}else if(transform.localEulerAngles.y > 285 || transform.localEulerAngles.y < 15){
				SendToArduino[SendToArduinoPosition[modDir[2]]]='1';
			}else if(transform.localEulerAngles.y > 15 && transform.localEulerAngles.y < 105){
				SendToArduino[SendToArduinoPosition[modDir[3]]]='1';
			}
		}
		
		// Envia para o display tátil /////////////////////////////////////////////////////////////////////
		if(end == 1)
		SendToArduino= new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		
		ImprimeTacton(SendToArduino);
		_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
		onAlerta = -1;
	}
	
	public static void ImprimeTacton(char[] ArrayTactons) {
		int j = 0;
		string[] motors = {"Sphere1", "Sphere2", "Sphere3", "Sphere5", "Sphere4", "Sphere6", "Sphere7", "Sphere8"};
		GameObject sphere;
		
		for (int i=0; i < 8; i++) {
			if(ArrayTactons[j]=='1') {
				sphere = GameObject.Find(motors[i]);
				sphere.renderer.material.color = Color.red;
			}			
			if(ArrayTactons[j]=='0'){
				sphere = GameObject.Find(motors[i]);
				sphere.renderer.material.color = Color.white;
				/*if(motors[i] == "Sphere4" || motors[i] == "Sphere5" || motors[i] == "Sphere6")
						plano.renderer.material.color = Color.gray;*/
			}
			j += 2;
		}
	}
	
	void GravaLog(){
		// transform.localEulerAngles.y -> gravar inclinaçao do personagem
		Debug.Log("Participante " + Id);
		Debug.Log("Fase: " + currentPhase);
		Debug.Log("Tempo inicial: " + initDateTime);
		Debug.Log("Tempo final: " + endDateTime);
		Debug.Log("Colisoes em barris: " + qtdColisoes);
		Debug.Log("Colisoes ANTIGAS em barris: " + qtdColisoesOLD);
		Debug.Log("Trajetória:" + trajeto);
	}		
	
	void OnApplicationQuit() {
		if(end == 0){
			endDateTime = DateTime.Now.ToString("hh:mm:ss");
			GravaLog();
		}
		
		char[] SendToArduino = new char[] {'0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0', ',', '0'};
		if (_SerialPort.IsOpen){
			ImprimeTacton(SendToArduino);
			_SerialPort.Write(SendToArduino, 0, SendToArduino.Length);
		}
		
		CloseConnection();
		portopen = false;
		print ("A porta serial foi fechada");

		//UDP thread closed
		if (t_udp != null) t_udp.Abort();
		print ("UDP thread closed");
	}	


	public void UDPRead()
	{
		while (true)
		{
			try
			{
				//print ("listening UDP port " + UDPport);
				byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
				//string returnData = Encoding.ASCII.GetString(receiveBytes);
			
				int byteLeap = 8;
				int byteOffset = 1;
				int incrementLeap=0;
				byte[] subarray = new byte[8];


				Array.Copy(receiveBytes,0,subarray,0,byteLeap);
				long timestamp = BitConverter.ToInt64(subarray,0);
				//print (timestamp);

				Array.Copy(receiveBytes,byteLeap,subarray,0,byteLeap);
				string returnData = Encoding.ASCII.GetString(subarray);
				//print (returnData);

				incrementLeap = byteLeap*2+byteOffset;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				R1X = BitConverter.ToDouble(subarray,0 );

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				R1Y = BitConverter.ToDouble( subarray, 0 );

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				R1Z = BitConverter.ToDouble( subarray, 0 );

				//print ("R1X:" + R1X + " R1Y:" + R1Y + " R1Z:" + R1Z); 

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				R2X = BitConverter.ToDouble( subarray, 0 );

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				R2Y = BitConverter.ToDouble( subarray, 0 );

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				R2Z = BitConverter.ToDouble( subarray, 0 );

				//print ("R2X:" + R2X + " R2Y:" + R2Y + " R2Z:" + R2Z); 

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				R3X = BitConverter.ToDouble( subarray, 0 );

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				R3Y = BitConverter.ToDouble( subarray, 0 );

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				R3Z = BitConverter.ToDouble( subarray, 0 );

				//print ("R3X:" + R3X + " R3Y:" + R3Y + " R3Z:" + R3Z);

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				TX = BitConverter.ToDouble( subarray, 0 );

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				TY = BitConverter.ToDouble( subarray, 0 );

				incrementLeap = incrementLeap + byteLeap;
				Array.Copy(receiveBytes,incrementLeap,subarray,0,byteLeap);
				TZ = BitConverter.ToDouble( subarray, 0 );


				//print ("TX:" + TX + " TY:" + TY + " TZ:" + TZ );


			}
			catch (Exception e)
			{
				Debug.Log("Not so good " + e.ToString());
			}
			Thread.Sleep(20);
		}
	}


}







/*
public class Mindtuner : MonoBehaviour
{

	void Start()
	{
		client = new UdpClient(port);
		RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
		regexParse = new Regex(@"\d*$");
		t_udp = new Thread (new ThreadStart (UDPRead));
		t_udp.Name = "Mindtuner UDP thread";
		t_udp.Start();
	}
	
	public void UDPRead()
	{
		while (true)
		{
			try
			{
				Debug.Log("listening UDP port " + port);
				byte[] receiveBytes = client.Receive(ref RemoteIpEndPoint);
				string returnData = Encoding.ASCII.GetString(receiveBytes);
				// parsing
				Debug.Log(returnData);
				Debug.Log(regexParse.Match(returnData).ToString());
				UDPValue = Int32.Parse(regexParse.Match(returnData).ToString());
			}
			catch (Exception e)
			{
				Debug.Log("Not so good " + e.ToString());
			}
			Thread.Sleep(20);
		}
	}
	
	void Update()
	{
		if (t_udp != null) Debug.Log(t_udp.IsAlive);
		Vector3 scale = transform.localScale;
		scale.x = (float)UDPValue;
	}
	
	void OnDisable()
	{

	}
}
*/



