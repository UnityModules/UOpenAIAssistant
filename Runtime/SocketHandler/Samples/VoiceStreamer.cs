using Newtonsoft.Json.Linq;
using VoiceRecord;

namespace USocketHandler.Test
{
    public class VoiceRecorderStreamer : VoiceStreamRecorder
    {
        public VoiceRecorderStreamer(VoiceRecorder recorder) =>
            this.recorder = recorder;

        public override void StopStream() =>
            SocketHandler.Socket.EmitAsync(EventName.StreamVoiceEnd.ToEnumString());

        protected override void SendingAudioToServer(byte[] audioData) =>
            SocketHandler.Socket.EmitAsync(EventName.StreamVoiceChunk.ToEnumString(),
                new JObject { ["data"] = audioData });
    }
}