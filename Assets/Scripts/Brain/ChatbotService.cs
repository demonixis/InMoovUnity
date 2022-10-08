namespace Demonixis.InMoov.Chatbots
{
    public abstract class ChatbotService : ImService
    {
        public override ImServices Type => ImServices.Chat;
        
        public abstract string GetResponse(string inputSpeech);
    }
}