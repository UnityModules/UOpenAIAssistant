using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UHTTP;
using System.IO;

namespace OpenAI.TTS
{
    public class TextToSpeech : MonoBehaviour
    {
        private static TextToSpeech instance;
        private static string ApiKey = null;
        
        public static void SetAPIKey(string key) 
        {
            ApiKey = key;

            if(instance == null)
                instance = new GameObject(typeof(TextToSpeech).Name).AddComponent<TextToSpeech>();
        }
        
        private const string API_URL = "https://api.openai.com/v1/audio/speech";

        public static void Request(string input,UnityAction<AudioClip> callback, int maxToken = 100)
        {
            if(!instance)
                return;


  // convert this to body json:
              

            HTTPRequestData reqData = new HTTPRequestData()
            {
                URL = API_URL,
                Method = UnityWebRequest.kHttpVerbPOST,
                BodyJson = "{"
                        + "\"model\": \"tts-1\","
                        + "\"input\": \"" + input + "\","
                        + "\"voice\": \"alloy\""
                        + "}",
                Headers = new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>("Content-Type", "application/json"),
                    new KeyValuePair<string, string>("Authorization", "Bearer " + ApiKey)
                },
                HaveAuth = true
            };

            reqData.CreateRequest(Respone).Send();

            void Respone(UnityWebRequest request)
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

                instance.StartCoroutine(DownloadAudio(outputFilePath, callback));
            }
        }

        private static IEnumerator DownloadAudio(string url,UnityAction<AudioClip> callback = null)
        {
            UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
            yield return audioRequest.SendWebRequest();
            callback?.Invoke(audioRequest.result == UnityWebRequest.Result.Success ? DownloadHandlerAudioClip.GetContent(audioRequest) : null);
        }
    }
}