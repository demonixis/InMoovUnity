using System;
using Demonixis.InMoov.Settings;

namespace  Demonixis.InMoov.Services.Speech
{
    public class VoiceRecognitionService : RobotService
    {
        public override RobotServices Type => RobotServices.Ears;

        public event Func<RobotVoiceKeywords, string, bool> SystemKeywordDetected; 
        public event Action<string> PhraseDetected;
        
        public override void SetPaused(bool paused) { }
        
        protected void NotifyPhraseDetected(string phrase)
        {
            PhraseDetected?.Invoke(phrase);
        }
    }
}