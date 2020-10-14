using d4160.Core;
using UnityEngine;

namespace d4160.GameFramework.Logging
{
    [CreateAssetMenu(menuName = "Game Framework/Logging Settings")]
    public class LogLevelsSO : ScriptableObject
    {
        [SerializeField] protected LogLevelDefinition[] _logLevels;

        public LogLevelDefinition[] LogLevels => _logLevels;

        public bool IsActived(int idx, LogLevel logLevel)
        {
            if (_logLevels.IsValidIndex(idx))
            {
                return _logLevels[idx].logLevel <= logLevel;
            }

            return false;
        }

        public bool IsActived(string name, LogLevel logLevel)
        {
            int idx = GetIdx(name);
            if (_logLevels.IsValidIndex(idx))
            {
                return _logLevels[idx].logLevel <= logLevel;
            }

            return false;
        }

        protected int GetIdx(string name)
        {
            for (int i = 0; i < _logLevels.Length; i++)
            {
                if (_logLevels[i].name == name)
                    return i;
            }

            return -1;
        }
    }

    [System.Serializable]
    public struct LogLevelDefinition
    {
        public string name;
        public LogLevel logLevel;
    }

    public enum LogLevel
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical,
        None
    }

    public enum LogCategory
    {
        System = 0,
        Other = 1
    }
}