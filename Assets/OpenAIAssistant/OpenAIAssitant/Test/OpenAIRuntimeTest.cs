using UnityEngine;

namespace OpenAIAssistant
{

    public class OpenAIRuntimeTest : MonoBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private string threadId;
        [SerializeField] private string assistantId;
        [SerializeField] private string message;

        private void Awake() =>
            OpenAIAssistantProvider.Initialize(key);

        [ContextMenu("Create Thread")]
        private void CreateThread() =>
            OpenAIAssistantProvider.CreateThread((response) =>
            {
                Debug.Log(response.downloadHandler.text);
            });

        [ContextMenu("Create Assistant")]
        private void CreateAssistant() =>
            OpenAIAssistantProvider.CreateAssistant(new CreateAssistantDTO("Unity Tutor","Learn Unity to US","","gpt-4.5-turbo"),(response) =>
            {
                Debug.Log(response.downloadHandler.text);
            });

        [ContextMenu("Add Assistant To Thread")]
        private void AddAssistantToThread() =>
            OpenAIAssistantProvider.AddAssistantToThread(threadId,new AddAssistantToThreadDTO(assistantId,"Learn Unity to US"),(response) =>
            {
                Debug.Log(response.downloadHandler.text);
            });

        [ContextMenu("Add Message To Thread")]
        private void AddMessageToThread() =>
            OpenAIAssistantProvider.AddMessageToThread(threadId,new AddMessageToThreadDTO("user",message),(response) =>
            {
                Debug.Log(response.downloadHandler.text);
            });

        [ContextMenu("Add Message To Thread")]
        private void GetMessagesThread() =>
            OpenAIAssistantProvider.GetMessagesThread(threadId,(response) =>
            {
                Debug.Log(response.downloadHandler.text);
            });
    }
}