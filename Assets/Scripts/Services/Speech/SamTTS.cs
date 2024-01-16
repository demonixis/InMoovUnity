using System.Collections;
using Demonixis.InMoovSharp.Services;
using Demonixis.InMoovSharp.Utils;
using UnityEngine;

namespace Demonixis.InMoovUnity.Services
{
    public sealed class SamTTS : SpeechSynthesisService
    {
        private AudioSource _audioSource;

        [SerializeField] private int _samplerate = 22000;

        protected override void SafeInitialize()
        {
            base.SafeInitialize();
            _audioSource = UnityRobotProxy.Instance.GetComponent<AudioSource>();
        }

        public override void Speak(string message)
        {
            if (Paused) return;

            message += "."; // TODO: my C# port seems to crash without final punctuation.

            var output = UnitySAM.TextToPhonemes(message + "[", out int[] ints);

            UnitySAM.SetInput(ints);

            var buffer = UnitySAM.SAMMain();
            if (buffer == null)
            {
                Debug.LogError("Buffer was null");
                return;
            }

            var audioClip = AudioClip.Create("TemporaryAC", buffer.Size, 1, _samplerate, false);
            audioClip.SetData(buffer.GetFloats(), 0);

            _audioSource.clip = audioClip;
            _audioSource.Play();
            StartCoroutine(SpeechLoop(message));
        }

        private IEnumerator SpeechLoop(string message)
        {
            NotifySpeechStarted(message);

            while (_audioSource.isPlaying)
            {
                yield return null;
            }

            NotifySpeechState(false);

            yield return CoroutineFactory.WaitForSeconds(1.0f);

            NotifySpeechState(true);
        }
    }
}