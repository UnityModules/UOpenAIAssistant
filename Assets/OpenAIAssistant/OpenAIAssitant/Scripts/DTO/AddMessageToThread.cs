
namespace OpenAIAssistant
{
    [System.Serializable]
    public record AddMessageToThreadDTO
    {
        public string role { private set; get; }
        public string content { private set; get; }

        public AddMessageToThreadDTO(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }
}