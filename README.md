# InMoov Unity

InMoov Unity is a robotics framework dedicated to the [InMoov Robot](https://www.inmoov.fr). It's a fully replacement of My Robot Lab.

The idea behind this project is to use the power and tools of the Unity Engine to makes the robot alive.
The robot is treated like a "standard NPC" (Non Playable Character). That way, animations, behaviours, etc. are converted and sent to the robot.

## Features
- Robotics framework
- Speech Recognition/Synthesis
- Chatbot
- Spatial understanding
- VR support to possess the robot using OpenXR

### Roadmap
- Unity Maching Learning Agent integration
- Yolo 
- Brain Context
- Navigation
- Remote control over internet
- User tracking
- Inverse Kinematics

## Platforms
This project can be tried on Windows, Linux, Mac and Android. Note that this is still in development with a focus on the Windows plateform.
Standalone VR headset will be the next target

## Disclamer
This project is in **early stage**, please try each servo separatly, in best conditions. I'm not responsible of broken hardware. 
Note that every InMoov robot are not the same, here are the specification sof mine:
- Old Neck (1x servo in the head, another in the chest)
- Arduino Nano boards + custom PCB
- Intel RealSense instead of Kinect

That doesn't mean you can't use the new Neck because servos are exposed, but some things can be missing.
I don't have the nervo board so I don't know if it'll work or not. If you don't use an arduino nano, you'll have to change the PinStart/PinEnd values in SerialPortManager. I'll add more control on Arduino card later.

## Quick start
First you've to install the Unity Editor 2022.2.x, it's probably working with lower and higher versions, but I only support latest 2022.x version.

Then flash your Arduino board with the code located into the Arduino folder. If you don't use an Arduino Nano, change the PinStart/End in both the Arduino source file and in the Unity project (SerialPortManager).
Run the Unity Editor, select the MainScene and hit the play button. You can now start the setup of your Arduino Cards.

### Settings
There are many settings stored into My Documents/My Games/InMoov in the JSON format. You can use a simple text editor to change what you want.

### Troubleshooting
The first thing to do is to remove the InMoov folder located in My Documents/My Games/. You can also open an issue on the github page.

## Architecture
There are two concepts in InMoov Unity, services and systems. You can write your own service or system and override existing ones.

### Robot Service
A service is a tiny program that handle a specific primary task, it's mandatory. You can get the list of enabled services using Robot.Instance.Services. A dedicated user interface allows you to select the service you want. Note that when a service is changed, the entire scene is reloaded (it's temporary).

#### List of services
| Service | Description | In Project |
|---------|-------------|------------|
| Voice | Makes the robot able to talk using speech synthesis | Microsoft Speech |
| Ears | Makes the robot able to understand human voice | Microsoft Speech |
| Chat | Chatbot which use voice and ears to discus with a human | AIML.Net (Alice bot) | 
| Servo | Handle the communication between the computer and the robot cards (Arduino) | SerialPortManager/ServoMixer |
| ComputerVision | Allows the robot to see and understand the environment | Webcam, Intel RealSense |
| Navigation | Allows the robot to move | Default Navigation System |

For instance you can rewrite the servo service to use a Raspberry Pi card instead of an Arduino.

### Robot Systems
A system is a component that use one or more services to make something. For instance the Jaw Mechanism uses both ServoMixer and Voice Recognition services to work. Here is a list of systems in the project:

| System | Description |
|--------|-------------|
| Jaw Mechanism | Makes the jaw moves when the robot is talking |
| MecanimTranslator | Allows to play an Unity animation on the robot |
| RandomAnimator | Makes the robot move randomly |
| VRPosses | Allows the user to possess the robot using a VR headset |

More systems will come later.

## License
This project is released under the MIT License. Please read the `license.md` file for more information about it.
The AIML.Net project is released under the GNU GPL license.
Assets from the InMoov project are released under the Creative Common license.

