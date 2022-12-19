using d4160.Core;
using d4160.Texts;
using UnityEngine;

namespace d4160.Logging
{
    [CreateAssetMenu(menuName = "d4160/Logging/Logger")]
    public class LoggerSO : ScriptableObject
    {
        [SerializeField] private LogLevelType _logLevel;
        [SerializeField] private string _heading;
        [SerializeField] private Color _headingColor;
        [SerializeField] private Color _messageColor;

#if UNITY_EDITOR
        private void SetDefaultColors()
        {
            _headingColor = UnityEditor.EditorGUIUtility.isProSkin ? Color.white : Color.black;
            _messageColor = UnityEditor.EditorGUIUtility.isProSkin ? Color.white : Color.black;
        }

        private void Reset()
        {
            SetDefaultColors();
        }
#endif

        public LogLevelType LogLevel => _logLevel;
        public GameObject Context { get; set; }

        public bool CanSendLog(LogLevelType logLevel) => (int)_logLevel >= (int)logLevel;
        public string GetMessage(string message) => $"{_heading.Color(_headingColor)}:{message.Color(_messageColor)}";

        public void LogInfo(string message, GameObject context)
        {
            LoggerM31.LogInfo(this, message, context);
        }

        public void LogInfo(string message) 
        {
            LoggerM31.LogInfo(this, message, Context);
        }

        public void LogWarning(string message, GameObject context)
        {
            LoggerM31.LogWarning(this, message, context);
        }

        public void LogWarning(string message) 
        {
            LoggerM31.LogWarning(this, message, Context);
        }

        public void LogError(string message, GameObject context)
        {
            LoggerM31.LogError(this, message, context);
        }

        public void LogError(string message) 
        {
            LoggerM31.LogError(this, message, Context);
        }
    }
}