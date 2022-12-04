using Demonixis.InMoov.Chatbots;
using Demonixis.InMoov.ComputerVision;
using Demonixis.InMoov.Navigation;
using Demonixis.InMoov.Services.Speech;
using Demonixis.InMoov.Servos;
using System;
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
        [Header("Services")] public string Chatbot;
        public string SpeechSynthesis;
        public string VoiceRecognition;
        public string Navigation;
        public string EyeCamera;
        public string SpatialMapping;
        public string ServoMixer;

        [Header("Settings")] public byte LeftEyeCameraIndex;
        public byte RightEyeCameraIndex;
        public float VRStereoOffset;

        public bool IsValid() =>
            !string.IsNullOrEmpty(Chatbot) &&
            !string.IsNullOrEmpty(SpeechSynthesis) &&
            !string.IsNullOrEmpty(VoiceRecognition) &&
            !string.IsNullOrEmpty(ServoMixer);

        public static ServiceList New()
        {
            return new ServiceList
            {
                // Services
                Chatbot = nameof(AIMLNetService),
                VoiceRecognition = nameof(VoskVoiceRecognitionService),
#if UNITY_STANDALONE_WIN
                SpeechSynthesis = nameof(WindowsSpeechSynthesis),
#else
                SpeechSynthesis = nameof(SpeechSynthesisService),
#endif
                Navigation = nameof(NavigationService),
                EyeCamera = nameof(EyeCamera),
                SpatialMapping = nameof(DepthMappingService),
                ServoMixer = nameof(ServoMixerService)
            };
        }
    }
}