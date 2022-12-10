using UnityEngine;

namespace Demonixis.InMoov.Services.Speech
{
    [RequireComponent(typeof(AudioSource))]
    public class SAMSpeechSynthesis : SpeechSynthesisService
    {
        private AudioSource _audioSource;

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

            var audioClip = AudioClip.Create("TemporaryAC", buffer.Size, 1, 22050, false);
            audioClip.SetData(buffer.GetFloats(), 0);

            _audioSource.clip = audioClip;
            _audioSource.Play();
        }
    }
}