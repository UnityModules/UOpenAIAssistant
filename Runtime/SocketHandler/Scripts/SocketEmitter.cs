using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace USocketHandler
{
    public class SocketEmitter
    {
        private static SemaphoreSlim SemaphoreSlim { get; set; } = new(1, 1);

        private string Event { get; set; }

        [CanBeNull] private string Body { get; set; }

        private bool AttemptToEmit { get; set; }

        public SocketEmitter(EventName eventName) =>
            Event = eventName.ToEnumString();

        public SocketEmitter SetBody(string body)
        {
            Body = body;
            return this;
        }

        public SocketEmitter WithAttempt()
        {
            AttemptToEmit = true;
            return this;
        }

        public async void Emit()
        {
            await SemaphoreSlim.WaitAsync();

            try
            {
                if (SocketHandler.Socket.Disconnected)
                    Debug.LogError("Socket disconnected");

                await SocketHandler.Socket.EmitAsync(Event, Body ?? string.Empty);
            }
            catch (Exception e)
            {
                if (AttemptToEmit is false) return;

                Debug.LogError(e);
                await Task.Delay(1000);
                Emit();
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}