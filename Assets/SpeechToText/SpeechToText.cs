using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.IO;
using UHTTP;
using System.Collections.Generic;

namespace OpenAI.STT
{
    public class SpeechToText : MonoBehaviour
    {
        private static string ApiKey = null;

        public static void SetAPIKey(string key) =>
            ApiKey = key;

        private const string API_URL = "https://api.openai.com/v1/audio/transcriptions";

        public static void Request(string audioFilePath, UnityAction<string> callback)
        {
            if (string.IsNullOrEmpty(ApiKey))
                return;

            if (!File.Exists(audioFilePath))
            {
                Debug.LogError("Audio file does not exist: " + audioFilePath);
                callback(null);
                return;
            }

            UploadAudioWithoutUHTTP(audioFilePath, callback);
        }

        private static void UploadAudioWithUHTTP(string filePath, UnityAction<string> callback)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            HTTPRequestData reqData = new HTTPRequestData()
            {
                URL = API_URL,
                Method = UnityWebRequest.kHttpVerbPOST,
                FormBinaryData = new FormBinaryData("file", fileData, Path.GetFileName(filePath), "audio/wav"),
                Headers = new List<KeyValuePair<string, string>>(){
                    new KeyValuePair<string, string>("Authorization", "Bearer " + ApiKey),
                    new KeyValuePair<string, string>("Content-Type", "multipart/form-data")
                }
            };

            reqData.CreateRequest(Respone).Send();

            void Respone(UnityWebRequest request)
            {
                if (request.result == UnityWebRequest.Result.Success)
                    callback?.Invoke(ParseResponse(request.downloadHandler.text));
                else
                {
                    Debug.LogError("Error: " + request.error);
                    callback?.Invoke(null);
                }
            }
        }

        private static void UploadAudioWithoutUHTTP(string filePath, UnityAction<string> callback)
        {
            byte[] fileData = File.ReadAllBytes(filePath);

            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("model", "whisper-1"));
            formData.Add(new MultipartFormFileSection("file", fileData, Path.GetFileName(filePath), "audio/wav"));
            
            UnityWebRequest request = UnityWebRequest.Post(API_URL, formData);
            request.SetRequestHeader("Authorization", "Bearer " + ApiKey);

            request.SendWebRequest().completed += (asyncOperation) => Respone();

            void Respone()
            {
                if (request.result == UnityWebRequest.Result.Success)
                    callback?.Invoke(ParseResponse(request.downloadHandler.text));
                else
                {
                    Debug.LogError("Error: " + request.error);
                    callback?.Invoke(null);
                }
            }
        }

        private static string ParseResponse(string jsonResponse) =>
            JsonUtility.FromJson<TranscriptionResponse>(jsonResponse)?.text ?? null;

        [System.Serializable]
        private class TranscriptionResponse
        {
            public string text;
        }
    }
}
