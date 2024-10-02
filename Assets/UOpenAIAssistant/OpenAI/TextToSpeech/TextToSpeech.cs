using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

namespace OpenAI.TTS
{
    public static class TextToSpeech
    {
        private static string ApiKey = null;
        
        public static void SetAPIKey(string key) =>
            ApiKey = key;
        
        private const string API_URL = "https://api.openai.com/v1/audio/speech";

        public static void Request(string input,Action<AudioClip> callback, int maxToken = 100)
        {
            UnityWebRequest request = UnityWebRequest.Post(API_URL,"");
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(
                "{"
                + "\"model\": \"tts-1\","
                + "\"input\": \"" + input + "\","
                + "\"voice\": \"alloy\""
                + "}"));

            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + ApiKey);
            request.SendWebRequest().completed += (ao) => Respone();

            void Respone()
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    #if UNITY_EDITOR
                    Debug.Log(request.error);
                    #endif
                    callback(null);
                    return;
                }

                string outputFilePath = Path.Combine(Application.persistentDataPath, "speech.wav");
                File.WriteAllBytes(outputFilePath, request.downloadHandler.data);
                
                #if UNITY_EDITOR
                Debug.Log(outputFilePath);
                #endif
                DownloadAudio(outputFilePath, callback);
            }
        }

        public static void DownloadAudio(string url,Action<AudioClip> callback = null)
        {
            UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip("file:///" +url, AudioType.MPEG);
            audioRequest.SendWebRequest().completed += (ao) => Respone();

            void Respone()
            {
                var clip = DownloadHandlerAudioClip.GetContent(audioRequest);
                callback?.Invoke(clip);
            }
        }
    }
}