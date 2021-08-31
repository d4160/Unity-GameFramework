using d4160.Core;
using UnityEngine;
using d4160.Texts;

namespace d4160.Logging
{
    public static class LoggerM31
    {
        public static LogLevelType LogLevelGlobal { get; set; } 

        public static void LogWarning(string message, string heading = "", GameObject context = null) 
        {
            Log(string.IsNullOrEmpty(heading) ? $"{message}" : $"{heading}: {message}", LogLevelGlobal, LogLevelType.Warning, context);
        }

        public static void LogWarning(string message, string heading, Color headingColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message}", LogLevelGlobal, LogLevelType.Warning, context);
        }

        public static void LogWarning(string message, string heading, Color headingColor, Color messageColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message.Color(messageColor)}", LogLevelGlobal, LogLevelType.Warning, context);
        }

        public static void LogInfo(string message, string heading = "", GameObject context = null) 
        {
            Log(string.IsNullOrEmpty(heading) ? $"{message}" : $"{heading}: {message}", LogLevelGlobal, LogLevelType.Info, context);
        }

        public static void LogInfo(string message, string heading, Color headingColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message}", LogLevelGlobal, LogLevelType.Info, context);
        }

        public static void LogInfo(string message, string heading, Color headingColor, Color messageColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message.Color(messageColor)}", LogLevelGlobal, LogLevelType.Info, context);
        }

        public static void LogError(string message, string heading = "", GameObject context = null) 
        {
            Log(string.IsNullOrEmpty(heading) ? $"{message}" : $"{heading}: {message}", LogLevelGlobal, LogLevelType.Error, context);
        }

        public static void LogError(string message, string heading, Color headingColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message}", LogLevelGlobal, LogLevelType.Error, context);
        }

        public static void LogError(string message, string heading, Color headingColor, Color messageColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message.Color(messageColor)}", LogLevelGlobal, LogLevelType.Error, context);
        }

        public static void LogWarning(string message, LogLevelType storedLogLevel, string heading = "", GameObject context = null) 
        {
            Log(string.IsNullOrEmpty(heading) ? $"{message}" : $"{heading}: {message}", storedLogLevel, LogLevelType.Warning, context);
        }

        public static void LogWarning(string message, LogLevelType storedLogLevel, string heading, Color headingColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message}", storedLogLevel, LogLevelType.Warning, context);
        }

        public static void LogWarning(string message, LogLevelType storedLogLevel, string heading, Color headingColor, Color messageColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message.Color(messageColor)}", storedLogLevel, LogLevelType.Warning, context);
        }

        public static void LogInfo(string message, LogLevelType storedLogLevel, string heading = "", GameObject context = null) 
        {
            Log(string.IsNullOrEmpty(heading) ? $"{message}" : $"{heading}: {message}", storedLogLevel, LogLevelType.Info, context);
        }

        public static void LogInfo(string message, LogLevelType storedLogLevel, string heading, Color headingColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message}", storedLogLevel, LogLevelType.Info, context);
        }

        public static void LogInfo(string message, LogLevelType storedLogLevel, string heading, Color headingColor, Color messageColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message.Color(messageColor)}", storedLogLevel, LogLevelType.Info, context);
        }

        public static void LogError(string message, LogLevelType storedLogLevel, string heading = "", GameObject context = null) 
        {
            Log(string.IsNullOrEmpty(heading) ? $"{message}" : $"{heading}: {message}", storedLogLevel, LogLevelType.Error, context);
        }

        public static void LogError(string message, LogLevelType storedLogLevel, string heading, Color headingColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message}", storedLogLevel, LogLevelType.Error, context);
        }

        public static void LogError(string message, LogLevelType storedLogLevel, string heading, Color headingColor, Color messageColor, GameObject context = null) 
        {
            Log($"{heading.Color(headingColor)}: {message.Color(messageColor)}", storedLogLevel, LogLevelType.Error, context);
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(string message, LogLevelType storedLogLevel, LogLevelType logLevel, GameObject context = null) {
            if((int)storedLogLevel >= (int)logLevel) {
                switch(logLevel) {
                    case LogLevelType.None:
                        break;
                    case LogLevelType.Error:
                        if (context) Debug.LogError(message, context); else Debug.LogError(message);
                        break;
                    case LogLevelType.Warning:
                        if (context) Debug.LogWarning(message, context); else Debug.LogError(message);
                        break;
                    case LogLevelType.Info:
                    case LogLevelType.Debug:
                        if (context) Debug.Log(message, context); else Debug.LogError(message);
                        break;
                }
            }
        }
    }
}