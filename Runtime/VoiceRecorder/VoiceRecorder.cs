using UnityEngine;
using UnityEngine.Android;

namespace VoiceRecord
{
    public class VoiceRecorder
    {
        public bool IsRecording {private set; get;}
        public AudioClip RecordedClip { private set; get; }
        public string MicrophoneDevice { private set; get; }

        private VoiceStreamRecorder streamer;

        public AudioClip Record(int lengthSec = 10, VoiceStreamRecorder streamAudio = null)
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
            
            MicrophoneDevice = Microphone.devices[0];  // Use the first available microphone
            RecordedClip = Microphone.Start(MicrophoneDevice, false, lengthSec, 44100);  // Record for up to 10 seconds

            if(RecordedClip == null)
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

            (streamer = streamAudio)?.StartStream();

            return RecordedClip;
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

            Microphone.End(MicrophoneDevice);
            IsRecording = false;
            streamer?.StopStream();
            return RecordedClip;
            // AudioSaver.Save(recordedClip, filePath);
            // Debug.Log("Recording stopped. Audio saved at: " + filePath);
        }
    }
}