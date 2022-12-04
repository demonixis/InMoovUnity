namespace Demonixis.InMoov
{
    public static class ServoConverter
    {
        public static byte UnityRotationToServo(float rotation, bool scaleTo180)
        {
            rotation += 90; // Center is 0 on Unity but 90 in Arduino
            rotation %= 360;

            if (rotation > 180)
                rotation = 360 - rotation;

            return (byte)rotation;
        }
    }
}