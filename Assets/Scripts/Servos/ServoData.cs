using System;

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
        public float Speed;
        public int PinId;
        public int CardId;
        public byte Enabled;

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
                PinId = -1,
                CardId = -1,
                Enabled = 0
            };
        }
    }

    [Serializable]
    public enum ServoIdentifier
    {
        // Head
        EyeX = 0,
        EyeY,
        HeadYaw,
        HeadPitch,
        HeadRoll,

        // Torso + Stomach
        PelvisYaw,
        PelvisPitch,
        PelvisRoll,
        
        // Left Shoulder
        LeftShoulderYaw,
        LeftShoulderPitch,
        LeftShoulderRoll,
        
        // Right Arm

        // Left forearm
        LeftHandTwist,
        
        // Left Hand
        LeftFingerThumb,
        LeftFingerIndex,
        LeftFingerMiddle,
        LeftFingerRing,
        LeftFingerPinky,
        
        // Right Shoulder
        RightShoulderYaw,
        RightShoulderPitch,
        RightShoulderRoll,

        // Right Arm
        
        // Right forearm
        RightHandTwist,

        // Right Hand
        RightFingerThumb,
        RightFingerIndex,
        RightFingerMiddle,
        RightFingerRing,
        RightFingerPinky,
    }
}