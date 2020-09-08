﻿using UnityEngine;

namespace d4160.Core.MonoBehaviours
{
    /// <summary>
    /// Simple actions for UnityEvents
    /// </summary>
    public class UnityConsoleDebugger : MonoBehaviour
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