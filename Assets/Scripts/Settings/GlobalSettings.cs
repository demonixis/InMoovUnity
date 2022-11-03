using System;
using Demonixis.InMoov.Animations;
using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.ComputerVision;
using Demonixis.InMoov.Navigation;
using Demonixis.InMoov.Servos;
using Demonixis.InMoov.Speech;
using UnityEngine;

namespace Demonixis.InMoov.Settings
{
    [Serializable]
    public class GlobalSettings
    {
        private SystemLanguage[] SupportedLanguages = new[]
        {
            SystemLanguage.French,
            SystemLanguage.English
        };

        public SystemLanguage Language;
    }

    [Serializable]
    public struct ServiceList
    {
        public string Chatbot;
        public string SpeechSynthesis;
        public string VoiceRecognition;
        public string Animation;
        public string Navigation;
        public string EyeCamera;
        public string SpatialMapping;
        public string ServoMixer;

        public bool IsValid() => !string.IsNullOrEmpty(Chatbot) && !string.IsNullOrEmpty(SpeechSynthesis) &&
                                 !string.IsNullOrEmpty(VoiceRecognition);

        public static ServiceList New()
        {
            return new ServiceList
            {
                Chatbot = nameof(AIMLNetService),
#if UNITY_STANDALONE_WIN
                SpeechSynthesis = nameof(WindowsSpeechSynthesis),
                VoiceRecognition = nameof(WindowsSpeechRecognition),
#else
                SpeechSynthesis = nameof(SpeechSynthesisService),
                VoiceRecognition = nameof(VoiceRecognitionService),
#endif
                Animation = nameof(MecanimAnimationService),
                Navigation = nameof(NavigationService),
                EyeCamera = nameof(EyeCamera),
                SpatialMapping = nameof(DepthMappingService),
                ServoMixer = nameof(ServoMixerService)
            };
        }
    }
}