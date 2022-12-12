# Services

A service is a tiny program that handles a specific primary task, it's mandatory. You can get the list of enabled services using `Robot.Instance.Services`. A dedicated user interface allows to select the desired service. Note that when a service is changed, the entire scene is reloaded (it's temporary).
For instance, you can rewrite the servo service to use a Raspberry Pi card instead of an Arduino.

## Service Type
| Service | Description |
|---------|-------------|
| Voice | Makes the robot able to talk using speech synthesis | 
| Ears | Makes the robot able to understand human voice | 
| Chat | Chatbot which use voice and ears to discus with a human |
| Servo | Handle the communication between the computer and the robot cards (Arduino) | 
| ComputerVision | Allows the robot to see and understand the environment | 
| Navigation | Allows the robot to move |

## Service List

### Chatbot
| Service  | Description  												  |
|----------|--------------------------------------------------------------|
| AIML.Net | The A.L.I.C.E. Chat bot ported to C#. It uses aiml/xml files |

### Voice Recognition
| Service  | Description  												                        | Default | Platform |
|----------|--------------------------------------------------------------|---------|----------|
| Vosk     | A crossplatform voice recognition service. You've to install voice model into the `StreamingAssets\VoskModels` | Yes | Windows, Linux, Macos, Android |
| System.Speech | The integrated Speech Recognition feature in the Windows operating system. It's Windows **only** and you've to manage languages yourself from Windows | No | Windows / Microsoft Store |

### Speech Synthesis
| Service  | Description  												  | Default | Platform |
|----------|--------------------------------------------------------------|---------|----------|
| SAM | A C# port of the SAM speech synthetiser which is crossplatformer and enabled by default | Yes | Windows, Linux, Macos, Android |
| System.Speech  | The integrated Speech Synthesis feature integrated in the Windows operating system. It's Windows **only** and you've to manage languages yourself from Windows | No | Windows / Microsoft Store |
| VoiceRSS | VoiceRSS is an online service that requires an API key (put that key into the `global-config.json` file). It supports many languages. The free tier allows up to 350 query per day | No | Windows, Linux, Macos, Android |

### Servo Mixer
| Service  | Description  												  |
|----------|--------------------------------------------------------------|
| ServoMixer | The default servo mixer which uses the `SerialPort` class to communicate with Arduino Boards. It supports up to 6 boards (this number can be increased) |
