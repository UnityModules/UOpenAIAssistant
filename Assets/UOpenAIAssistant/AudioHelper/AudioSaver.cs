using UnityEngine;
using System.IO;

namespace AudioHelper
{
    public static class AudioSaver
    {
        public static void Save(AudioClip clip, string filePath)
        {
            if (clip == null)
            {
                Debug.LogError("No audio clip to save.");
                return;
            }

            byte[] wavData = ConvertAudioClipToWav(clip);
            File.WriteAllBytes(filePath, wavData);

            Debug.Log("Audio saved to: " + filePath);
        }

        private static byte[] ConvertAudioClipToWav(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("AudioClip is null.");
                return null;
            }

            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            byte[] pcmData = new byte[samples.Length * 2];
            int offset = 0;
            for (int i = 0; i < samples.Length; i++)
            {
                short pcmSample = (short)(Mathf.Clamp(samples[i], -1.0f, 1.0f) * short.MaxValue);
                pcmData[offset++] = (byte)(pcmSample & 0x00FF);
                pcmData[offset++] = (byte)((pcmSample & 0xFF00) >> 8);
            }

            return AddWavHeader(pcmData, clip.channels, clip.frequency);
        }

        private static byte[] AddWavHeader(byte[] pcmData, int channels, int sampleRate)
        {
            int fileSize = pcmData.Length + 44;

            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
                writer.Write(fileSize - 8);
                writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));

                writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * 2);
                writer.Write((short)(channels * 2));
                writer.Write((short)16);

                writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
                writer.Write(pcmData.Length);
                writer.Write(pcmData);

                return memoryStream.ToArray();
            }
        }
    }
}