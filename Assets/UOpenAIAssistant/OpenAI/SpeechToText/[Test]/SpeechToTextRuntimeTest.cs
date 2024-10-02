using UnityEngine;
using System.IO;

namespace OpenAI.STT
{
    public class SpeechToTextRuntimeTest : MonoBehaviour
    {
        [SerializeField] private string ApiKey;
        [SerializeField] private string audioFilePath;

        private void Start()
        {
            SpeechToText.SetAPIKey(ApiKey);
            // string audioFilePath = Path.Combine(Application.persistentDataPath, "speech.wav");
            // SpeechToText.Request(audioFilePath, OnTranscriptionReceived);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
                SpeechToText.Request(audioFilePath, OnTranscriptionReceived);
        }

        void OnTranscriptionReceived(string transcription)
        {
            if (!string.IsNullOrEmpty(transcription))
                Debug.Log("Transcription: " + transcription);
            else
                Debug.Log("Transcription failed.");
        }
    }
}