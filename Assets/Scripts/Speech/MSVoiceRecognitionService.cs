using Demonixis.InMoov.Speech;
using UnityEngine.Windows.Speech;

namespace Demonixis.InMoov.Speech.Microsoft
{
    public class MSVoiceRecognitionService : VoiceRecognitionService
    {
        private DictationRecognizer _dictationRecognizer;
        private bool _paused;
        
        public override void Initialize()
        {
            _dictationRecognizer = new DictationRecognizer();
            _dictationRecognizer.DictationResult += (text, confidence) =>
            {
                if (_paused) return;
                NotifyPhraseDetected(text);
            };
            _dictationRecognizer.Start();
        }

        public override void SetPaused(bool paused)
        {
            _paused = paused;
        }

        public override void Shutdown()
        {
            _dictationRecognizer.Dispose();
        }
    }
}