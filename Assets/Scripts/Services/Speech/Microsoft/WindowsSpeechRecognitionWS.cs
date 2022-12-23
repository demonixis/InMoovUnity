#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
#define MS_SPEECH_SYNTHESIS
#endif

using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    public class WindowsSpeechRecognitionWS : VoiceRecognitionService
    {
#if MS_SPEECH_SYNTHESIS
        private SpeechLink _speechLink;
#endif

        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer
        };

#if MS_SPEECH_SYNTHESIS
        public override void Initialize()
        {
            base.Initialize();

            _speechLink = SpeechLink.Instance;
            _speechLink.Initialize();
            _speechLink.EnableVoiceRecognition = true;
            _speechLink.VoiceRecognized += Instance_VoiceRecognized;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            _speechLink.EnableVoiceRecognition = false;
            _speechLink.VoiceRecognized -= Instance_VoiceRecognized;
        }

        public override void SetLanguage(string culture)
        {
            _speechLink.SetLanguage(culture);
        }

        private void Instance_VoiceRecognized(string message)
        {
            if (!CanListen) return;
            NotifyPhraseDetected(message);
        }
#endif
    }
}