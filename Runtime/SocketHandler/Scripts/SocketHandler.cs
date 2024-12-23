using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using SocketIOClient.Transport;
using UnityEngine;
using Socket_IO = SocketIOClient.SocketIO;

namespace USocketHandler
{
    public enum EventName
    {
        ConnectNewInstruction,
        StreamVoiceChunk,
        StreamVoiceEnd,
        AIResponseAudioChunk,
        Interrupt,
        ConnectionHasBeenReset
    }

    public class SocketHandler : MonoBehaviour
    {
        public static Socket_IO Socket { get; private set; }

        public async Task ConnectToServer(string url)
        {
            Socket = new Socket_IO(url, CreateSocketIOOptionsWithQuery("", ""));
            Socket.JsonSerializer = new NewtonsoftJsonSerializer();
            Socket.OnDisconnected += async (sender, s) => await Socket.ConnectAsync();
            Socket.OnError += async (sender, s) => await Socket.ConnectAsync();

            try
            {
                Debug.Log("Try To Establish Connection");
                StartListening();
                await Socket.ConnectAsync();
                Debug.Log("Socket Connected Successfully");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private static SocketIOOptions CreateSocketIOOptionsWithQuery(string Key, string Value)
        {
            var options = new SocketIOOptions
            {
                Query = new List<KeyValuePair<string, string>> { new(Key, Value), }.ToArray(),
                Transport = TransportProtocol.WebSocket
            };

            return options;
        }

        #region Actions

        public Action<SocketIOResponse> AIResponseAudioChunk { get; set; }
        public Action<SocketIOResponse> ConnectNewInstruction { get; set; }
        public Action<SocketIOResponse> ConnectionHasBeenReset { get; set; }

        #endregion

        private void StartListening()
        {
            Socket.On(EventName.AIResponseAudioChunk.ToEnumString(), r => InvokeAction(AIResponseAudioChunk, r));
            Socket.On(EventName.ConnectNewInstruction.ToEnumString(), r => InvokeAction(ConnectNewInstruction, r));
            Socket.On(EventName.ConnectionHasBeenReset.ToEnumString(), r => InvokeAction(ConnectionHasBeenReset, r));
        }

        private static void InvokeAction(Action<SocketIOResponse> action, SocketIOResponse response)
        {
            action?.Invoke(response);
            response.CallbackAsync("Received");
        }
    }

    public static class EnumExtensions
    {
        public static string ToEnumString(this Enum value) => Enum.GetName(value.GetType(), value);
    }
}