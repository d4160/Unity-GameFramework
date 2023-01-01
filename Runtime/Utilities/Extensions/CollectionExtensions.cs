using System.Collections.Generic;
using UnityEngine;
using System;

namespace d4160.Collections
{
    public static class CollectionExtensions
    {
        public static void Shuffle<T>(this IList<T> source, Action<int, int> onItem = null)
        {
            int n = source.Count;
            while (n > 1)
            {
                n--;

                int k = UnityEngine.Random.Range(0, n + 1);

                onItem?.Invoke(n, k);

                T value = source[k];
                source[k] = source[n];
                source[n] = value;
            }
        }

        public static bool IsValidIndex<T>(this IList<T> source, int idx) => (source != null && source.Count > 0 && idx >= 0 && idx < source.Count) ? true : false;

        public static bool IsValidIndex(this string source, int idx) => (source != null && source.Length > 0 && idx >= 0 && idx < source.Length) ? true : false;

        public static bool IsLastIndex<T>(this IList<T> source, int idx) => (source.Count - 1 == idx) ? true : false;

        public static int LastIndex<T>(this IList<T> source) => source.Count - 1;

        public static T First<T>(this IList<T> source) => source.Count > 0 ? source[0] : default;

        public static T Last<T>(this IList<T> source) => source.Count > 0 ? source[source.Count - 1] : default;

        public static int Scalar<T>(this T[,,] source) => source.GetLength(0) * source.GetLength(1) * source.GetLength(2);

        public static void Iterate<T>(this IList<T> source, Action<T> callback)
        {
            for (int i = 0; i < source.Count; i++)
            {
                callback(source[i]);
            }
        }

        public static void Iterate<T>(this IList<T> source, Action<T, int> callback)
        {
            for (int i = 0; i < source.Count; i++)
            {
                callback(source[i], i);
            }
        }

        public static void Iterate<T>(this T[,,] source, Action<T> callback)
        {
            for (int x = 0; x < source.GetLength(0); x++)
                for (int y = 0; y < source.GetLength(1); y++)
                    for (int z = 0; z < source.GetLength(2); z++)
                        callback(source[x, y, z]);
        }

        public static void Iterate<T>(this T[,,] source, Action<T, Vector3Int> callback)
        {
            for (int x = 0; x < source.GetLength(0); x++)
                for (int y = 0; y < source.GetLength(1); y++)
                    for (int z = 0; z < source.GetLength(2); z++)
                        callback(source[x, y, z], new Vector3Int(x, y, z));
        }

        public static void Iterate<T>(this T[,,] source, Action<T, Vector3Int, int> callback)
        {
            int index = 0;
            if (source == null) return;

            for (int x = 0; x < source.GetLength(0); x++)
                for (int y = 0; y < source.GetLength(1); y++)
                    for (int z = 0; z < source.GetLength(2); z++)
                    {
                        callback(source[x, y, z], new Vector3Int(x, y, z), index);
                        index++;
                    }
        }

        public static int Scalar<T>(this T[,] source) => source.GetLength(0) * source.GetLength(1);

        public static void Iterate<T>(this T[,] source, Action<T> callback)
        {
            for (int x = 0; x < source.GetLength(0); x++)
                for (int y = 0; y < source.GetLength(1); y++)
                    callback(source[x, y]);
        }

        public static void Iterate<T>(this T[,] source, Action<T, Vector2Int> callback)
        {
            for (int x = 0; x < source.GetLength(0); x++)
                for (int y = 0; y < source.GetLength(1); y++)
                    callback(source[x, y], new Vector2Int(x, y));
        }

        public static void Iterate<T>(this T[,] source, Action<T, Vector2Int, int> callback)
        {
            int index = 0;
            for (int x = 0; x < source.GetLength(0); x++)
                for (int y = 0; y < source.GetLength(1); y++)
                {
                    callback(source[x, y], new Vector2Int(x, y), index);
                    index++;
                }
        }

        public static int RandomIndex<T>(this IList<T> source, int min = 0, int max = 0)
        {
            if (source.Count == 1) return -1;

            max = max <= 0 || max > source.Count ? source.Count : max;
            min = min < 0 || min > max ? 0 : min;

            return UnityEngine.Random.Range(min, max);
        }

        public static int RandomCount<T>(this IList<T> source, int min = 1, int max = 0)
        {
            if (source.Count == 0) return 0;

            max = max <= 0 || max > source.Count ? source.Count : max;
            min = min < 1 || min > max ? 1 : min;

            return UnityEngine.Random.Range(min, max + 1);
        }

        public static T Random<T>(this IList<T> source) => source.Count > 0 ? source[UnityEngine.Random.Range(0, source.Count)] : default;

        public static T KeyByValue<T, W>(this IDictionary<T, W> source, W value)
        {
            T key = default;
            foreach (KeyValuePair<T, W> pair in source)
            {
                if (EqualityComparer<W>.Default.Equals(pair.Value, value))
                {
                    key = pair.Key;
                    break;
                }
            }
            return key;
        }
    }
}