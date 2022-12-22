using System;

namespace Demonixis.InMoov.Services.Speech
{
    public class VoiceRecognitionService : RobotService
    {
        protected bool _isLocked;

        public bool CanListen => !_isLocked && !Paused;

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
            speech.SpeechStarted += _ => SetLocked(true);
            speech.SpeechFinished += () => SetLocked(false);
        }

        protected virtual void SetLocked(bool locked)
        {
            _isLocked = locked;
        }

        public virtual void SetLanguage(string culture)
        {
        }

        protected void NotifyPhraseDetected(string phrase)
        {
            PhraseDetected?.Invoke(phrase);
        }
    }
}