using System;
using UnityEngine;
using VoiceRecord;
using OpenAI.Assistant.ChatBot;
using OpenAI.STT;
using OpenAI.TTS;
using AudioHelper;
using System.Collections.Generic;
using USocketHandler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenAI.VoiceBot
{
    public class VoiceBot
    {
        private ChatBot chatBot;
        private VoiceRecorder recorder;

        public VoiceBot(ChatBotConfig config)
        {
            recorder = new VoiceRecorder();
            chatBot = new ChatBot(config);
            TextToSpeech.SetAPIKey(config.token);
            SpeechToText.SetAPIKey(config.token);
        }

        public void RecordVoice() 
        {
            if(recorder.IsRecording)
                return;

            recorder.Record();
        }

        public void CancelRecord() =>
            recorder.Stop();

        public void Send(Action<AudioClip> result,string commandBeforeMessage = "") 
        {
            if(!recorder.IsRecording)
                return;

            var clip = recorder.Stop();
            string audioPath = Application.persistentDataPath + "/recorded.wav";
            AudioSaver.Save(clip,audioPath);
            SpeechToText.Request(audioPath,OnRecognized);

            void OnRecognized(string text)
            {
                Debug.Log("Recognized: " + text);
                chatBot.SendMessage(commandBeforeMessage+text,OnChatBotAnswerStream,OnChatBotAnswer);
            }

            void OnChatBotAnswerStream(Message message)
            {
                Debug.Log("Bot Answer Stream: " +message.Contents[0].Text.Value);
                TextToSpeech.Request(message.Contents[0].Text.Value,result);
            }


            void OnChatBotAnswer(Message message)
            {
                Debug.Log("Bot Answer: " +message.Contents[0].Text.Value);
                TextToSpeech.Request(message.Contents[0].Text.Value,result);
            }
        }           
    
        public void SendStream(short[] data, bool isComplete,Action<AudioClip> clipToSpeak, Action<float> clipLength)
        {
            if (!recorder.IsRecording) return;

            CancelRecord();

            var audioBuffer = new List<short>();
            AudioClip streamingClip;
            var isPlaying = false;
            const int minBufferSize = 24000;
            const int sampleRate = 24000;
            var totalAudioLength = 0;

            OnGetAudio(data,isComplete);

            void OnGetAudio(short[] data, bool isComplete = true)
            {
                totalAudioLength += data.Length;

                audioBuffer.AddRange(data);

                if (!isPlaying && audioBuffer.Count >= minBufferSize)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        if (isPlaying) return;

                        streamingClip = AudioClip.Create("StreamedAudio",
                            44100 * 300,
                            1,
                            sampleRate,
                            true,
                            OnAudioRead);

                        clipToSpeak(streamingClip);
                        isPlaying = true;
                    });
                }

                if (!isComplete)
                    return;

                var durationInSeconds = (float)totalAudioLength / sampleRate;
                clipLength(durationInSeconds);
            }
            void OnAudioRead(float[] data)
            {
                for (var i = 0; i < data.Length; i++)
                    if (i < audioBuffer.Count)
                        data[i] = audioBuffer[i] / 32768f;
                    else
                        data[i] = 0;

                audioBuffer.RemoveRange(0, Mathf.Min(data.Length, audioBuffer.Count));
            }
        }

    }
}
