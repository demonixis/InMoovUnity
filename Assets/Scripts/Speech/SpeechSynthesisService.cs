namespace Demonixis.InMoov.Speech
{
    public abstract class SpeechSynthesisService : ImService
    {
        public override ImServices Type => ImServices.Voice;

        public abstract void Speak(string message);
    }
}