using System;

namespace Demonixis.InMoov.Services.Speech
{
    public class VoiceRecognitionService : RobotService
    {
        public event Action<string> PhraseDetected;

        public void SetCulture(string culture) { }
        
        protected void NotifyPhraseDetected(string phrase)
        {
            PhraseDetected?.Invoke(phrase);
        }
    }
}