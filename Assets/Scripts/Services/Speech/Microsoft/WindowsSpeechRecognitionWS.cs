namespace  Demonixis.InMoov.Services.Speech
{
    public class WindowsSpeechRecognitionWS : VoiceRecognitionService
    {
        public override void Initialize()
        {
            base.Initialize();
            SpeechLink.Instance.VoiceRecognized += Instance_VoiceRecognized;
        }

        private void Instance_VoiceRecognized(string obj)
        {
            NotifyPhraseDetected(obj);
        }
    }
}