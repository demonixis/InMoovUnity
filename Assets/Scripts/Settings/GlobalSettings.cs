using System;

namespace Demonixis.InMoov.Settings
{
    [Serializable]
    public enum SupportedLanguages
    {
        English = 0, French
    }
    
    [Serializable]
    public class GlobalSettings
    {
        public SupportedLanguages Language;
    }
}