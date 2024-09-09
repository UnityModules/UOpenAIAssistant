using System;
using UnityEngine;
using System.IO;

public static class WavUtility
{
    // Convert byte[] WAV data into an AudioClip
    public static AudioClip ToAudioClip(byte[] data)
    {
        using (var memoryStream = new MemoryStream(data))
        using (var reader = new BinaryReader(memoryStream))
        {
            int channels = reader.ReadInt16();
            int sampleRate = reader.ReadInt32();
            reader.BaseStream.Seek(22, SeekOrigin.Begin); // Move to Data
            int dataSize = reader.ReadInt32();
            byte[] audioData = reader.ReadBytes(dataSize);
            AudioClip audioClip = AudioClip.Create("wavClip", dataSize / channels, channels, sampleRate, false);
            float[] audioSamples = ConvertByteToFloat(audioData);
            audioClip.SetData(audioSamples, 0);
            return audioClip;
        }
    }

    // Convert byte[] audio data into float[]
    private static float[] ConvertByteToFloat(byte[] audioData)
    {
        int floatCount = audioData.Length / 4;
        float[] floatArray = new float[floatCount];
        for (int i = 0; i < floatCount; i++)
        {
            floatArray[i] = BitConverter.ToSingle(audioData, i * 4);
        }
        return floatArray;
    }
}
