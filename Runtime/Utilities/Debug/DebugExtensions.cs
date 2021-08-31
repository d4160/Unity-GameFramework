using System.Collections.Generic;
using UnityEngine;

namespace d4160._Debug
{
    public static class DebugExtensions
    {
        public static void Log<T>(this IList<T> source, char separator = ',')
        {
            string t = string.Empty;

            for (int i = 0; i < source.Count; i++)
            {
                t += $"{source[i]}{separator}";
            }

            t = t.Substring(0, t.Length - 1);

            Debug.Log(t);
        }
    }
}
