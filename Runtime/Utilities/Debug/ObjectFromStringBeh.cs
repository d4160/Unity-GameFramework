using System;
using UnityEngine;

namespace d4160._Debug
{
    /// <summary>
    /// Transforms string to common types. Util for UltEvents.
    /// </summary>
    public class ObjectFromStringBeh : MonoBehaviour
    {
        public float ToFloat(string value)
        {
            return Convert.ToSingle(value);
        }

        public int ToInt(string value)
        {
            return Convert.ToInt32(value);
        }

        public bool ToBool(string value)
        {
            return Convert.ToBoolean(value);
        }
    }
}