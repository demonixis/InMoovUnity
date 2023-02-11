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

        public static readonly string[] SupportedLanguages =
        {
            "en-US",
            "fr-FR"
        };

        public string Language;

        [Header("Keys")] public string VoiceRSSKey;
        public string OpenAIKey;

        public GlobalSettings()
        {
            Language = SupportedLanguages[0];
        }

        public int GetLanguageIndex()
        {
            return Array.IndexOf(SupportedLanguages, Language);
        }

        public void SetLanguageByIndex(int index)
        {
            if (index < 0 || index >= SupportedLanguages.Length)
            {
                Debug.Log(
                    $"Error the language index is not valid, entered {index} but maximum is {SupportedLanguages.Length}");
                return;
            }

            Language = SupportedLanguages[index];
        }

        public static GlobalSettings Get()
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
            SaveGame.SaveRawData(SaveGame.GetPreferredStorageMode(), Get(), GlobalSettingsFilename, "Config");
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
#if UNITY_STANDALONE_WIN || UNITY_WSA
                SpeechSynthesis = nameof(WindowsSpeechSynthesisWS),
#elif UNITY_STANDALONE_OSX
                SpeechSynthesis = nameof(MacosSpeechSynthesis),
#else
                SpeechSynthesis = nameof(SAMSpeechSynthesis),
#endif
                Navigation = nameof(NavigationService),
                ServoMixer = nameof(ServoMixerService),
                ComputerVision = nameof(ComputerVisionService),
                XR = nameof(XRService)
            };
        }
    }
}