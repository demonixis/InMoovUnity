# Servo Mapping
The naming convention of servo was changed to be closer to Unity or other realtime 3d engine.
All servos are suffixed with the type of degree of freedom. For instance, Yaw/Pitch/Roll.

## Mapping
Some servos are suffixed by Primary/Secondary. This is the case for some servos that work together.
In the current build of InMoov, you've to disassemble some servos to invert them. Thanks to the Servo Mixer panel, you can mix two servos together.
For instance you can create a mix between Head Roll Primary and Secondary with an opposite direction. It requires two pins on the board of course.

If you already tweaked your servos, you don't need to use the secondary servo. This is a feature for new builders that doesn't want to disassemble their servos (anyway, we've to do it for some bones, so..)

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
