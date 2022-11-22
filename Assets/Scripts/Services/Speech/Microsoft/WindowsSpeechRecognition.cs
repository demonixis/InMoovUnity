using UnityEngine;
using UnityEngine.Windows.Speech;

namespace  Demonixis.InMoov.Services.Speech
{
    public class WindowsSpeechRecognition : VoiceRecognitionService
    {
        public override RuntimePlatform[] SupportedPlateforms => new []
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer,
        };
        
        private DictationRecognizer _dictationRecognizer;
        private KeywordRecognizer _keywordRecognizer;
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

            /*var keywords = new[] {""};
            _keywordRecognizer = new KeywordRecognizer(keywords);
            _keywordRecognizer.OnPhraseRecognized += KeywordRecognizerOnOnPhraseRecognized;*/
            
            base.Initialize();
        }

        private void KeywordRecognizerOnOnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            throw new System.NotImplementedException();
        }

        public override void SetPaused(bool paused)
        {
            _paused = paused;
        }

        public override void Shutdown()
        {
            _dictationRecognizer.Dispose();
            //_keywordRecognizer.Dispose();
            base.Shutdown();
        }
    }
}