using System;
using Demonixis.InMoov.Servos;
using Demonixis.ToolboxV2;

namespace Demonixis.InMoov
{
    [Serializable]
    public sealed class ServoSettings : SaveGame
    {
        private const string Filename = "servo.json";
        private static ServoSettings _instance;
        
        // Head
        public ServoData EyeX;
        public ServoData EyeY;
        public ServoData Jaw;
        public ServoData HeadYaw;
        public ServoData HeadPitch;
        public ServoData HeadRoll;
        public ServoData HeadRoll2;

        // Torso + Stomach
        
        // Left Arm
        
        // Left Hand
        
        // Right Arm
        
        // Right Hand
        
        public static void Save()
        {
            SaveRawData(GetPreferredStorageModeForSettings(), _instance, Filename);
        }

        public static ServoSettings Get()
        {
            if (_instance == null)
            {
                _instance = LoadRawData<ServoSettings>(GetPreferredStorageModeForSettings(), Filename);

                if (_instance == null)
                    _instance = new ServoSettings();
            }

            return _instance;
        }
    }
}