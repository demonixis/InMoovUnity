using Demonixis.InMoov.Speech;
using UnityEngine;
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
            _dictationRecognizer.DictationComplete += cause => Debug.Log("DR Completed");
            _dictationRecognizer.DictationError += (error, hresult) => Debug.Log($"DR Error: {error}");
            _dictationRecognizer.DictationHypothesis += text => Debug.Log($"DR H: {text}");
            _dictationRecognizer.DictationResult += (text, confidence) =>
            {
                Debug.Log($"DR Result: {text} - {confidence}");
                if (_paused) return;
                NotifyPhraseDetected(text);
            };
            _dictationRecognizer.Start();
            Debug.Log("DR Init");
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