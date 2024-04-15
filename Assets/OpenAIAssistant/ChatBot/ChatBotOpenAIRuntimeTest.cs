using UnityEngine;

namespace UHTTP.Sample.OpenAIAssistant.ChatBot
{
    public class ChatBotOpenAIRuntimeTest : MonoBehaviour
    {
        [SerializeField,TextArea] private string message;
        [SerializeField] private ChatBotConfig config;
        private ChatBotOpenAI chatBot;

        private void Awake() =>
            chatBot = new ChatBotOpenAI(config,() => Debug.Log("Chat bot Initialized"));

        [ContextMenu("Send Message")]
        public void SendMessageLocal() =>
            chatBot.SendMessage(message,messages => {});

        [ContextMenu("Get Messages")]
        public void GetMessages() =>
            chatBot.GetMessages(message => {});

        [ContextMenu("Set Config")]
        public void SetConfig() =>
            chatBot.SetConfig(config,() => Debug.Log("Chat bot ReConfiged"));
    }
}