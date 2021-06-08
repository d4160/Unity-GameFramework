using d4160.Core;
using UnityEngine;

namespace d4160.Logging
{
    public static class Logger
    {
        public static LogLevelType LogLevelGlobal { get; set; } 

        public static void LogWarning(string message) {
            Log(message, LogLevelGlobal, LogLevelType.Warning);
        }

        public static void LogInfo(string message) {
            Log(message, LogLevelGlobal, LogLevelType.Info);
        }

        public static void LogError(string message) {
            Log(message, LogLevelGlobal, LogLevelType.Error);
        }

        public static void LogVerbose(string message) {
            Log(message, LogLevelGlobal, LogLevelType.Debug);
        }
        
        public static void LogWarning(string message, LogLevelType currentLogLevel) {
            Log(message, currentLogLevel, LogLevelType.Warning);
        }

        public static void LogInfo(string message, LogLevelType currentLogLevel) {
            Log(message, currentLogLevel, LogLevelType.Info);
        }

        public static void LogError(string message, LogLevelType currentLogLevel) {
            Log(message, currentLogLevel, LogLevelType.Error);
        }

        public static void LogVerbose(string message, LogLevelType currentLogLevel) {
            Log(message, currentLogLevel, LogLevelType.Debug);
        }

        public static void Log(string message, LogLevelType currentLogLevel, LogLevelType logLevelToCheck) {
            if((int)currentLogLevel >= (int)logLevelToCheck) {
                switch(logLevelToCheck) {
                    case LogLevelType.None:
                        break;
                    case LogLevelType.Error:
                        Debug.LogError(message);
                        break;
                    case LogLevelType.Warning:
                        Debug.LogWarning(message);
                        break;
                    case LogLevelType.Info:
                    case LogLevelType.Debug:
                        Debug.Log(message);
                        break;
                }
            }
        }
    }
}