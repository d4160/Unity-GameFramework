using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace d4160.GameFramework.Logging
{
    public class MyDebug
    {
        protected static LogLevelsSO _logLevelsSO;
        public static LogLevelsSO LogLevelsSO {
            get
            {
                if (!_logLevelsSO)
                    _logLevelsSO = Resources.Load<LogLevelsSO>("LogLevelsSO");

                if (!_logLevelsSO)
                {
                    Debug.LogError("Missing LogLevelsSO object in Resources folder. Ensure that there is one and has the exact name of 'LogLevelsSO'");
                }

                return _logLevelsSO;
            }
        }

        public static void Log(string message, LogLevel level = LogLevel.Debug, string categoryName = null, Object context = null)
        {
            if (_logLevelsSO)
            {
                if (!string.IsNullOrEmpty(categoryName))
                {
                    if (!_logLevelsSO.IsActived(categoryName, level))
                        return;
                }
                else
                {
                    if (!_logLevelsSO.IsActived(0, level))
                        return;
                }
            }
            else
            {
                return;
            }

            LogInternal(message, level, context);
        }

        public static void Log(string message, LogLevel level = LogLevel.Debug, int categoryIdx = 0, Object context = null)
        {
            if (_logLevelsSO)
            {
                if (!_logLevelsSO.IsActived(categoryIdx, level))
                    return;
            }
            else
            {
                return;
            }

            LogInternal(message, level, context);
        }

        protected static void LogInternal(string message, LogLevel level, Object context = null)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    Debug.Log(message, context);
                    break;
                case LogLevel.Debug:
                    Debug.Log(message, context);
                    break;
                case LogLevel.Information:
                    Debug.Log(message, context);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(message, context);
                    break;
                case LogLevel.Error:
                    Debug.LogError(message, context);
                    break;
                case LogLevel.Critical:
                    Debug.LogError(message, context);
                    break;
                case LogLevel.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }
    }
}