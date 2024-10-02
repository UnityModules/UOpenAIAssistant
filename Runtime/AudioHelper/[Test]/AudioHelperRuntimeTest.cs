using UnityEngine;

namespace AudioHelper.Test
{
    public class AudioHelperRuntimeTest : MonoBehaviour
    {
        private const string AUDIO_FILE_NAME = "audioSavedFromAudioSaver_Unity.wav";

        [SerializeField] private AudioClip clip;
        [SerializeField] private string path;
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            gameObject.AddComponent<AudioListener>();

            Debug.Log("Application.persistentDataPath: "+Application.persistentDataPath);
        }

        private void Update() 
        {
            if(Input.GetKeyDown(KeyCode.S))
                AudioSaver.Save(clip, path+AUDIO_FILE_NAME);

            if(Input.GetKeyDown(KeyCode.Y))
                AudioSaver.Save(audioSource.clip, path+AUDIO_FILE_NAME);

            if(Input.GetKeyDown(KeyCode.L))
                StartCoroutine(AudioLoader.LoadAudioFromFile(path+AUDIO_FILE_NAME, PlayAudio));
        }

        private void PlayAudio(AudioClip clip)
        {
            Debug.Log("Loaded Audio");
            audioSource.clip = clip;
            audioSource.Play();
        }

        [ContextMenu("Play Audio")]
        private void PlayAudio() =>
            audioSource.Play();
    }
}
