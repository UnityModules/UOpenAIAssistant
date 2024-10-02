using UnityEngine;

namespace OpenAI.Assistant.ChatBot
{
    public class ChatBotOpenAIRuntimeTest : MonoBehaviour
    {
        [SerializeField,TextArea] private string message;
        [SerializeField] private ChatBotConfig config;
        private ChatBot chatBot;

        private void Awake() =>
            chatBot = new ChatBot(config,() => Debug.Log("<color=green>Chat bot Initialized</color>"));

        [ContextMenu("Send Message")]
        public void SendMessageLocal() =>
            chatBot.SendMessage(message, messageResp => { Debug.Log("[STREAM] your message: "+message+"\n answer: <color=green>"+ messageResp.Contents[0].Text.Value+"</color>"); },messageResp => { Debug.Log("your message: "+message+"\n answer: <color=green>"+ messageResp.Contents[0].Text.Value+"</color>"); });

        [ContextMenu("Get Messages")]
        public void GetMessages() =>
            chatBot.GetMessages(messages => { Debug.Log("message Count : "+messages.Length); });

        [ContextMenu("Set Config")]
        public void SetConfig() =>
            chatBot.SetConfig(config,() => Debug.Log("Chat bot ReConfiged"));
    }
}