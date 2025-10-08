using System.Threading.Tasks;

namespace VirtualTutor
{
    public class GPTService : UnityEngine.MonoBehaviour
    {
        public async Task<string> GenerateResponseAsync(string input, string context)
        {
            await Task.Delay(200);
            if (string.IsNullOrWhiteSpace(input)) return "Could you repeat that?";
            return $"In simple terms: {input}. Let's build on your current topic.";
        }
    }
}