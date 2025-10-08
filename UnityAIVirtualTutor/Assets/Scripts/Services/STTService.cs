using System.Threading.Tasks;
using UnityEngine;

namespace VirtualTutor
{
    public class STTService : MonoBehaviour
    {
        public async Task<string> RecognizeAsync(AudioClip microphoneInput)
        {
            await Task.Delay(200);
            return string.Empty; // integrate Whisper/Azure later
        }
    }
}