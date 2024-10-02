using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;

namespace OpenAI.Assistant
{
    public static class OpenAIAssistantProvider
    {
        // Based On AI Assistant Open AI Doc: https://platform.openai.com/docs/assistants/overview?context=without-streaming&lang=curl

        private const string BASE_URL = "https://api.openai.com/v1/";
        
        private static string AccessToken;

        public static void SetAccessToken(string token) =>
            AccessToken = token;
        public static void CreateAssistant(CreateAssistantDTO data, Action<UnityWebRequest> callback) =>
            Request("assistants",UnityWebRequest.kHttpVerbPOST,callback,JsonConvert.SerializeObject(data));
        public static void CreateThread(Action<UnityWebRequest> callback) =>
            Request("threads",UnityWebRequest.kHttpVerbPOST,callback);
        public static void AddMessageToThread(string threadId, AddMessageToThreadDTO data, Action<UnityWebRequest> callback) =>
            Request("threads/" + threadId + "/messages",UnityWebRequest.kHttpVerbPOST,callback,JsonConvert.SerializeObject(data));
        public static void AddAssistantToThread(string threadId, AddAssistantToThreadDTO data, Action<UnityWebRequest> callback) =>
            Request("threads/" + threadId + "/runs",UnityWebRequest.kHttpVerbPOST,callback,JsonConvert.SerializeObject(data));
        public static void RunAssistantToThread(string threadId, string assistantId, Action<UnityWebRequest> callback) =>
            Request("threads/" + threadId + "/runs",UnityWebRequest.kHttpVerbPOST,callback,JsonConvert.SerializeObject(new RunAssistantToThreadDTO(assistantId)));
        public static void GetMessagesThread(string threadId, Action<UnityWebRequest> callback) =>
            Request("threads/" + threadId + "/messages",UnityWebRequest.kHttpVerbGET,callback);

        public static void RunAssistantToThreadStream(string threadId, string assistantId, Action<UnityWebRequest> callbackStream,Action<UnityWebRequest> callback) =>
            CoroutineRunner.Run(RequestStream("threads/" + threadId + "/runs",UnityWebRequest.kHttpVerbPOST,callbackStream,callback,JsonConvert.SerializeObject(new RunAssistantToThreadStreamDTO(assistantId))));

        private static UnityWebRequest BaseRequest(string appendURL,string Method, string bodyJson)
        {
            UnityWebRequest request = new UnityWebRequest(BASE_URL + appendURL, Method);

            if (!string.IsNullOrEmpty(bodyJson))
                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(bodyJson));

            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("OpenAI-Beta", "assistants=v2");
            request.SetRequestHeader("Authorization", "Bearer " + AccessToken);
            request.SetRequestHeader("Content-Type", "application/json");
            return request;
        }

        private static void Request(string appendURL,string Method, Action<UnityWebRequest> callback, string bodyJson = "")
        {
            var request = BaseRequest(appendURL,Method,bodyJson);
            request.SendWebRequest().completed += (ao) => callback?.Invoke(request);
            //request.Dispose();
        }

        private static IEnumerator RequestStream(string appendURL, string method,Action<UnityWebRequest> streamCallback, Action<UnityWebRequest> callback, string bodyJson = "")
        {
            var request = BaseRequest(appendURL,method,bodyJson);
            request.SetRequestHeader("Accept", "text/event-stream");
            request.SendWebRequest();

            while(!request.isDone)
            {
                streamCallback?.Invoke(request);
                yield return new WaitForEndOfFrame();
            }

            callback?.Invoke(request);
            //request.Dispose();
        }
    }
}
