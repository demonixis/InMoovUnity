#include <Servo.h>

// You can change that for another board
const int PinStart = 3;
const int PinEnd = 13;
const int ServoCount = PinEnd - PinStart;

// The trame is
// Servo 0 (Pin2) => Value (0/180), Enabled
// byteArray[0] => Value
// byteArray[1] => Enabled
const int MessageSize = ServoCount * 2;

// Servos
Servo servos[ServoCount];
// Values
int values[ServoCount];
// Activation
int servoActivation[ServoCount];
int lastServoActivation[ServoCount];

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
  // Read data from the Unity App, see the header for the trame
  int dataCount = Serial.available();
  if (dataCount == MessageSize)
  {
    int i = 0;
    if (Serial.available() > 0)
    {
        values[i] = Serial.read();
        servoActivation[i] = Serial.read();
    }

    Serial.print("Data Received!");
  }

  // Apply values to the servos
  for (int i = 0; i < ServoCount; i++)
  {
    // Check if we need to enable or disable the servo
    if (servoActivation[i] != lastServoActivation[i])
    {
      if (servoActivation[i])
        servos[i].attach(i + PinStart);
      else
        servos[i].detach();

        lastServoActivation[i] = servoActivation[i];
        Serial.print("Servo Enabled Changed");
    }

    // Apply the value if enabled.
    if (servoActivation[i])
      servos[i].write(values[i]);
  }
}