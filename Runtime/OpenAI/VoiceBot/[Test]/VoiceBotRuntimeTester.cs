using OpenAI.Assistant.ChatBot;
using UnityEngine;

namespace OpenAI.VoiceBot.Test
{
    public class VoiceBotRuntimeTester : MonoBehaviour
    {
        [SerializeField] private ChatBotConfig config;
        private VoiceBot voiceBot;
        private AudioSource audioSource;

        private void Awake() 
        {
            voiceBot = new VoiceBot(config);
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
                voiceBot.RecordVoice();

            if(Input.GetKeyDown(KeyCode.Alpha2))
                voiceBot.Send(PlayAudio);
        }

        private void PlayAudio(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}