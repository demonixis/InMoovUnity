#include <Servo.h>
#include <SoftwareSerial.h>

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
const int MaxBufferSize = 63;

// The trame is
// Servo 0 (Pin2) => Value [0; 180], Disable if value is upper to ServoMax
// byteArray[index] => Value [0; 180]

// Servos
Servo servos[ServoCount];
int values[ServoCount];
int servoActivation[ServoCount];
int lastServoActivation[ServoCount];

// Bluetooth
const byte rxBT = 2;
const byte txBT = 3;
SoftwareSerial BTSerial(rxBT, txBT);

// Prototypes
void CheckSerialData();
void CheckBluetoothData();
void ApplyServoValues();

void setup() {
  Serial.begin(DefaultBaudRate);
  BTSerial.begin(DefaultBaudRate);

  for (int i = 0; i < ServoCount; i++) {
    values[i] = ServoNeutral;  // Neutral
    servoActivation[i] = 0;
    lastServoActivation[i] = 0;
  }
}

void loop() {
  // Handle USB connection (PC)
  if (Serial.available()) {
    CheckSerialData();
    ApplyServoValues();
  }

  // Handle Bluetooth connection (Everything)
  if (BTSerial.available()) {
    CheckBluetoothSerial();
    ApplyServoValues();
  }
}

void CheckSerialData() {
  // Read data from the Unity App, see the header for the trame
  int dataCount = Serial.available();

  if (dataCount > 0) {
    Serial.println(dataCount);
  }

  if (dataCount != ServoCount) {
    // Flush the buffer if full.
    if (dataCount == MaxBufferSize) {
      while (Serial.available()) {
        Serial.read();
      }
    }

    // Return while it doesn't have the expected size.
    return;
  }

  int i = 0;
  while (Serial.available() > 0) {
    values[i] = Serial.read();
    servoActivation[i] = values[i] <= ServoMax ? 1 : 0;
    i++;
  }
}

void CheckBluetoothSerial() {
  // Read data from the Unity App, see the header for the trame
  int dataCount = BTSerial.available();

  if (dataCount > 0) {
    BTSerial.println(dataCount);
  }

  if (dataCount != ServoCount) {
    // Flush the buffer if full.
    if (dataCount == MaxBufferSize) {
      while (BTSerial.available()) {
        BTSerial.read();
      }
    }

    // Return while it doesn't have the expected size.
    return;
  }

  int i = 0;
  while (BTSerial.available() > 0) {
    values[i] = BTSerial.read();
    servoActivation[i] = values[i] <= ServoMax ? 1 : 0;
    i++;
  }
}

void ApplyServoValues() {
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
