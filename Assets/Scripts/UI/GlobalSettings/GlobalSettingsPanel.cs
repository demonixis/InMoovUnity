using Demonixis.InMoov.Settings;
using TMPro;
using UnityEngine;

namespace Demonixis.InMoov.UI
{
    public sealed class GlobalSettingsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _languagesList;
        [SerializeField] private TMP_InputField _openAIKey;
        [SerializeField] private TMP_InputField _voiceRSSKey;

        private void Start()
        {
            _languagesList.options.Clear();

            foreach (var lang in GlobalSettings.SupportedLanguages)
            {
                _languagesList.options.Add(new TMP_Dropdown.OptionData(lang));
            }
            
            var settings = GlobalSettings.Get();

            _languagesList.SetValueWithoutNotify(settings.GetLanguageIndex());
            _languagesList.RefreshShownValue();
            _languagesList.onValueChanged.AddListener(i =>
            {
                var globalSettings = GlobalSettings.Get();
                globalSettings.SetLanguageByIndex(i);
                Robot.Instance.BrainSpeechProxy.SetLanguage(globalSettings.Language);
            });
            
            _openAIKey.SetTextWithoutNotify(settings.OpenAIKey);
            _openAIKey.onValueChanged.AddListener(s =>
            {
                var globalSettings = GlobalSettings.Get();
                globalSettings.OpenAIKey = s;
            });
            
            _voiceRSSKey.SetTextWithoutNotify(settings.VoiceRSSKey);
            _voiceRSSKey.onValueChanged.AddListener(s =>
            {
                var globalSettings = GlobalSettings.Get();
                globalSettings.VoiceRSSKey = s;
            });
        }
    }
}