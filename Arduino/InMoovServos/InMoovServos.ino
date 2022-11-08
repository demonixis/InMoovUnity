#include <Servo.h>

const int PinStart = 3;
const int PinEnd = 13;
const int ServoCount = PinEnd - PinStart;
Servo servos[ServoCount];
int values[ServoCount];

void setup() 
{
  for (int i = 0; i < ServoCount; i++)
  {
    servos[i].attach(i + PinStart);
    values[i] = 90; // Neutral
  }
}

void loop() 
{
  // TODO: Serial Read
  
  for (int i = 0; i < ServoCount; i++)
  {
    servos[i].write(values[i]);
  }
}
