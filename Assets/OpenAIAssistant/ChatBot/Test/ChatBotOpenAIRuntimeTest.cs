using UnityEngine;

namespace OpenAIAssistant.ChatBot
{
    public class ChatBotOpenAIRuntimeTest : MonoBehaviour
    {
        [SerializeField,TextArea] private string message;
        [SerializeField] private ChatBotConfig config;
        private ChatBotOpenAI chatBot;

        private void Awake() =>
            chatBot = new ChatBotOpenAI(config,() => Debug.Log("<color=green>Chat bot Initialized</color>"));

        [ContextMenu("Send Message")]
        public void SendMessageLocal() =>
            chatBot.SendMessage(message,messageResp => { Debug.Log("message: <color=green>"+ messageResp.Contents[0].Text.Value+"</color>"); });

        [ContextMenu("Get Messages")]
        public void GetMessages() =>
            chatBot.GetMessages(messages => { Debug.Log("message Count : "+messages.Length); });

        [ContextMenu("Set Config")]
        public void SetConfig() =>
            chatBot.SetConfig(config,() => Debug.Log("Chat bot ReConfiged"));
    }
}