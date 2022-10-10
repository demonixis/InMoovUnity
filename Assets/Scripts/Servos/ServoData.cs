using System;

namespace Demonixis.InMoov.Servos
{
    [Serializable]
    public struct ServoData
    {
        public bool Inverse;
        public int Min;
        public int Neutral;
        public int Max;
        public float Speed;
        public int PinId;
        public int CardId;
    }

    [Serializable]
    public enum ServoIdentifier
    {
        EyeX = 0,
        EyeY,
        HeadYaw,
        HeadPitch,
        HeadRoll,
        HeadRoll2,
        
        // WIP
        LeftHandTwist,
        LeftFingerThumb,
        LeftFingerIndex,
        LeftFingerMiddle,
        LeftFingerRing,
        LeftFingerPinky,
        
        // WIP
        RightHandTwist,
        RightFingerThumb,
        RightFingerIndex,
        RightFingerMiddle,
        RightFingerRing,
        RightFingerPinky,
    }
}