
namespace OpenAI.Assistant
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

    [System.Serializable]
    public record RunAssistantToThreadStreamDTO
    {
        public string assistant_id { private set; get; }
        public bool stream { private set; get; }
        public RunAssistantToThreadStreamDTO(string assistant_id)
        {
            this.assistant_id = assistant_id;
            stream = true;
        }
    }
}