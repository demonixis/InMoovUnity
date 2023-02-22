using System;
using static UnityEditor.PlayerSettings;

namespace Demonixis.InMoov
{
    public static class ServoConverter
    {
        public static byte UnityRotationToServo(float rotation, bool scaleTo180)
        {
            rotation %= 360;
            rotation += 90; // Center is 0 on Unity but 90 in Arduino

            if (rotation > 180)
                rotation = 360 - rotation;

            return (byte) rotation;
        }

        public static float MapF(float value, float min, float max)
        {
            var oldMin = 0;
            var oldMax = 180;
            var oldRange = oldMax - oldMin;
            var newRange = max - min;

            return (((value - oldMin) * newRange) / oldRange) + min; 
        }

        public static byte Map(byte value, byte min, byte max)
        {
            return (byte)MapF(value, min, max);
        }
    }
}