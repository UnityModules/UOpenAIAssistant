using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI.VoiceBot;

namespace USocketHandler.Test
{
    public static class VoiceBotStreamer 
    {
        public static void EndStream(VoiceBot voiceBot, SocketHandler socketHandler)
        {
            socketHandler.AIResponseAudioChunk = response =>
            {
                var deserializedData =
                    JsonConvert.DeserializeObject<AIResponseAudioChunk>(response.GetValue<JObject>().ToString());

                voiceBot.SendStream(deserializedData.Audio, deserializedData.Status == "completed",null,null);
            };
        }
    }
    
    [Serializable]
    public record AIResponseAudioChunk(
        string Id,
        string Status,
        short[] Audio,
        string Transcript)
    {
        [JsonProperty("id")] public string Id { get; } = Id;
        [JsonProperty("status")] public string Status { get; } = Status;

        [JsonProperty("audio"), JsonConverter(typeof(AudioArrayConverter))]
        public short[] Audio { get; } = Audio;

        [JsonProperty("transcript")] public string Transcript { get; } = Transcript;
    }
    public class AudioArrayConverter : JsonConverter<short[]>
        {
            public override short[] ReadJson(JsonReader reader, Type objectType, short[] existingValue,
                bool hasExistingValue,
                JsonSerializer serializer)
            {
                var values = JObject.Parse(reader.Value.ToString()).Properties().Values<short>();
                return values.ToArray();
            }

            public override void WriteJson(JsonWriter writer, short[] value, JsonSerializer serializer)
            {
            }
        }
}
