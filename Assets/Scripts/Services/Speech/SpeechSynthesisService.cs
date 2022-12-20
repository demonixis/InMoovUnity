using System;
using AIMLbot.AIMLTagHandlers;

namespace Demonixis.InMoov.Services.Speech
{
    public class SpeechSynthesisService : RobotService
    {
        public event Action SpeechStarted;
        public event Action SpeechFinished;
        
        public virtual void SetCulture(string culture)
        {
        }

        public virtual void Speak(string message)
        {
        }

        protected void NotifySpeechState(bool started)
        {
            if (started)
                SpeechStarted?.Invoke();
            else
                SpeechFinished?.Invoke();
        }
    }
}