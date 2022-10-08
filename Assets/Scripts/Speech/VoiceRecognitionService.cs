using System;

namespace Demonixis.InMoov.Speech
{
    public abstract class VoiceRecognitionService : ImService
    {
        public override ImServices Type => ImServices.Ears;
        
        public event Action<string> PhraseDetected;

        protected void NotifyPhraseDetected(string phrase)
        {
            PhraseDetected?.Invoke(phrase);
        }
    }
}