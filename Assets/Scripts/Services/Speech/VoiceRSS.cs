using System.Collections;
using Demonixis.InMoov.Settings;
using Demonixis.InMoov.Utils;
using TextToSpeech;
using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class VoiceRSS : SpeechSynthesisService
    {
        public const string VoiceRSSFilename = "voicerss.json";
        
        public readonly string[] Voices =
        {
            "Default",
            "en-US_Linda",
            "en-US_Amy",
            "en-US_Mary",
            "en-US_John",
            "fr-FR_Bette",
            "fr-FR_Iva",
            "fr-FR_Zola",
            "fr-FR_Axel"
        };

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

        public override void SetVoice(string voiceName)
        {
            if (voiceName == "Default")
            {
                _voice = null;
                return;
            }

            // voiceName is like frFR_VoiceName
            var tmp = voiceName.Split('_');
            if (tmp.Length != 2)
            {
                Debug.LogError($"[VoiceRSS] SelectVoice/Bad voice format: {voiceName}");
                return;
            }

            _voice = tmp[1];
        }

        public override void SetVoice(int voiceIndex)
        {
            if (voiceIndex < 0 || voiceIndex >= Voices.Length)
            {
                Debug.LogError($"[VoiceRSS] The VoiceIndex is not valid: {voiceIndex}");
                return;
            }

            if (voiceIndex == 0)
            {
                _voice = null;
                return;
            }

            var tmp = Voices[voiceIndex].Split('_');

            _voice = tmp[1];
        }

        public override string[] GetVoices()
        {
            return Voices;
        }

        public override int GetVoiceIndex()
        {
            if (string.IsNullOrEmpty(_voice))
                return 0;

            for (var i = 0; i < Voices.Length; i++)
            {
                if (!Voices[i].Contains(_voice)) continue;
                return i;
            }

            return 0;
        }

        public override void Speak(string text)
        {
            if (Paused) return;
            StartCoroutine(SpeakCoroutine(text));
        }

        private IEnumerator SpeakCoroutine(string message)
        {
            var url = _ttsManager.GetTextToSpeechAudioWithIndex(message, (int) _language, (int) _audioCodecs, _voice);
            var www = new WWW(url); // FIXME need to switch to UnityWebRequest
            yield return www;

            var clip = www.GetAudioClip(false, false, _ttsManager.GetCurrentAudioTypeWithIndex((int) _audioCodecs));

            if (clip != null && clip.length > 0)
            {
                _audioSource.clip = clip;
                _audioSource.Play();

                NotifySpeechState(true, message);

                while (_audioSource.isPlaying)
                {
                    yield return null;
                }
                
                yield return CoroutineFactory.WaitForSeconds(1.0f);

                NotifySpeechState(false, null);
            }
            else
            {
                Debug.Log(www.text);
            }
        }
    }
}