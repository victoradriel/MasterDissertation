/** author: Victor Adriel 
	Primeiro teste: reconhecimento de padrões.
	Depois de explicados os significados dos parâmetros acompanhando o estímulo correspondente
	são impressos padrões e o usuário é inquirido a respeito de:
	- Quantos vibradores estão ativos;
	- Quais vibradores foram ativados (posição);
	- Qual informação transmitida (tacton).	
*/

int vibDur = 1000;
int peqIntrvl = 2000;
int grdIntrvl = 6000;
int qtdTactors = 9;

void setup(){
  for(int x=0; x<qtdTactors; x++){
    pinMode(x+5, OUTPUT);
  }
}

void loop(){
  teste();
  delay(grdIntrvl*10);
}

void tactorOn(int x){
  digitalWrite(x, HIGH);
}

void tactorOff(int x){
  digitalWrite(x, LOW);
}

void patTrainn1(){
  tactorOn(8);
  delay(vibDur);
  tactorOff(8);
}
void patTrainn2(){
  tactorOn(9);
  delay(vibDur);
  tactorOff(9);
}
void patTrainn3(){
  tactorOn(6);
  tactorOn(9);
  delay(vibDur);
  tactorOff(6);
  tactorOff(9);
}
void patTrainn4(){
  tactorOn(6);
  delay(vibDur);
  tactorOff(6);
}
void patTrainn5(){
  tactorOn(7);
  tactorOn(13);
  delay(vibDur);
  tactorOff(7);
  tactorOff(13);
}
void patTrainn6(){
  tactorOn(11);
  tactorOn(12);
  tactorOn(13);
  delay(vibDur);
  tactorOff(11);
  tactorOff(12);
  tactorOff(13);
}
void patTrainn7(){
  tactorOn(9);
  tactorOn(10);
  delay(vibDur);
  tactorOff(9);
  tactorOff(10);
}
void patTrainn8(){
  tactorOn(9);
  tactorOn(12);
  delay(vibDur);
  tactorOff(9);
  tactorOff(12);
}
void patTrainn9(){
  tactorOn(7);
  tactorOn(10);
  tactorOn(13);
  delay(vibDur);
  tactorOff(7);
  tactorOff(10);
  tactorOff(13);
}

void patTest3(){
  tactorOn(7);
  delay(vibDur);
  tactorOff(7);
}
void patTest4(){
  tactorOn(7);
  tactorOn(9);
  delay(vibDur);
  tactorOff(7);
  tactorOff(9);
}
void patTest5(){
  tactorOn(7);
  tactorOn(11);
  delay(vibDur);
  tactorOff(7);
  tactorOff(11);
}
void patTest6(){
  tactorOn(9);
  tactorOn(13);
  delay(vibDur);
  tactorOff(9);
  tactorOff(13);
}
void patTest7(){
  tactorOn(7);
  tactorOn(11);
  tactorOn(13);
  delay(vibDur);
  tactorOff(7);
  tactorOff(11);
  tactorOff(13);
}
void patTest8(){
  tactorOn(7);
  tactorOn(8);
  tactorOn(10);
  tactorOn(13);
  delay(vibDur);
  tactorOff(7);
  tactorOff(8);
  tactorOff(10);
  tactorOff(13);
}

void teste(){
	// Teste
	delay(grdIntrvl);

	patTest3();
	delay(peqIntrvl);
	patTest3();
	delay(peqIntrvl);
	patTest3();

	delay(grdIntrvl);

	patTest4();
	delay(peqIntrvl);
	patTest4();
	delay(peqIntrvl);
	patTest4();

	delay(grdIntrvl);

	patTest5();
	delay(peqIntrvl);
	patTest5();
	delay(peqIntrvl);
	patTest5();

	delay(grdIntrvl);

	patTest6();
	delay(peqIntrvl);
	patTest6();
	delay(peqIntrvl);
	patTest6();

	delay(grdIntrvl);

	patTest7();
	delay(peqIntrvl);
	patTest7();
	delay(peqIntrvl);
	patTest7();

	delay(grdIntrvl);

	patTest8();
	delay(peqIntrvl);
	patTest8();
	delay(peqIntrvl);
	patTest8();

	delay(grdIntrvl);
	
	patTrainn1();
	delay(peqIntrvl);
	patTrainn1();
	delay(peqIntrvl);
	patTrainn1();
	
	delay(grdIntrvl);
	
    patTrainn2();
	delay(peqIntrvl);
    patTrainn2();
	delay(peqIntrvl);
    patTrainn2();
	
	delay(grdIntrvl);
	
    patTrainn3();
	delay(peqIntrvl);
    patTrainn3();
	delay(peqIntrvl);
    patTrainn3();
	
	delay(grdIntrvl);
	
    patTrainn4();
	delay(peqIntrvl);
    patTrainn4();
	delay(peqIntrvl);
    patTrainn4();
	
	delay(grdIntrvl);
	
    patTrainn5();
	delay(peqIntrvl);
    patTrainn5();
	delay(peqIntrvl);
    patTrainn5();
	
	delay(grdIntrvl);
	
    patTrainn6();
	delay(peqIntrvl);
    patTrainn6();
	delay(peqIntrvl);
    patTrainn6();
	
	delay(grdIntrvl);
	
    patTrainn7();
	delay(peqIntrvl);
    patTrainn7();
	delay(peqIntrvl);
    patTrainn7();
	
	delay(grdIntrvl);
	
    patTrainn8();
	delay(peqIntrvl);
    patTrainn8();
	delay(peqIntrvl);
    patTrainn8();
	
	delay(grdIntrvl);
	
	patTrainn9();
	delay(peqIntrvl);
    patTrainn9();
	delay(peqIntrvl);
    patTrainn9();
	
	delay(grdIntrvl);
}
