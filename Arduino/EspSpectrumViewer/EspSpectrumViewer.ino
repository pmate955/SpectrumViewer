#define double_buffer

#include <Ticker.h>
#include <PxMatrix.h>
#include <ESP8266WiFi.h>

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

// FFT data
int position = 0;
bool isColorInfo = false;
int colorIndex = 0;
byte levels[64];
int r = 0;
int g = 150;
int b = 30;
bool dynamicColorChannels[3];
bool isDotMode = false;

// Screen saver
bool isScreenSaver = false;
unsigned long lastCommunication = millis();

struct Snowflake {
  int8_t Position = -1;      
  uint8_t StepCycle = 1;     
  uint8_t ActualStep = 0;    
  uint8_t Red = 100;    
  uint8_t Green = 100;    
  uint8_t Blue = 100;    
};

Snowflake snowflakes[64];
int actualSnowflakes = 0;
int maxSnowflakes = 18;

// Wifi socket
WiFiServer wifiServer(3333);
char packetBuffer[255];
const char *ssid = "Telekom-cc4591-2.4GHz";  //Enter your wifi SSID
const char *password = "";  //Enter your wifi Password

// ISR for display refresh
void display_updater()
{
  display.display(20);
}

// Setup with WiFi, display and Serial initialization
void setup() {
  display.begin(16);
  display_ticker.attach(0.002, display_updater);
  display.setFastUpdate(true);
  yield();

  printSomething("Init serial...");
  Serial.begin(74880);
  while (!Serial) { ; }
  Serial.println(" Connected " );
  printSomething("Serial initialized. Connect Wifi!");

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password); 
  WiFi.setSleep(false);

  delay(5000);
  // Wait for connection  
  Serial.println("Connecting to Wifi");
  int maxTry = 5;
  int actual = 0;
  while (actual++ < maxTry) {
  if (WiFi.status() == WL_CONNECTED) {   
      wifiServer.begin();
      printSomething(WiFi.localIP().toString().c_str());
      break;
  } else {
      printSomething("Wifi not available! Retry!");
    }  
    delay(5000);
  }  
}

// Main loop with serial and socket processing
void loop() {
  while (Serial.available()) {
    //delay(2);  //delay to allow byte to arrive in input buffer
    int v = Serial.read();
    processByte(v);
  }

  WiFiClient client = wifiServer.available();
 
  if (client) { 
    while (client.connected()) { 
      while (client.available() > 0) {
        byte c = client.read();
        processByte(c);
      }
      screenSaver();
    }    
    client.stop(); 
  } else {
    screenSaver();
  }

}

// State machine to process message byte
void processByte(int v) {
  isScreenSaver = false;
  lastCommunication = millis();
  char c = v;
  if (c == '.' && !isColorInfo) {               // The first byte is the dot mode
    isDotMode = true;
  } else if (c == '_' && !isColorInfo) {        // _ character is the start 
    position = 0;
  } else if (c == ';' && !isColorInfo) {        // ; character contains color info
    isColorInfo = true;
  } else if (isColorInfo && colorIndex == 0) {  // If color info, the first it the red valure
    r = v;
    colorIndex++;
  } else if (isColorInfo && colorIndex == 1) {  // Second is the green
    g = v;
    colorIndex++;
  } else if (isColorInfo && colorIndex == 2) {  // Third is blue
    b = v;
    colorIndex++;
  } else if (isColorInfo && colorIndex == 3) {  // The 4. byte contains which channels can change by level
    dynamicColorChannels[0] = (v & 4) == 4;
    dynamicColorChannels[1] = (v & 2) == 2;
    dynamicColorChannels[2] = (v & 1) == 1;
    colorIndex++;
  } else if (c == ';' && isColorInfo) {         // ; character ends the color info too
    isColorInfo = false;
    colorIndex = 0;
  } else {                                      // By default the a-z byte or over A is the level 
    byte height = (c >= 'a' && c <= 'z' ? c - 'a' : c - 'A' + 26);
    if (height >= 0 && height < 33)
      levels[position++] = height;            
  }

  if (position >= 64){                          // After we got all values, update the display
    drawSpectrum(); 
    isDotMode = false;
    position = 0;   
  }
}

void drawSpectrum() {
  display.clearDisplay();
  int position2 = 0;
  uint16_t columnColor = display.color565(r, g, b);

  for(int i = 0; i < 64; i++) {
    if (dynamicColorChannels[0])
      columnColor = display.color565(levels[i] * 6, g, b);
    else if (dynamicColorChannels[1])
      columnColor = display.color565(r, levels[i] * 6, b);
    else if (dynamicColorChannels[2])
      columnColor = display.color565(r, g, levels[i] * 6);

    if (isDotMode)
      display.drawPixel(i, 31 - levels[i], columnColor);
    else 
      display.drawLine(i, 31, i, 31 - levels[i],  columnColor);
  }  

  display.showBuffer();
}

void printSomething(const char* str) {
  
  display.fillScreen(myBLACK);
  display.flushDisplay();
  display.setTextColor(myWHITE);
  display.setCursor(0,0);
  display.print(str);
  display.showBuffer();
}