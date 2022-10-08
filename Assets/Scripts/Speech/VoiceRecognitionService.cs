using System;

namespace Demonixis.InMoov.Speech
{
    public class VoiceRecognitionService : ImService
    {
        public override ImServices Type => ImServices.Ears;
        
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