using UnityEngine;
using System.IO;
using System;
using System.Collections;

namespace AudioHelper
{
    public static class AudioLoader
    {
        public static AudioClip ToAudioClip(byte[] wavFile)
        {
            int headerOffset = 44; // WAV header size
            int dataSize = wavFile.Length - headerOffset;
            float[] samples = new float[dataSize / 2];

            for (int i = 0; i < samples.Length; i++)
                samples[i] = BitConverter.ToInt16(wavFile, headerOffset + i * 2) / 32768.0f;
 
            AudioClip audioClip = AudioClip.Create("wavClip", samples.Length, 1, 22050, false);
            audioClip.SetData(samples, 0);
            return audioClip;
        }

        public static IEnumerator LoadAudioFromFile(string filePath, Action<AudioClip> callback)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("Audio file not found at: " + filePath);
                callback?.Invoke(null);
                yield break;
            }

            byte[] audioData;

            try
            {
                // Read the WAV file bytes
                audioData = File.ReadAllBytes(filePath);
                Debug.Log("Successfully loaded file: " + filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to read audio file: " + e.Message);
                callback?.Invoke(null);
                yield break;
            }

            AudioClip audioClip = ToAudioClip(audioData);

            if (audioClip == null)
            {
                Debug.LogError("Failed to create AudioClip from the file: " + filePath);
                callback?.Invoke(null);
            }
            else
            {
                Debug.Log("Successfully created AudioClip from file.");
                callback?.Invoke(audioClip);
            }
        }
    }
}