#define double_buffer

#include <Ticker.h>
#include <PxMatrix.h>
#include <ESP8266WiFi.h>
#include <WiFiUdp.h>

Ticker display_ticker;

// Pins for LED MATRIX
#define P_LAT 16
#define P_A 5
#define P_B 4
#define P_C 15
#define P_OE 2
#define P_D 12
#define P_E 0

 PxMATRIX display(64,32,P_LAT, P_OE,P_A,P_B,P_C,P_D);

// Some standard colors
uint16_t myRED = display.color565(255, 0, 0);
uint16_t myGREEN = display.color565(0, 255, 0);
uint16_t myBLUE = display.color565(0, 0, 255);
uint16_t myWHITE = display.color565(150, 150, 150);
uint16_t myYELLOW = display.color565(255, 255, 0);
uint16_t myCYAN = display.color565(0, 255, 255);
uint16_t myMAGENTA = display.color565(255, 0, 255);
uint16_t myBLACK = display.color565(0, 0, 0);

uint16 myCOLORS[8] = {myRED, myGREEN, myBLUE, myWHITE, myYELLOW, myCYAN, myMAGENTA, myBLACK};
int position = 0;
bool isColorInfo = false;
int colorIndex = 0;
byte levels[64];
int r = 0;
int g = 150;
int b = 30;
bool dynamicColorChannels[3];
bool isDotMode = false;

// WIfi server
int port = 3333; 
WiFiUDP udp;
char packetBuffer[255];
const char *ssid = "Telekom-cc4591-2.4GHz";  //Enter your wifi SSID
const char *password = "";  //Enter your wifi Password

// ISR for display refresh
void display_updater()
{
  display.display(20);
}

void setup() {
  display.begin(16);
  display_ticker.attach(0.002, display_updater);
  yield();

  printSomething("Init serial...");
  Serial.begin(74880);
  while (!Serial) { ; }
  Serial.println(" Kapcsolodva " );
  printSomething("Serial initialized. Connect Wifi!");

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password); 
  WiFi.setSleep(false);

  delay(5000);
  // Wait for connection  
  Serial.println("Connecting to Wifi");
  if (WiFi.status() == WL_CONNECTED) {   
    printSomething(WiFi.localIP().toString().c_str());
    if (udp.begin(port) == 1)
      printSomething(strcat ("UDP: ", WiFi.localIP().toString().c_str()));
  } else {
    printSomething("Wifi not available!");
  }  
}

void loop() {
  while (Serial.available()) {
    //delay(2);  //delay to allow byte to arrive in input buffer
    int v = Serial.read();
    processByte(v);
  }

  delay(1);
  while (udp.parsePacket()) {
    while (true) {
      int b = udp.read();
      if (b == -1)
        break;
      processByte(b);
    }    
    delay(1);   
  }
}

void processByte(int v) {
  char c = v;
  if (c == '.' && !isColorInfo) {
    isDotMode = true;
  } else if (c == '_' && !isColorInfo) {  
    position = 0;
  } else if (c == ';' && !isColorInfo) {
    isColorInfo = true;
  } else if (isColorInfo && colorIndex == 0) {
    r = v;
    colorIndex++;
  } else if (isColorInfo && colorIndex == 1) {
    g = v;
    colorIndex++;
  } else if (isColorInfo && colorIndex == 2) {
    b = v;
    colorIndex++;
  } else if (isColorInfo && colorIndex == 3) {
    dynamicColorChannels[0] = (v & 4) == 4;
    dynamicColorChannels[1] = (v & 2) == 2;
    dynamicColorChannels[2] = (v & 1) == 1;
    colorIndex++;
  } else if (c == ';' && isColorInfo) {
    isColorInfo = false;
    colorIndex = 0;
  } else {
    byte height = (c >= 'a' && c <= 'z' ? c - 'a' : c - 'A' + 26);
    if (height >= 0 && height < 33)
      levels[position++] = height;            
  }

  if (position >= 64){     
    display2(); 
    isDotMode = false;
    position = 0;   
  }
}

void display2() {
  display.clearDisplay();
  int position2 = display.width() - 1;
  uint16_t col = display.color565(r, g, b);

  for(int i = 0; i < 64; i++) {
    if (dynamicColorChannels[0])
      col = display.color565(levels[i] * 6, g, b);

    if (dynamicColorChannels[1])
      col = display.color565(r, levels[i] * 6, b);

    if (dynamicColorChannels[2])
      col = display.color565(r, g, levels[i] * 6);

    if (isDotMode)
      display.drawPixel(position2, levels[i], col);
    else 
      display.drawLine(position2, 0, position2, levels[i],  col);

    position2--;
  }  

  display.showBuffer();
}

void printSomething(const char* str) {
  
  display.fillScreen(myBLACK);
  display.flushDisplay();
  display.setTextColor(myRED);
  display.setCursor(0,0);
  display.print(str);
  display.showBuffer();
}