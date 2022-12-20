using UnityEngine;
using Yetibyte.Unity.SpeechRecognition;

namespace Demonixis.InMoov.Services.Speech
{
    [RequireComponent(typeof(VoskListener))]
    public sealed class VoskVoiceRecognitionService : VoiceRecognitionService
    {
        public override void Initialize()
        {
            base.Initialize();

            var vosk = GetComponent<VoskListener>();
            vosk.LoadModel();
            vosk.StartListening();
            vosk.ResultFound += Vosk_ResultFound;
        }

        public override void SetCulture(string culture)
        {
            
        }

        public override void SetPaused(bool paused)
        {
            var vosk = GetComponent<VoskListener>();
            if (paused)
                vosk.StopListening();
            else
                vosk.StartListening();
        }

        private void Vosk_ResultFound(object sender, VoskResultEventArgs e)
        {
            if (_isLocked) return;
            NotifyPhraseDetected(e.Result.Text);
        }

        public override void Shutdown()
        {
            var vosk = GetComponent<VoskListener>();
            vosk.StopListening();
            vosk.ResultFound -= Vosk_ResultFound;
        }
    }
}