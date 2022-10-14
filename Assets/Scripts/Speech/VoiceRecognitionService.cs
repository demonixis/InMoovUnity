using System;

namespace Demonixis.InMoov.Speech
{
    public class VoiceRecognitionService : RobotService
    {
        public override RobotServices Type => RobotServices.Ears;
        
        public event Action<string> PhraseDetected;
        
        public override void SetPaused(bool paused) { }
        
        protected void NotifyPhraseDetected(string phrase)
        {
            PhraseDetected?.Invoke(phrase);
        }
    }
}