#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
#define MS_SPEECH_SYNTHESIS
#endif

using UnityEngine;
#if MS_SPEECH_SYNTHESIS
using UnityEngine.Windows.Speech;
#endif

namespace Demonixis.InMoov.Services.Speech
{
    public class UMSSpeechRecognitionService : VoiceRecognitionService
    {
        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer,
            RuntimePlatform.WSAPlayerX64
        };

#if MS_SPEECH_SYNTHESIS
        private DictationRecognizer _dictationRecognizer;

        public override void Initialize()
        {
            base.Initialize();

            _dictationRecognizer = new DictationRecognizer();
            _dictationRecognizer.DictationComplete += cause => Debug.Log($"[DictationComplete] {cause}");
            _dictationRecognizer.DictationError += (error, hresult) => Debug.Log($"[DictationError] {error}");
            _dictationRecognizer.DictationHypothesis += text => Debug.Log($"[DictationHypothesis] {text}");
            _dictationRecognizer.DictationResult += (text, confidence) =>
            {
                if (!CanListen) return;
                NotifyPhraseDetected(text);
            };
            _dictationRecognizer.Start();

            base.Initialize();
        }

        public override void Shutdown()
        {
            _dictationRecognizer?.Dispose();
            base.Shutdown();
        }

#endif
    }
}