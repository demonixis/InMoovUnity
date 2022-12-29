using System;

namespace Demonixis.InMoov.Services.Speech
{
    public class SpeechSynthesisService : RobotService
    {
        public override string SerializationFilename => "voice-synthesis";

        public event Action<string> SpeechStarted;
        public event Action SpeechJustFinished;
        public event Action SpeechFinishedSafe;

        public virtual void SetLanguage(string culture)
        {
        }

        public virtual string[] GetVoices() => null;

        public virtual int GetVoiceIndex() => 0;

        public virtual void SetVoice(string voiceName)
        {
        }

        public virtual void SetVoice(int voiceIndex)
        {
            AddSetting("VoiceIndex", $"{voiceIndex}");
        }

        public virtual void Speak(string message)
        {
        }

        public float GetSpeakTime(string sentence, int wordsPerMinute = 100)
        {
            var words = sentence.Split(' ');
            return words.Length * 60.0f / wordsPerMinute;
        }

        protected void NotifySpeechStarted(string message)
        {
            SpeechStarted?.Invoke(message);
        }

        protected void NotifySpeechState(bool safe)
        {
            if (safe)
                SpeechFinishedSafe?.Invoke();
            else
                SpeechJustFinished?.Invoke();

        }
    }
}