using d4160.Core;
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

        public GameObject Context { get; set; }

        public void LogInfo(string message) 
        {
            LoggerM31.LogInfo(message, _logLevel, _heading, _headingColor, _messageColor, Context);
        }

        public void LogWarning(string message) 
        {
            LoggerM31.LogWarning(message, _logLevel, _heading, _headingColor, _messageColor, Context);
        }

        public void LogError(string message) 
        {
            LoggerM31.LogError(message, _logLevel, _heading, _headingColor, _messageColor, Context);
        }
    }
}