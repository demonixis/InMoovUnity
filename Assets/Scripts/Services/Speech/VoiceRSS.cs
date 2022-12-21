using System.Collections;
using Demonixis.InMoov.Settings;
using TextToSpeech;
using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class VoiceRSS : SpeechSynthesisService
    {
        public enum Voices
        {
            enUS_Linda,
            enUS_Amy,
            enUS_Mary,
            enUS_John,
            frFR_Bette,
            frFR_Iva,
            frFR_Zola,
            frFR_Axel
        }

        private TextToSpeechManager _ttsManager;
        private AudioSource _audioSource;

        [SerializeField] private Language _language = Language.English_UnitedStates;
        [SerializeField] private AudioCodecs _audioCodecs = AudioCodecs.OGG;
        [SerializeField] private string _voice;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _ttsManager = new TextToSpeechManager();
        }

        public override void Initialize()
        {
            base.Initialize();

            var settings = GlobalSettings.Get();
            _ttsManager.APIKey = settings.VoiceRSSKey;

            if (string.IsNullOrEmpty(_ttsManager.APIKey))
            {
                Debug.LogError("You need a valid API key to use the VoiceRSS service.");
            }
        }

        public override void SetLanguage(string culture)
        {
            switch (culture)
            {
                case "fr-FR":
                case "fr-CA":
                    _language = Language.French_France;
                    break;
                case "en-GB":
                    _language = Language.English_GreatBritain;
                    break;
                case "en-US":
                    _language = Language.English_UnitedStates;
                    break;
            }

            // Feel free to continue this list ;)      
        }

        public override void SelectVoice(string voiceName)
        {
            // voiceName is like frFR_VoiceName
            var tmp = voiceName.Split('_');
            if (tmp.Length != 2)
            {
                Debug.LogError($"[VoiceRSS] SelectVoice/Bad voice format: {voiceName}");
                return;
            }

            _voice = tmp[1];
        }

        public override void Speak(string text)
        {
            if (Paused) return;
            StartCoroutine(SpeakCoroutine(text));
        }

        private IEnumerator SpeakCoroutine(string text)
        {
            var url = _ttsManager.GetTextToSpeechAudioWithIndex(text, (int) _language, (int) _audioCodecs, _voice);
            var www = new WWW(url); // FIXME need to switch to UnityWebRequest
            yield return www;

            var clip = www.GetAudioClip(false, false, _ttsManager.GetCurrentAudioTypeWithIndex((int) _audioCodecs));

            if (clip != null && clip.length > 0)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
                StartCoroutine(SpeechLoop(text));
            }
            else
            {
                Debug.Log(www.text);
                Debug.LogError("Failed to get the voice. Please try:\n" +
                               "1.Try it in other languages.\n" +
                               "2.Fill in something in text field.\n" +
                               "3.Choose the correct audio format.");
            }
        }

        private IEnumerator SpeechLoop(string message)
        {
            NotifySpeechState(true, message);

            while (_audioSource.isPlaying)
            {
                yield return null;
            }

            NotifySpeechState(false, null);
        }
    }
}