using System;
using System.Collections;
using UnityEngine;

namespace VoiceRecord
{
    public abstract class VoiceStreamRecorder
    {
        protected VoiceRecorder recorder;

        protected const int SAMPLE_RATE = 24000;
        protected const int CHUNK_SIZE = 2400;
        protected int _lastSamplePosition;
        protected Coroutine streamer;

        public virtual void StartStream()
        {
            streamer = CoroutineRunner.Run(StreamAudio());
            IEnumerator StreamAudio()
            {
                while (recorder.IsRecording)
                {
                    var currentPosition = Microphone.GetPosition(recorder.MicrophoneDevice);
                    var samplesToSend = currentPosition - _lastSamplePosition;

                    if (samplesToSend >= CHUNK_SIZE)
                    {
                        var audioData = new float[CHUNK_SIZE];
                        recorder.RecordedClip.GetData(audioData, _lastSamplePosition);

                        var int16Audio = ConvertFloatArrayToInt16(audioData);
                        SendAudio(int16Audio);

                        _lastSamplePosition += CHUNK_SIZE;
                    }

                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        public virtual void StopStream()
        {
            if(streamer != null)
                CoroutineRunner.Stop(streamer);
        }

        protected static short[] ConvertFloatArrayToInt16(float[] floatArray)
        {
            var int16Array = new short[floatArray.Length];
            for (var i = 0; i < floatArray.Length; i++)
                int16Array[i] = (short)(floatArray[i] * short.MaxValue);

            return int16Array;
        }

        protected void SendAudio(short[] audioData)
        {
            var byteArray = new byte[audioData.Length * sizeof(short)];
            Buffer.BlockCopy(audioData, 0, byteArray, 0, byteArray.Length);

            SendingAudioToServer(byteArray);
        }
        protected virtual void SendingAudioToServer(byte[] audioData) {}
    }
}