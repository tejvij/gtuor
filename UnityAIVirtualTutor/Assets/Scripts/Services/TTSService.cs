using System.Threading.Tasks;
using UnityEngine;

namespace VirtualTutor
{
    public class TTSService : MonoBehaviour
    {
        [Range(0.75f, 1.5f)] public float speechRate = 1.0f;
        [Range(0.0f, 0.1f)] public float volume = 0.03f;
        public int sampleRate = 24000;

        public async Task<AudioClip> GenerateAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            float duration = Mathf.Clamp(text.Length / 14f / Mathf.Max(0.01f, speechRate), 1f, 8f);
            int samples = Mathf.CeilToInt(duration * sampleRate);
            float[] data = new float[samples];
            float freq = 220f;
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                data[i] = Mathf.Sin(2f * Mathf.PI * freq * t) * volume;
            }
            var clip = AudioClip.Create($"TTS_{text.Substring(0, Mathf.Min(10, text.Length))}", samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            await Task.Yield();
            return clip;
        }
    }
}