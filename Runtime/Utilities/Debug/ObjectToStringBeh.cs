using UnityEngine;

namespace d4160._Debug
{
    /// <summary>
    /// Helper to cast object to string. Util when use UltEvents and want to debug the value.
    /// </summary>
    public sealed class ObjectToStringBeh : MonoBehaviour
    {
        public string ToString(float value)
        {
            return value.ToString();
        }

        public string ToString(int value)
        {
            return value.ToString();
        }

        public string ToString(bool value)
        {
            return value.ToString();
        }
    }
}
