namespace Demonixis.InMoov.Speech
{
    public class SpeechSynthesisService : RobotService
    {
        public override RobotServices Type => RobotServices.Voice;

        public override void Initialize() { }

        public override void SetPaused(bool paused) { }

        public override void Shutdown() { }

        public virtual void Speak(string message) { }
    }
}