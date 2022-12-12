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

You can read the [documentation here](Documentations/README.md) and see what is available, what is not, the roadmap, etc.

## Requirements
### Final User
- A computer with Windows, Linux or Macos
- Arduino IDE to upload the code on your boards
- [Optional] A text editor to tweak the configuration files if needed

### Developers & Contributors
- Unity Editor 2022.x (latest stable version)
- Visual Studio / Rider for C# editing
- [Optional] Python + Baracuda for Machine Learning development/training

## Disclamer
This project is in **early stage**, please try each servo separately, in best conditions. I'm not responsible for broken hardware. 
Note that every InMoov robot are not the same, here are the specifications of mine:
- Arduino Nano on a custom PCB and a Mega board with the Servo Shield (The Nervoboard will probably work too)
- Intel RealSense instead of Kinect

## Quick start
First, you've to install the Unity Editor 2022.2.x, it's probably working with lower and higher versions, but I only support latest 2022.x version.
Then flash your Arduino board with the code located into the Arduino folder.

### Platforms
This project can be tried on Windows, Linux, Mac and Android. Note that this is still in development with a focus on the Windows platform.
Standalone VR headsets will be the next target

### Settings
There are many settings stored into `My Documents/My Games/InMoov` in the `JSON` format. You can use a simple text editor to change what you want.

### Troubleshooting
The first thing to do is to remove the InMoov folder located in `My Documents/My Games/`. You can also open an issue on the github page.

## License
This project is released under the MIT License. Please read the `license.md` file for more information about it.
The AIML.Net project is released under the GNU GPL license.
Assets coming from the InMoov project are released under the Creative Commons license.

