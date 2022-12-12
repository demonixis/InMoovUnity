# InMoov Unity

InMoov Unity is an application to manage the [InMoov Robot](https://www.inmoov.fr). It's an alternative of My Robot Lab.

The idea is to use the power and tools of the Unity Engine to make the robot alive.
The robot is treated like a "standard NPC" (Non Playable Character). That way, animations, behaviours, etc. are converted and sent to the robot.

## Features
- Arduino support
- Servo Management / Mixer
- Chatbot (A.L.I.C.E.)
- Speech Recognition/Synthesis
- Jaw Mechanism plugged to the chatbot
- VR support to possess the robot using OpenXR
- Inverse Kinematics
- Webcam passthrough
- Random Animations
- Everything is overridable!

### About sensors
I've no plans to support the PIR and ultrasonic sensors for now. I think it's a better idea to focus the work on computer vision / spatial understanding.
However it's an easy task that can be made later if the community want it.

### About the Neo Pixel Ring
I don't have this ring so I can't add support for it for now.

### Roadmap
#### Short term
- Bugfix + Documentation for the initial release
- Mecanim to Servo (Play a FBX animation directly on the robot)
- Object tracking using Yolo 
- Spatial understanding using the RealSense camera

#### long term
- Unity Maching Learning Agent integration
- Brain Context
- Navigation
- Remote control over internet
- User Tracking
- OpenAI Chatbot GPT3

## Requirements
### Final User
- A computer with Windows, Linux or Macos
- Arduino IDE to upload the code on your boards
- [Optional] A text editor to tweak the configuration files if needed

### Developers & Contributors
- Unity Editor 2022.x (latest stable version)
- Visual Studio / Rider for C# editing
- [Optional] Python + Baracuda for Machine Learning development/training

## Platforms
This project can be tried on Windows, Linux, Mac and Android. Note that this is still in development with a focus on the Windows platform.
Standalone VR headsets will be the next target

## Disclamer
This project is in **early stage**, please try each servo separately, in best conditions. I'm not responsible for broken hardware. 
Note that every InMoov robot are not the same, here are the specifications of mine:
- Old Neck (1x servo in the head, another in the chest)
- Arduino Nano boards + custom PCB
- Intel RealSense instead of Kinect

That doesn't mean you can't use the new Neck because the servos are exposed, but some things can be missed.
I don't have the nervo board so I don't know if it'll work or not. If you don't use an Arduino nano, you'll have to change the `PinStart` and `PinEnd` values in `SerialPortManager`. I'll add more control on Arduino card later.

## Quick start
First, you've to install the Unity Editor 2022.2.x, it's probably working with lower and higher versions, but I only support latest 2022.x version.
Then flash your Arduino board with the code located into the Arduino folder.

### Settings
There are many settings stored into `My Documents/My Games/InMoov` in the `JSON` format. You can use a simple text editor to change what you want.

### Servo Mapping
#### Head
| InMoov Servo | InMoov Unity Servo  |
|--------------|---------------------|
| rothead	   | Head Yaw 			 |
| rollNeck 	   | Head Roll Primary	 |
| rollNeck 	   | Head Roll Secondary |
| neck		   | Head Pitch 		 |
| eyeX		   | Eye X				 |
| eyeY         | Eye Y				 |
| eyelidLeft   | Eyelid Left		 |
| eyelidRight  | Eyelid Right		 |
| jaw		   | Jaw				 |

#### Body
| InMoov Servo | InMoov Unity Servo     |
|--------------|------------------------|
| top Stom	   | Pelvis Roll Primary    |
| top Stom	   | Pelvis Roll Secondary  |
| mid Stom 	   | Pelvis Yaw Primary	    |
| mid Stom 	   | Pelvis Yaw Secondary   |
| low Stom 	   | Pelvis Pitch Primary   |
| low Stom 	   | Pelvis Pitch Secondary |

##### Arm
| InMoov Servo  | InMoov Unity Servo  |
|---------------|---------------------|
| left rotate   | Left Shoulder Yaw	  |
| left omoplate | Left Shoulder Roll  |
| left shoulder | Left Shoulder Pitch |
| left bicept   | Left Elbow		  | 
| left wrist    | Left Wrist		  |
| thumb   		| Left Finger Thumb	  |
| index   		| Left Finger Index	  |
| majeure		| Left Finger Middle  |
| ringFinger	| Left Finger Ring	  |
| pinky		    | Left Finger Pinky	  |

### Troubleshooting
The first thing to do is to remove the InMoov folder located in `My Documents/My Games/`. You can also open an issue on the github page.

## Architecture
There are two concepts in InMoov Unity, services and systems. You can write your own service or system and override existing ones.

### Robot Service
A service is a tiny program that handles a specific primary task, it's mandatory. You can get the list of enabled services using Robot. Instance. Services. A dedicated user interface allows you to select the service you want. Note that when a service is changed, the entire scene is reloaded (it's temporary).

#### List of services
| Service | Description | In Project |
|---------|-------------|------------|
| Voice | Makes the robot able to talk using speech synthesis | Microsoft Speech |
| Ears | Makes the robot able to understand human voice | Microsoft Speech |
| Chat | Chatbot which use voice and ears to discus with a human | AIML.Net (Alice bot) | 
| Servo | Handle the communication between the computer and the robot cards (Arduino) | SerialPortManager/ServoMixer |
| ComputerVision | Allows the robot to see and understand the environment | Webcam, Intel RealSense |
| Navigation | Allows the robot to move | Default Navigation System |

For instance, you can rewrite the servo service to use a Raspberry Pi card instead of an Arduino.

### Robot Systems
A system is a component that uses one or more services to make something. For instance the Jaw Mechanism uses both ServoMixer and Voice Recognition services to work. Here is a list of systems in the project:

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
Assets coming from the InMoov project are released under the Creative Commons license.

