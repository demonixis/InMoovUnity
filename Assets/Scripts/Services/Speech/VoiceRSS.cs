using System.Collections;
using Demonixis.InMoov.Settings;
using TextToSpeech;
using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    [RequireComponent(typeof(AudioSource))]
    public class VoiceRSS : SpeechSynthesisService
    {
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

            var settings = GlobalSettings.GetInstance();
            _ttsManager.APIKey = settings.VoiceRSSKey;

            if (string.IsNullOrEmpty(_ttsManager.APIKey))
            {
                Debug.LogError("You need a valid API key to use the VoiceRSS service.");
            }
        }

        public override void Speak(string text)
        {
            if (Paused) return;
            StartCoroutine(SpeakCoroutine(text));
        }

        private IEnumerator SpeakCoroutine(string text)
        {
            var url = _ttsManager.GetTextToSpeechAudioWithIndex(text, (int) _language, (int) _audioCodecs, _voice);
            var www = new WWW(url);
            yield return www;

            var clip = www.GetAudioClip(false, false, _ttsManager.GetCurrentAudioTypeWithIndex((int) _audioCodecs));

            if (clip != null && clip.length > 0)
            {
                _audioSource.clip = clip;
                _audioSource.Play();
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
    }
}