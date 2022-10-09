using System;
using Demonixis.InMoov;
using Demonixis.ToolboxV2;

namespace Settings
{
    [Serializable]
    public class SerialSettings : SaveGame
    {
        private const string Filename = "serial.json";
        private static SerialSettings _instance;
        
        public SerialData Arduino1;
        public SerialData Arduino2;
        public SerialData Arduino3;
        public SerialData Arduino4;
        
        public static void Save()
        {
            SaveRawData(GetPreferredStorageModeForSettings(), _instance, Filename);
        }

        public static SerialSettings Get()
        {
            if (_instance == null)
            {
                _instance = LoadRawData<SerialSettings>(GetPreferredStorageModeForSettings(), Filename);

                if (_instance == null)
                    _instance = new SerialSettings();
            }

            return _instance;
        }
    }
}