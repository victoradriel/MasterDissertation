
// variabels
char serInString[40];  // the received string with output information max 40 characters
int stringLength = 1;  // expected length of string received from Flash
int serInStringLength; // length of string received by Flash.
int digOut = 0;        // Number of digital outputs
int digOutputs[12];    // array that stores which pins are used as digital outputs max 12.

void setup() {
 //Serial.begin(9600);   // start serial port
 Serial.begin(57600);    // start serial port

 for (int i =0; i < 8; i++) {
  pinMode(i+3, OUTPUT);
  digOutputs[digOut] = i+3;
  stringLength +=2; // output string part will have 2 characters
  digOut++;
 }
}

// Function to read a string from the serialport and store it in an array
void readSerialString (char *strArray){
 int i = 0;
 if(Serial.available()){
  while (Serial.available()) {
    strArray[i] = Serial.read();
    i++;
  }
 }
 serInStringLength=i;
}

// Function to find out wether the array is empty or not
boolean isStringEmpty(char *strArray){
  if (strArray[0] == 0)   return true;
  else                    return false;
}

void updateArduinoOutputs(){
  int serInStringPosition = 0;
  
  for (int i=0; i < digOut; i++) {
    if(serInString[serInStringPosition]=='1') digitalWrite(digOutputs[i], HIGH);
    if(serInString[serInStringPosition]=='2') digitalWrite(digOutputs[i], HIGH);
    serInStringPosition += 2;
  }
  
  // Frequencia menor  - 60 ml
  delay(60);
  serInStringPosition = 0;
  for (int i=0; i < digOut; i++) {
    if(serInString[serInStringPosition]=='2') digitalWrite(digOutputs[i], LOW);
    serInStringPosition += 2;
  }
  
  // Frequencia maior - 100 ml
  delay(40);
  serInStringPosition = 0;
  for (int i=0; i < digOut; i++) {
    if(serInString[serInStringPosition]=='1') digitalWrite(digOutputs[i], LOW);
    serInStringPosition += 2;
  }
}

void eraseStrings(){ // erase contents in the serInString array
  for (int i=0; i < 40; i++) {
    serInString[i]=0;
  }
}

void loop () {
  // read the serial port and create a string out of what you read
  readSerialString(serInString);

  // check if there data is received if isStringEmpty is true do nothing.
  if(isStringEmpty(serInString) == false) {
    updateArduinoOutputs();
    //eraseStrings();
  }
  // short delay before making sure serial.read is not called to soon again
  delay(20);
}


