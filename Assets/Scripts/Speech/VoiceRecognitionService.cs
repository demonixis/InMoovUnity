using System;

namespace Demonixis.InMoov.Speech
{
    public class VoiceRecognitionService : RobotService
    {
        public override RobotServices Type => RobotServices.Ears;
        
        public event Action<string> PhraseDetected;

        public override void Initialize() { }

        public override void SetPaused(bool paused) { }

        public override void Shutdown() { }

        protected void NotifyPhraseDetected(string phrase)
        {
            PhraseDetected?.Invoke(phrase);
        }
    }
}