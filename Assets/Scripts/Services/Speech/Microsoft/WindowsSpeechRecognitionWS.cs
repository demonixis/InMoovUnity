using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    public class WindowsSpeechRecognitionWS : VoiceRecognitionService
    {
        public override RuntimePlatform[] SupportedPlateforms => new[]
        {
            RuntimePlatform.WindowsEditor,
            RuntimePlatform.WindowsPlayer
        };

        public override void Initialize()
        {
            base.Initialize();
            SpeechLink.Instance.VoiceRecognized += Instance_VoiceRecognized;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            SpeechLink.Instance.VoiceRecognized -= Instance_VoiceRecognized;
        }

        public override void SetLanguage(string culture)
        {
            SpeechLink.Instance.SetLanguage(culture);
        }

        private void Instance_VoiceRecognized(string obj)
        {
            if (!CanListen) return;
            NotifyPhraseDetected(obj);
        }
    }
}