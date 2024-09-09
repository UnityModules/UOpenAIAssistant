using UnityEngine;

namespace OpenAI.TTS
{
    public class TTSRuntimeTest : MonoBehaviour
    {
        [SerializeField] private string apiKey;
        private AudioSource audioSource;

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
        }

        private void Apply()
        {
            Debug.Log("Send Request: "+"Hello World");
            TextToSpeech.Request("Hello World", (clip) =>
            {
                if(!clip)
                {
                    Debug.Log("Failed to download audio");
                    return;
                }
                audioSource.clip = clip;
                audioSource.Play();
            }, 100);
        }
    }
}