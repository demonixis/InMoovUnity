using System;
using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    public class VoiceRecognitionService : RobotService
    {
        private const string WordTriggerKey = "WordTrigger";
        protected bool _isLocked;

        public bool IsLocked => _isLocked;
        public bool CanListen => !_isLocked && !Paused;

        public override string SerializationFilename => "voice-recognition";

        [SerializeField] private string _wordTrigger = "Robot";

        public string WordTrigger
        {
            get => _wordTrigger;
            set
            {
                _wordTrigger = value.ToLower();
                AddSetting(WordTriggerKey, _wordTrigger);
            }
        }

        public event Action<bool> ListenChanged;
        public event Action<string> PhraseDetected;

        public override void Initialize()
        {
            base.Initialize();

            if (_customSettings.TryGetValue(WordTriggerKey, out string value))
                _wordTrigger = value;

            if (!string.IsNullOrEmpty(_wordTrigger))
                _wordTrigger = _wordTrigger.ToLower();

            Robot.Instance.WhenStarted(InternalInitialize);
        }

        private void InternalInitialize()
        {
            var robot = Robot.Instance;
            var speech = robot.GetService<SpeechSynthesisService>();
            speech.SpeechStarted += _ => SetLocked(true);
            speech.SpeechFinishedSafe += () => SetLocked(false);
        }

        protected virtual void SetLocked(bool locked)
        {
            _isLocked = locked;
            ListenChanged?.Invoke(CanListen);
        }

        public override void SetPaused(bool paused)
        {
            base.SetPaused(paused);
            ListenChanged?.Invoke(CanListen);
        }

        public virtual void SetLanguage(string culture)
        {
        }

        protected void NotifyPhraseDetected(string phrase)
        {
            if (!string.IsNullOrEmpty(_wordTrigger))
            {
                if (!phrase.StartsWith(_wordTrigger)) return;
                phrase = phrase.Remove(0, _wordTrigger.Length);
            }

            PhraseDetected?.Invoke(phrase);
        }
    }
}