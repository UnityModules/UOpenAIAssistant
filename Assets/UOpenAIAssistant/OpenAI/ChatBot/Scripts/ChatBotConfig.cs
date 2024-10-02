
namespace OpenAI.Assistant.ChatBot
{
    [System.Serializable]
    public record ChatBotConfig
    {
        public string token;
        public string assistantId;
        public string threadId;
        public float delayGetMessage = 2;
        public bool logInEditor = true;
    }
}