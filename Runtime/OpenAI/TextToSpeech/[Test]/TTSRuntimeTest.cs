using UnityEngine;

namespace OpenAI.TTS
{
    public class TTSRuntimeTest : MonoBehaviour
    {
        [SerializeField] private string apiKey;
        [SerializeField] private string message = "Hello World!";
        private AudioSource audioSource;

        [SerializeField] private string audioLocalPath = "";

        private void Awake()
        {
            if(!(audioSource = gameObject.GetComponent<AudioSource>()))
                audioSource = gameObject.AddComponent<AudioSource>();

            TextToSpeech.SetAPIKey(apiKey);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
                Apply();

            if(Input.GetKeyDown(KeyCode.L))
               TextToSpeech.DownloadAudio(audioLocalPath,PlayAudio);

        }

        private void Apply()
        {
            Debug.Log("Send Request: "+message);
            TextToSpeech.Request(message,PlayAudio, 100);
        }

        private void PlayAudio(AudioClip clip)
        {
                if(!clip)
                {
                    Debug.Log("Failed to download audio");
                    return;
                }
                audioSource.clip = clip;
                audioSource.Play();
        }
    }
}