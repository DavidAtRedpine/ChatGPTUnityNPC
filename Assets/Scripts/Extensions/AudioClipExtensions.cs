using System;
using System.IO;
using UnityEngine;

public static class AudioClipExtensions
{
    public static void SaveToWAV(this AudioClip clip, string filename)
    {
        var filepath = Path.Combine(Application.persistentDataPath, filename);
        // Create WAV file from audio clip
        var wavFile = AudioClipToWav(clip);
        File.WriteAllBytes(filepath, wavFile);
        Debug.Log("Saved file to: " + filepath);
    }

    private static byte[] AudioClipToWav(AudioClip clip)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                var samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);

                WriteWavHeader(writer, clip.channels, clip.frequency, samples.Length);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }

                writer.Seek(4, SeekOrigin.Begin);
                uint fileSize = (uint)writer.BaseStream.Length;
                writer.Write(fileSize - 8);

                return memoryStream.ToArray();
            }
        }
    }

    private static void WriteWavHeader(BinaryWriter writer, int channels, int sampleRate, int sampleCount)
    {
        writer.Write(new char[] { 'R', 'I', 'F', 'F' });
        writer.Write(0); // Placeholder for file size
        writer.Write(new char[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
        writer.Write(16); // Length of format data
        writer.Write((short)1); // Type of format (1 is PCM)
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * 2); // byte rate
        writer.Write((short)(channels * 2)); // block align
        writer.Write((short)16); // bits per sample
        writer.Write(new char[] { 'd', 'a', 't', 'a' });
        writer.Write(sampleCount * 2); // data size
    }
}
