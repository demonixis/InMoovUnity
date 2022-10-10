namespace Demonixis.InMoov.Chatbots
{
    public abstract class ChatbotService : RobotService
    {
        public override RobotServices Type => RobotServices.Chat;
        
        public abstract string GetResponse(string inputSpeech);
    }
}