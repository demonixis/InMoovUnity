namespace Demonixis.InMoov
{
    public static class ServoConverter
    {
        public static byte UnityRotationToServo(float rotation, bool scaleTo180)
        {
            // [-360; 360]
            rotation = rotation % 360.0f;

            // Reverse to go from 0 to 90
            if (rotation > 180)
                rotation *= -1.0f;

            // Fix limits..
            if (rotation >= 360.0f) return 180;
            else if (rotation <= -360) return 0;

            // [-180; 180] if needed
            if (scaleTo180) 
                rotation /= 2.0f;

            // [0; 180]
            var value = rotation / 4.0f + 90.0f;
            
            return (byte)value;
        }
    }
}
