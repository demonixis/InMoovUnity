using System;
using AIMLbot.AIMLTagHandlers;

namespace Demonixis.InMoov.Services.Speech
{
    public class SpeechSynthesisService : RobotService
    {
        public event Action<string> SpeechStarted;
        public event Action SpeechFinished;
        
        public virtual void SetLanguage(string culture)
        {
        }

        public virtual string[] GetVoices() => null;

        public virtual void SelectVoice(string voiceName)
        {
            
        }

        public virtual void Speak(string message)
        {
        }

        public float GetSpeakTime(string sentence, int wordsPerMinute = 100)
        {
            var words = sentence.Split(' ');
            return words.Length * 60.0f / wordsPerMinute;
        }

        protected void NotifySpeechState(bool started, string message)
        {
            if (started)
                SpeechStarted?.Invoke(message);
            else
                SpeechFinished?.Invoke();
        }
    }
}