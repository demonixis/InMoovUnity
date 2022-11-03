namespace Demonixis.InMoov.Speech
{
    public class WindowsSpeechSynthesisWS : SpeechSynthesisService
    {
        public override void Speak(string message)
        {
            SpeechLink.Instance.Speak(message);
        }
    }
}
