using UnityEngine;
using UnityEngine.Android;

namespace VoiceRecord
{
    public class VoiceRecorder
    {
        public bool IsRecording {private set; get;}
        private AudioClip recordedClip;
        private string microphoneDevice;

        public AudioClip Record(int lengthSec = 10)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
                return null;
            }

            if (Microphone.devices.Length == 0 || IsRecording)
            {
                #if UNITY_EDITOR
                Debug.Log("No microphone detected or already recording.");
                #endif
                return null;
            }
            
            microphoneDevice = Microphone.devices[0];  // Use the first available microphone
            recordedClip = Microphone.Start(microphoneDevice, false, lengthSec, 44100);  // Record for up to 10 seconds

            if(recordedClip == null)
            {
                #if UNITY_EDITOR
                Debug.Log("Failed to record audio.");
                #endif
                return null;
            }

            IsRecording = true;
            #if UNITY_EDITOR
            Debug.Log("Recording started...");
            #endif

            return recordedClip;
        }
        public AudioClip Stop()
        {
            if (!IsRecording)
            {
                Debug.Log("Not currently recording.");
                return null;
            }

            #if UNITY_EDITOR
            Debug.Log("Recording Complete");
            #endif

            Microphone.End(microphoneDevice);
            IsRecording = false;
            return recordedClip;
            // AudioSaver.Save(recordedClip, filePath);
            // Debug.Log("Recording stopped. Audio saved at: " + filePath);
        }
    }
}