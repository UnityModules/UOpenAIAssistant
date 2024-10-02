using Newtonsoft.Json;

namespace OpenAI.Assistant.ChatBot
{
    [System.Serializable]
    public record CreateThread
    {
        public string id { get; set; }

        [JsonProperty("object")]
        public string @object { get; set; }
        public int created_at { get; set; }
        public object metadata { get; set; }

        public CreateThread(string id, string @object, int created_at, object metadata)
        {
            this.id = id;
            this.@object = @object;
            this.created_at = created_at;
            this.metadata = metadata;
        }
    }
}