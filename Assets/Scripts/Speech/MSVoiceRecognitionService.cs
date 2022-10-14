using Demonixis.InMoov.Speech;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Demonixis.InMoov.Speech.Microsoft
{
    public class MSVoiceRecognitionService : VoiceRecognitionService
    {
        public override RobotPlatform[] SupportedPlateforms => new[] {RobotPlatform.Windows};
        
        private DictationRecognizer _dictationRecognizer;
        private bool _paused;
        
        public override void Initialize()
        {
            _dictationRecognizer = new DictationRecognizer();
            _dictationRecognizer.DictationComplete += cause => Debug.Log($"[DictationComplete] {cause}");
            _dictationRecognizer.DictationError += (error, hresult) => Debug.Log($"[DictationError] {error}");
            _dictationRecognizer.DictationHypothesis += text => Debug.Log($"[DictationHypothesis] {text}");
            _dictationRecognizer.DictationResult += (text, confidence) =>
            {
                Debug.Log($"[DictationResult] {text} - {confidence}");
                if (_paused) return;
                NotifyPhraseDetected(text);
            };
            _dictationRecognizer.Start();
            base.Initialize();
        }

        public override void SetPaused(bool paused)
        {
            _paused = paused;
        }

        public override void Shutdown()
        {
            _dictationRecognizer.Dispose();
            base.Shutdown();
        }
    }
}