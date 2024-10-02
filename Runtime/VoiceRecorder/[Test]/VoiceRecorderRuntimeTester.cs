using UnityEngine;

namespace VoiceRecord.Test
{
    public class VoiceRecorderRuntimeTester : MonoBehaviour
    {
        private VoiceRecorder recorder;
        private AudioSource audioSource;

        private void Awake() 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            gameObject.AddComponent<AudioListener>();
        }

        private void Start() =>
            recorder = new VoiceRecorder();

        private void Update() 
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
                recorder.Record();
            if(Input.GetKeyDown(KeyCode.Alpha2))
                {
                    audioSource.clip = recorder.Stop();
                    audioSource.Play();
                }

            if(Input.GetKeyDown(KeyCode.Alpha3))
                audioSource.Play();
        }
    }
}