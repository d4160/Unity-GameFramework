using UnityEngine;

namespace d4160._Debug
{
    /// <summary>
    /// Debug calls to call from UnityEvents
    /// </summary>
    public sealed class UnityConsoleDebuggerBeh : MonoBehaviour
    {
        public void Log(string value)
        {
            Debug.Log(value);
        }

        public void LogWarning(string value)
        {
            Debug.LogWarning(value);
        }

        public void LogError(string value)
        {
            Debug.LogError(value);
        }
    }
}