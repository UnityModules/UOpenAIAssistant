using System.Threading.Tasks;
using UnityEngine;

namespace USocketHandler.Test
{
    public class SocketHandlerRuntimeTest : MonoBehaviour
    {
        // private const string URL = "http://91.216.104.192:57342"; // Turkey Sever
        private const string URL = "http://45.66.245.113"; //UAE Server
        //private const string URL = "http://localhost:2000";

        private SocketHandler socketHandler;
        public async Task Start()
        {
            await socketHandler.ConnectToServer(URL);
        }
    }
}