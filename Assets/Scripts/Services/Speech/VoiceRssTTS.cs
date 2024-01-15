using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovSharp.Settings;
using Demonixis.InMoovSharp.Utils;
using System.Collections;
using TextToSpeech;
using UnityEngine;

namespace Demonixis.InMoovUnity.Services
{
    public class VoiceRssTTS : SpeechSynthesisService
    {
        public const string VoiceNameKey = "voicerss.voicename";

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
        private string _voice;

        public Language VoiceLanguage = Language.English_UnitedStates;
        public AudioCodecs SelectedCodec = AudioCodecs.OGG;

        protected override void SafeInitialize()
        {
            base.SafeInitialize();

            _audioSource = UnityRobotProxy.Instance.GetComponent<AudioSource>();
            _ttsManager = new TextToSpeechManager();

            var settings = GlobalSettings.Get();
            _ttsManager.APIKey = settings.VoiceRSSKey;

            if (string.IsNullOrEmpty(_ttsManager.APIKey))
            {
                Debug.LogError("You need a valid API key to use the VoiceRSS service.");
            }

            if (_customSettings.ContainsKey(VoiceNameKey))
                _voice = _customSettings[VoiceNameKey];
        }

        public override void SetLanguage(string culture)
        {
            switch (culture)
            {
                case "fr-FR":
                case "fr-CA":
                    VoiceLanguage = Language.French_France;
                    break;
                case "en-GB":
                    VoiceLanguage = Language.English_GreatBritain;
                    break;
                case "en-US":
                    VoiceLanguage = Language.English_UnitedStates;
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

            AddSetting(VoiceNameKey, _voice);
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
            var url = _ttsManager.GetTextToSpeechAudioWithIndex(message, (int)VoiceLanguage, (int)SelectedCodec, _voice);
            var www = new WWW(url); // FIXME need to switch to UnityWebRequest
            yield return www;

            var clip = www.GetAudioClip(false, false, _ttsManager.GetCurrentAudioTypeWithIndex((int)SelectedCodec));

            if (clip != null && clip.length > 0)
            {
                _audioSource.clip = clip;
                _audioSource.Play();

                NotifySpeechStarted(message);

                while (_audioSource.isPlaying)
                {
                    yield return null;
                }

                NotifySpeechState(false);

                yield return CoroutineFactory.WaitForSeconds(DelayAfterSpeak);

                NotifySpeechState(true);
            }
            else
            {
                Debug.Log(www.text);
            }
        }
    }
}
