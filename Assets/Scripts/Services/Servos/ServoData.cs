﻿using System;

namespace Demonixis.InMoov.Servos
{
    [Serializable]
    public struct ServoData
    {
        public string Id;
        public bool Inverse;
        public byte Min;
        public byte Neutral;
        public byte Max;
        public byte Speed;
        public byte PinId;
        public int CardId;
        public byte Enabled;
        public byte Value;
        public byte ScaleValueTo180;

        public static ServoData New(string servoId)
        {
            return new ServoData
            {
                Id = servoId,
                Inverse = false,
                Min = 0,
                Max = 180,
                Neutral = 90,
                Speed = 1,
                PinId = SerialPortManager.PinStart,
                CardId = (int)ArduinoIdentifiers.None,
                Enabled = 0,
                Value = 90,
                ScaleValueTo180 = 0
            };
        }
    }

    [Serializable]
    public enum ServoIdentifier
    {
        // Head
        EyeX = 0,
        EyeY,
        Jaw,
        HeadYaw,
        HeadPitch,
        HeadRoll,

        // Torso + Stomach
        PelvisYaw,
        PelvisPitch,
        PelvisRoll,

        // Left Shoulder + Arm
        LeftShoulderYaw,
        LeftShoulderPitch,
        LeftShoulderRoll,
        LeftArm,
        LeftWrist,
        
        // Left Hand
        LeftFingerThumb,
        LeftFingerIndex,
        LeftFingerMiddle,
        LeftFingerRing,
        LeftFingerPinky,
        
        // Right Shoulder + Arm
        RightShoulderYaw,
        RightShoulderPitch,
        RightShoulderRoll,
        RightArm,
        RightWrist,

        // Right Hand
        RightFingerThumb,
        RightFingerIndex,
        RightFingerMiddle,
        RightFingerRing,
        RightFingerPinky,
    }
}