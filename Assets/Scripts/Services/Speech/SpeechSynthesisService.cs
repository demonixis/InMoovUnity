namespace  Demonixis.InMoov.Services.Speech
{
    public class SpeechSynthesisService : RobotService
    {
        public override RobotServices Type => RobotServices.Voice;
        
        public override void SetPaused(bool paused) { }

        public virtual void Speak(string message) { }
    }
}