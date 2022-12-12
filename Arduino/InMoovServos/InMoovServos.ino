#include <Servo.h>

const int PinStart = 2;
const int PinEnd = 53; 
#if defined(ARDUINO_AVR_MEGA2560)
const int BoardPinEnd = PinEnd;
#else
const int BoardPinEnd = 13;
#endif
const int BufferLength = BoardPinEnd - PinStart;
const int ServoCount = PinEnd - PinStart;
const int ServoMin = 0;
const int ServoNeutral = 90;
const int ServoMax = 180;
const int DefaultBaudRate = 9600;

// The trame is
// Servo 0 (Pin2) => Value [0; 180], Disable if value is upper to ServoMax
// byteArray[index] => Value [0; 180]

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
    values[i] = ServoNeutral;  // Neutral
    servoActivation[i] = 0;
    lastServoActivation[i] = 0;
  }
}

void loop() {
  // Read data from the Unity App, see the header for the trame
  int dataCount = Serial.available();

  if (dataCount > 0) {
    Serial.print(dataCount);
    Serial.println();
  }

  if (dataCount != ServoCount) {
    return;
  }

  int i = 0;
  while (Serial.available() > 0) {
    values[i] = Serial.read();
    servoActivation[i] = values[i] <= ServoMax ? 1 : 0;
    i++;
  }

  // Apply values to the servos
  for (int i = 0; i < BufferLength; i++) {
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
