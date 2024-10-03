using System;
using UnityEngine;
using VoiceRecord;
using OpenAI.Assistant.ChatBot;
using OpenAI.STT;
using OpenAI.TTS;
using AudioHelper;

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
    }
}
