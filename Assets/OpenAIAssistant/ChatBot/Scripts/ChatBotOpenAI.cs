using System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace OpenAIAssistant.ChatBot
{
    public class ChatBotOpenAI
    {
        private const string CHAT_BOT_ROLE = "assistant";

        private ChatBotConfig config;

        // Config
        public ChatBotOpenAI(ChatBotConfig config, UnityAction threadCreated) =>
            SetConfig(config, threadCreated);

        public void SetConfig(ChatBotConfig config,UnityAction threadCreated) 
        {
            this.config = config;
            OpenAIAssistantProvider.Initialize(this.config.token);
            if(string.IsNullOrEmpty(config.threadId))
                CreateThread(threadCreated);
        }
        private void CreateThread(UnityAction threadCreated) 
        {
            Request("Create Thread",response => OpenAIAssistantProvider.CreateThread(response),Response);

            void Response(string json)
            {
                CreateThread thread = JsonConvert.DeserializeObject<CreateThread>(json);
                #if UNITY_EDITOR
                if(config.logInEditor) Debug.Log("Thread Created Successfully: \n" + thread.id);
                #endif
                config.threadId = thread.id;
                threadCreated?.Invoke();
            }
        }

        public void SendMessage(string message,UnityAction<Message> result)
        {
            Request("Send Message",response => OpenAIAssistantProvider.AddMessageToThread(config.threadId, new AddMessageToThreadDTO("user", message), response),Response);

            void Response(string json)
            {
              CoroutineRunner.Run(()=> RunAssistant(RunAssistantComplete), config.delayGetMessage);

                void RunAssistantComplete() =>
                    CoroutineRunner.Run(()=> GetLastMessage(result), config.delayGetMessage);
            }
        }
        private void RunAssistant(Action completeAction)
        {
            Request("Run Assistant",response => OpenAIAssistantProvider.RunAssistantToThread(config.threadId,config.assistantId, response),Response);

            void Response(string json) =>
                completeAction?.Invoke();
        }

        public void GetLastMessage(UnityAction<Message> result) =>
            GetMessages(messages => result?.Invoke(messages.Length > 0 ? messages[0] : null));
        public void GetMessages(UnityAction<Message[]> result)
        {
            void Send() =>
                Request("Get Messages",response => OpenAIAssistantProvider.GetMessagesThread(config.threadId, response),Response);

            Send();

            void Response(string json) 
            {
                Messages messages = JsonConvert.DeserializeObject<Messages>(json);
                var message = messages.messages[0];
                #if UNITY_EDITOR
                    if(config.logInEditor) message.Print();
                #endif

                var resetRequest = message.Contents.Length == 0 || message.Role != CHAT_BOT_ROLE;

                if (resetRequest)
                {
                    CoroutineRunner.Run(Send, config.delayGetMessage);
                    Debug.Log("Waiting for assistant message ...");
                    return;
                }

                result?.Invoke(messages.messages);
            }
        }

        private void Request(string reuqestName,UnityAction<Action<UnityWebRequest>> request,UnityAction<string> requestCompleted)
        {
            void SendRequest() =>
                request?.Invoke(Response);

            SendRequest();
            #if UNITY_EDITOR
            if(config.logInEditor) Debug.Log(reuqestName+" ...");
            #endif

            void Response(UnityWebRequest request)
            {
                if(request.result != UnityWebRequest.Result.Success)
                {
                    #if UNITY_EDITOR
                    if(config.logInEditor) Debug.Log(reuqestName+" Request Failed, trying agian .... \n"+ request.error);
                    #endif
                    SendRequest();
                    return;
                }
                
                #if UNITY_EDITOR
                if(config.logInEditor) Debug.Log(reuqestName+" Successfully, \n"+request.downloadHandler.text);
                #endif

                requestCompleted?.Invoke(request.downloadHandler.text);
            }
        }
    }
}