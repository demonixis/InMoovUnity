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
        private const string GlobalSettingsFilename = "global-settings.json";
        private static GlobalSettings _instance;

        private static readonly SystemLanguage[] SupportedLanguages =
        {
            SystemLanguage.French,
            SystemLanguage.English
        };

        public SystemLanguage Language;

        [Header("Settings")] public byte LeftEyeCameraIndex;
        public byte RightEyeCameraIndex;
        public float VRStereoOffset;

        [Header("Keys")] public string VoiceRSSKey;
        public string OpenAIKey;

        public GlobalSettings()
        {
            Language = SystemLanguage.English;
        }

        public static GlobalSettings GetInstance()
        {
            if (_instance == null)
            {
                _instance = SaveGame.LoadRawData<GlobalSettings>(SaveGame.GetPreferredStorageMode(),
                    GlobalSettingsFilename, "Config");

                if (_instance == null)
                    _instance = new GlobalSettings();
            }

            return _instance;
        }

        public static void Save()
        {
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), GetInstance(), GlobalSettingsFilename, "Config");
        }
    }

    [Serializable]
    public struct ServiceList
    {
        [Header("Services")] public string Chatbot;
        public string SpeechSynthesis;
        public string VoiceRecognition;
        public string Navigation;
        public string ComputerVision;
        public string ServoMixer;
        public string XR;

        public bool IsValid() =>
            !string.IsNullOrEmpty(Chatbot) &&
            !string.IsNullOrEmpty(SpeechSynthesis) &&
            !string.IsNullOrEmpty(VoiceRecognition) &&
            !string.IsNullOrEmpty(ServoMixer) &&
            !string.IsNullOrEmpty(XR);

        public static ServiceList New()
        {
            return new ServiceList
            {
                // Services
                Chatbot = nameof(AIMLNetService),
                VoiceRecognition = nameof(VoskVoiceRecognitionService),
                SpeechSynthesis = nameof(SAMSpeechSynthesis),
                Navigation = nameof(NavigationService),
                ServoMixer = nameof(ServoMixerService),
                ComputerVision = nameof(ComputerVisionService),
                XR = nameof(XRService)
            };
        }
    }
}