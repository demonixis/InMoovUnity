using System;

namespace Demonixis.InMoov.Services.Speech
{
    public class VoiceRecognitionService : RobotService
    {
        protected bool _isLocked;

        public event Action<string> PhraseDetected;

        public override void Initialize()
        {
            base.Initialize();
            Robot.Instance.WhenStarted(InternalInitialize);
        }

        private void InternalInitialize()
        {
            var robot = Robot.Instance;
            var speech = robot.GetService<SpeechSynthesisService>();
            speech.SpeechStarted += () => OnSpeechLocked(true);
            speech.SpeechFinished += () => OnSpeechLocked(false);
        }

        public virtual void SetCulture(string culture)
        {
        }

        protected virtual void OnSpeechLocked(bool locked)
        {
            _isLocked = locked;
        }

        protected void NotifyPhraseDetected(string phrase)
        {
            PhraseDetected?.Invoke(phrase);
        }
    }
}