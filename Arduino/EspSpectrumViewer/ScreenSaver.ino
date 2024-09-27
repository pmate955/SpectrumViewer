void generateSnowflake() {
  if (actualSnowflakes < maxSnowflakes) {
    int randomColumn = random(0, 64);
    if (snowflakes[randomColumn].Position == -1) {
      int stepCycle = random(2, 10);
      snowflakes[randomColumn].Position = 32;
      snowflakes[randomColumn].StepCycle = stepCycle;
      snowflakes[randomColumn].ActualStep = 0;
      float divider = 1 - (stepCycle / 10.0);
      snowflakes[randomColumn].Red = random(50, 250) * divider;
      snowflakes[randomColumn].Green = random(50, 250) * divider;
      snowflakes[randomColumn].Blue = random(50, 250) * divider;
      actualSnowflakes++;
    }
  }
}

void stepSnowflakes() {
  for (int i = 0; i < 64; i++) {
    if (snowflakes[i].Position > 0) {
      if (snowflakes[i].ActualStep == snowflakes[i].StepCycle) {
        snowflakes[i].Position--;
        snowflakes[i].ActualStep = 0;
      } else {
        snowflakes[i].ActualStep++;
      }
    } else if (snowflakes[i].Position == 0) {
      if (snowflakes[i].ActualStep == snowflakes[i].StepCycle) {
        snowflakes[i].Position = -1;
        actualSnowflakes--;      
      } else {
        snowflakes[i].ActualStep++;
      }
    }
  }
}

void displaySnowflakes() {

  display.clearDisplay();
  
  for(int i = 0; i < 64; i++) {    
    int position = snowflakes[i].Position;
    
    if (position >= 0) {
      uint16_t columnColor = display.color565(snowflakes[i].Red, snowflakes[i].Green, snowflakes[i].Blue);
      display.drawPixel(i, 31 - position, columnColor);
    }      
  }  

  display.showBuffer();
}

void screenSaver() {
  unsigned long actualMillis = millis();
  if (actualMillis > lastCommunication + 20000) {
    isScreenSaver = true;
  }

  if (isScreenSaver) {
    stepSnowflakes();
    generateSnowflake();
    displaySnowflakes();
    delay(8);
  }
}