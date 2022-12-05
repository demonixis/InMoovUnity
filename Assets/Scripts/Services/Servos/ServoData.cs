using System;

namespace Demonixis.InMoov.Servos
{
    public enum ServoMixageType
    {
        None = 0,
        SameValue,
        InverseValue,
    }

    [Serializable]
    public struct ServoData
    {
        public ServoIdentifier Id;
        public bool Inverse;
        public byte Min;
        public byte Neutral;
        public byte Max;
        public byte Speed;
        public byte PinId;
        public int CardId;
        public bool Enabled;
        public byte Value;
        public byte ScaleValueTo180;
        public ServoIdentifier MixedServo;
        public ServoMixageType MixageType;

        public override string ToString()
        {
            return $"{Id} - Min: {Min} / Max: {Max} / Neutral: {Neutral} / Invert: {Inverse} / Card: {CardId} / Pin: {PinId}";
        }

        public static ServoData New(ServoIdentifier servoId)
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
                Enabled = false,
                Value = 90,
                ScaleValueTo180 = 0,
                MixedServo = ServoIdentifier.None,
                MixageType =  ServoMixageType.None
            };
        }
    }

    [Serializable]
    public enum ServoIdentifier
    { // Head
        EyeX = 0,
        EyeY,
        Jaw,
        HeadYaw,
        HeadPitch,
        HeadRollPrimary,
        HeadRollSecondary,

        // Torso + Stomach
        PelvisYaw,
        PelvisPitch,
        PelvisRollPrimary,
        PelvisRollSecondary,

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
        
        // Misc
        None
    }
}