
namespace OpenAIAssistant
{
    [System.Serializable]
    public record AddAssistantToThreadDTO
    {
        public string assistant_id { private set; get; }
        public string instructions { private set; get; }

        public AddAssistantToThreadDTO(string assistant_id, string instructions)
        {
            this.assistant_id = assistant_id;
            this.instructions = instructions;
        }
    }
}