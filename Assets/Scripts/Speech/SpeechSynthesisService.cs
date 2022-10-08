namespace Demonixis.InMoov.Speech
{
    public class SpeechSynthesisService : ImService
    {
        public override ImServices Type => ImServices.Voice;

        public override void Initialize() { }

        public override void SetPaused(bool paused) { }

        public override void Shutdown() { }

        public virtual void Speak(string message) { }
    }
}