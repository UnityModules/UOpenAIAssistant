
namespace OpenAIAssistant
{
    [System.Serializable]
    public record RunAssistantToThreadDTO
    {
        public string assistant_id { private set; get; }
        public RunAssistantToThreadDTO(string assistant_id)
        {
            this.assistant_id = assistant_id;
        }
    }
}