using System;
using UnityEngine;

namespace VirtualTutor
{
    public enum ChalkStyle
    {
        Chalk
    }

    public class ChalkBoard : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private Color chalkColor = Color.white;

        public event Action<string> OnWrite;
        public string LastWrittenContent { get; private set; } = string.Empty;

        public void Write(string content, ChalkStyle style, Color color)
        {
            LastWrittenContent = content;
            chalkColor = color;
            Debug.Log($"[ChalkBoard] Writing: {content}");
            OnWrite?.Invoke(content);
            // Real implementation would drive a line-renderer or texture draw here
        }
    }
}