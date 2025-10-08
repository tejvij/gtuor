using UnityEngine;

namespace VirtualTutor
{
    public class LipSyncController : MonoBehaviour
    {
        [Header("Defaults")]
        public string DefaultVoicePreset = "IndianEnglishFemale";

        public void Play(string text, string voicePreset, string expression)
        {
            // Placeholder: animate mouth/facial blendshapes keyed by synthesized phonemes
            // For now, do nothing beyond being callable
        }

        public void SetActive(bool active)
        {
            enabled = active;
        }
    }
}