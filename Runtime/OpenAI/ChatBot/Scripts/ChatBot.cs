using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static SentenceFinder;

namespace OpenAI.Assistant.ChatBot
{
    public class ChatBot
    {
        private const string CHAT_BOT_ROLE = "assistant";

        private ChatBotConfig config;

        // Config
        public ChatBot(ChatBotConfig config, Action threadCreated = null) =>
            SetConfig(config, threadCreated);

        public void SetConfig(ChatBotConfig config,Action threadCreated) 
        {
            this.config = config;
            OpenAIAssistantProvider.SetAccessToken(this.config.token);
            if(string.IsNullOrEmpty(config.threadId))
                CreateThread(threadCreated);
        }
        private void CreateThread(Action threadCreated) 
        {
            Request("Create Thread",response => OpenAIAssistantProvider.CreateThread(response),Response);

            void Response(string json)
            {
                CreateThread thread = JsonConvert.DeserializeObject<CreateThread>(json);
                #if UNITY_EDITOR
                if(config.logInEditor) Debug.Log("Thread Successfully convert to entity" + thread.id);
                #endif
                config.threadId = thread.id;
                threadCreated?.Invoke();
            }
        }

        public void SendMessage(string message,Action<Message> stream,Action<Message> result)
        {
            Request("Send Message",response => OpenAIAssistantProvider.AddMessageToThread(config.threadId, new AddMessageToThreadDTO("user", message), response),SendMessageResponse);

            void SendMessageResponse(string json)
            {
               CoroutineRunner.Run(()=> RunAssistant(RunAssistantResponse), config.delayGetMessage);

                void RunAssistantResponse(string json) =>
                    CoroutineRunner.Run(()=> GetLastMessage(result), config.delayGetMessage);
            }
        }
        private void RunAssistantStream(Action<Message> stream,Action<Message> completeAction)
        {
            int sentenceIndex = 0;
            void SendSentence(UnityWebRequest req,bool lastStream = false)
            {
                string[] CleanGetMessageStreamData(byte[] data)
                {
                    List<string> list = new();

                    string getString = Encoding.UTF8.GetString(data);
                    string[] lines = getString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string line in lines)
                    {
                        // Extract the JSON substring
                        string jsonSubstring = line.Substring(line.IndexOf("{"), line.LastIndexOf("}") - line.IndexOf("{") + 1);
                        JObject jsonObject = JObject.Parse(jsonSubstring);
                        if(jsonObject["t"] != null)
                            list.Add(jsonObject["t"].ToString());
                    }

                    return list.ToArray();
                }

                Debug.Log("[STREAMING]..."+req.downloadProgress);

                if (req.downloadProgress <= 0) 
                    return;
                Debug.Log("[STREAM] req.downloadHandler.data: \n"+req.downloadHandler.text);
                var sentences = CreateSentences(CleanGetMessageStreamData(req.downloadHandler.data));

                var count = sentences.Length - (lastStream ? 0 : 1);

                if(count > sentenceIndex)
                {
                    //callbackSentence?.Invoke(sentences[sentenceIndex]);
                    sentenceIndex++;
                }
            }

            void Stream(UnityWebRequest req) =>
                SendSentence(req);
            void Complete(UnityWebRequest req)
            {
                Debug.Log("[COMPLETE]" + req.downloadHandler.text);
                // if(req.result != UnityWebRequest.Result.Success)
                // {
                //     callback?.Invoke(req);
                //     return;
                // }

                // SendSentence(req,true);
                // callback?.Invoke(req);
            }

            OpenAIAssistantProvider.RunAssistantToThreadStream(config.threadId,config.assistantId, Stream,Complete);

            //Request("Send Message",response => OpenAIAssistantProvider.RunAssistantToThread(config.threadId,config.assistantId, response),Response);

            // void Response(string json) =>
            //     completeAction?.Invoke();
        }

        private void RunAssistant(Action<string> Response) =>
            Request("Send Message",response => OpenAIAssistantProvider.RunAssistantToThread(config.threadId,config.assistantId, response),Response);

        public void GetLastMessage(Action<Message> result) =>
            GetMessages(messages => result?.Invoke(messages.Length > 0 ? messages[0] : null));
        public void GetMessages(Action<Message[]> result)
        {
            void Send() =>
                Request("Get Message",response => OpenAIAssistantProvider.GetMessagesThread(config.threadId, response),Response);

            Send();

            void Response(string json) 
            {
                Messages messages = JsonConvert.DeserializeObject<Messages>(json);
                var message = messages.messages[0];
                #if UNITY_EDITOR
                    if(config.logInEditor) message.Print();
                #endif

                var waiting = message.Contents.Length == 0 || message.Role != CHAT_BOT_ROLE;

                if (waiting)
                {
                    CoroutineRunner.Run(Send, config.delayGetMessage);
                    Debug.Log("Waiting for assistant message ...");
                    return;
                }

                result?.Invoke(messages.messages);
            }
        }
    
        private void Request(string reuqestName,UnityAction<Action<UnityWebRequest>> request,Action<string> requestCompleted)
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