using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UHTTP;

namespace OpenAIAssistant
{
    public static class OpenAIAssistantProvider
    {
        // Based On AI Assistant Open AI Doc: https://platform.openai.com/docs/assistants/overview?context=without-streaming&lang=curl

        private const string baseURL = "https://api.openai.com/v1/";

        private const string ACCESS_TOKEN = "OPEN-AI-KEY";

        private static List<KeyValuePair<string, string>> Headers = new List<KeyValuePair<string, string>>()
        {
            HTTPHeaderHelper.ContentType,
            new KeyValuePair<string, string>("OpenAI-Beta", "assistants=v1")
        };

        public static void Initialize(string token = null) =>
            JWTTokenResolver.SetAccessToken(token ?? ACCESS_TOKEN);


        public static void CreateAssistant(CreateAssistantDTO data, Action<UnityWebRequest> callback) =>
            new HTTPRequestData()
            {
                URL = baseURL + "assistants",
                Method = UnityWebRequest.kHttpVerbPOST,
                BodyJson = JsonConvert.SerializeObject(data),
                Headers = Headers,
                HaveAuth = true
            }.CreateRequest(callback).Send();

        public static void CreateThread(Action<UnityWebRequest> callback) =>
            new HTTPRequestData()
            {
                URL = baseURL + "threads",
                Method = UnityWebRequest.kHttpVerbPOST,
                Headers = Headers,
                HaveAuth = true
            }.CreateRequest(callback).Send();

        public static void AddMessageToThread(string threadId, AddMessageToThreadDTO data, Action<UnityWebRequest> callback) =>
            new HTTPRequestData()
            {
                URL = baseURL + "threads/" + threadId + "/messages",
                Method = UnityWebRequest.kHttpVerbPOST,
                BodyJson = JsonConvert.SerializeObject(data),
                Headers = Headers,
                HaveAuth = true
            }.CreateRequest(callback).Send();


        public static void AddAssistantToThread(string threadId, AddAssistantToThreadDTO data, Action<UnityWebRequest> callback) =>
            new HTTPRequestData()
            {
                URL = baseURL + "threads/" + threadId + "/runs",
                Method = UnityWebRequest.kHttpVerbPOST,
                BodyJson = JsonConvert.SerializeObject(data),
                Headers = Headers,
                HaveAuth = true
            }.CreateRequest(callback).Send();

        public static void RunAssistantToThread(string threadId, string assistantId, Action<UnityWebRequest> callback) =>
            new HTTPRequestData()
            {
                URL = baseURL + "threads/" + threadId + "/runs",
                Method = UnityWebRequest.kHttpVerbPOST,
                BodyJson = JsonConvert.SerializeObject(new RunAssistantToThreadDTO(assistantId)),
                Headers = Headers,
                HaveAuth = true
            }.CreateRequest(callback).Send();


        public static void GetMessagesThread(string threadId, Action<UnityWebRequest> callback) =>
            new HTTPRequestData()
            {
                URL = baseURL + "threads/" + threadId + "/messages",
                Method = UnityWebRequest.kHttpVerbGET,
                Headers = Headers,
                HaveAuth = true
            }.CreateRequest(callback).Send();
    }
}
