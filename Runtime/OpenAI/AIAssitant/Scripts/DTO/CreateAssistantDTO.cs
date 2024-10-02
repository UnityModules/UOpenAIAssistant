
namespace OpenAI.Assistant
{
    [System.Serializable]
    public record CreateAssistantDTO
    {
        public string name { private set; get; }
        public string instructions { private set; get; }
        public string tools { private set; get; }
        public string model { private set; get; }

        public CreateAssistantDTO(string name, string instructions, string tools, string model)
        {
            this.name = name;
            this.instructions = instructions;
            this.tools = tools;
            this.model = model;
        }
    }
}