using System;

namespace Demonixis.InMoov.Chatbots
{
    public abstract class ChatbotService : RobotService
    {
        public override RobotServices Type => RobotServices.Chat;

        public event Action<string> ResponseReady;

        public abstract void SetCulture(string culture);
        
        public abstract void SubmitResponse(string inputSpeech);

        public void NotifyResponseReady(string response)
        {
            ResponseReady?.Invoke(response);
        }
    }
}