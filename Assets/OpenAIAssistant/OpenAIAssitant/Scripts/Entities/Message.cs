using Newtonsoft.Json;
using UnityEngine;

namespace OpenAIAssistant.ChatBot
{
    [System.Serializable]
    public partial class Messages
    {
        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("data")]
        public Message[] messages { get; set; }

        [JsonProperty("first_id")]
        public string FirstId { get; set; }

        [JsonProperty("last_id")]
        public string LastId { get; set; }

        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
    }

    [System.Serializable]
    public partial class Message
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }

        [JsonProperty("assistant_id")]
        public string AssistantId { get; set; }

        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }

        [JsonProperty("run_id")]
        public string RunId { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        [JsonProperty("content")]
        public Content[] Contents { get; set; }

        [JsonProperty("file_ids")]
        public object[] FileIds { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        public void Print()
        {
            string str = "";

            for (int i = 0;i < Contents.Length; i++)
                str += Contents[i].Type + ": "+ Contents[i].Text.Value + "\n";
            Debug.Log(str);
        }
    }

    [System.Serializable]
    public partial class Content
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public Text Text { get; set; }
    }

    [System.Serializable]
    public partial class Text
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("annotations")]
        public Annotation[] Annotations { get; set; }
    }

    [System.Serializable]
    public partial class Annotation
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("start_index")]
        public long StartIndex { get; set; }

        [JsonProperty("end_index")]
        public long EndIndex { get; set; }

        [JsonProperty("file_citation")]
        public FileCitation FileCitation { get; set; }
    }

    [System.Serializable]
    public partial class FileCitation
    {
        [JsonProperty("file_id")]
        public string FileId { get; set; }

        [JsonProperty("quote")]
        public string Quote { get; set; }
    }

    [System.Serializable]
    public partial class Metadata
    {
    }
}