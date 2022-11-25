#include <Servo.h>

// You can change that for another board
const int PinStart = 2;
const int PinEnd = 13;
const int ServoCount = PinEnd - PinStart;
const int ServoMin = 0;
const int ServoNeutral = 90;
const int ServoMax = 180;
const int DefaultBaudRate = 115200;

// The trame is
// Servo 0 (Pin2) => Value [0; 180], Enabled [0; 1]
// byteArray[index] => Value [0; 180]
// byteArray[index + 1] => Enabled Enabled [0; 1]
const int MessageSize = ServoCount * 2;

// Servos
Servo servos[ServoCount];
// Values
int values[ServoCount];
// Activation
int servoActivation[ServoCount];
int lastServoActivation[ServoCount];

void setup() {
  Serial.begin(DefaultBaudRate);

  for (int i = 0; i < ServoCount; i++) {
    //servos[i].attach(i + PinStart);
    values[i] = ServoNeutral;  // Neutral
    servoActivation[i] = 0;
    lastServoActivation[i] = 0;
  }
}

void loop() {
  // Read data from the Unity App, see the header for the trame
  int dataCount = Serial.available();

  if (dataCount == MessageSize) {
    int i = 0;
    while (Serial.available() > 0) {
      values[i] = Serial.read();
      servoActivation[i] = Serial.read();
      i++;
    }

    // Apply values to the servos
    for (int i = 0; i < ServoCount; i++) {
      // Check if we need to enable or disable the servo
      if (servoActivation[i] != lastServoActivation[i]) {
        if (servoActivation[i] > 0) {
          servos[i].attach(i + PinStart);
        } else {
          servos[i].detach();
        }
        lastServoActivation[i] = servoActivation[i];
      }

      // Apply the value if enabled.
      servos[i].write(values[i]);
    }
  }
}
