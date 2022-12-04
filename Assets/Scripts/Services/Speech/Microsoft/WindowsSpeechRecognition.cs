#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
#define MS_SPEECH_SYNTHESIS
#endif

using UnityEngine;
#if MS_SPEECH_SYNTHESIS
using UnityEngine.Windows.Speech;
#endif

namespace Demonixis.InMoov.Services.Speech
{
    public class WindowsSpeechRecognition : VoiceRecognitionService
    {
        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer,
        };

#if MS_SPEECH_SYNTHESIS
        private DictationRecognizer _dictationRecognizer;
#endif
        private bool _paused;

        public override void Initialize()
        {
#if MS_SPEECH_SYNTHESIS
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
#endif
            base.Initialize();
        }

        public override void SetPaused(bool paused)
        {
            _paused = paused;
        }

        public override void Shutdown()
        {
#if MS_SPEECH_SYNTHESIS
            _dictationRecognizer.Dispose();
#endif
            base.Shutdown();
        }
    }
}