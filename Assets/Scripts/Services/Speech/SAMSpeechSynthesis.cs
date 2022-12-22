using System.Collections;
using Demonixis.InMoov.Utils;
using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SAMSpeechSynthesis : SpeechSynthesisService
    {
        private AudioSource _audioSource;
        
        [SerializeField] private int _samplerate = 22000;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
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
            NotifySpeechState(true, message);
            
            while (_audioSource.isPlaying)
            {
                yield return null;
            }
            
            yield return CoroutineFactory.WaitForSeconds(1.0f);
            
            NotifySpeechState(false, null);
        }
    }
}