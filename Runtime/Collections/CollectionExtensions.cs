namespace d4160.Collections
{
    using System.Collections.Generic;
    using UnityEngine;
    using System;

    public static class CollectionExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static bool IsValidIndex<T>(this IList<T> list, int idx)
        {
            return (list != null && idx >= 0 && idx < list.Count) ? true : false;
        }

        public static bool IsLastIndex<T>(this IList<T> list, int idx)
        {
            return (list.Count - 1 == idx) ? true : false;
        }

        public static int LastIndex<T>(this IList<T> list)
        {
            return list.Count - 1;
        }

        public static T Last<T>(this IList<T> list)
        {
            return list[list.Count - 1];
        }

        public static int Scalar<T>(this T[,,] source)
        {
            return source.GetLength(0) * source.GetLength(1) * source.GetLength(2);
        }

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
            for (int z = 0; z < source.GetLength(0); z++)
                for (int y = 0; y < source.GetLength(1); y++)
                    for (int x = 0; x < source.GetLength(2); x++)
                        callback(source[z, y, x]);
        }

        public static void Iterate<T>(this T[,,] source, Action<T, Vector3Int> callback)
        {
            for (int z = 0; z < source.GetLength(0); z++)
                for (int y = 0; y < source.GetLength(1); y++)
                    for (int x = 0; x < source.GetLength(2); x++)
                        callback(source[z, y, x], new Vector3Int(x, y, z));
        }

        public static void Iterate<T>(this T[,,] source, Action<T, Vector3Int, int> callback)
        {
            int index = 0;
            if (source == null) return;

            for (int z = 0; z < source.GetLength(0); z++)
                for (int y = 0; y < source.GetLength(1); y++)
                    for (int x = 0; x < source.GetLength(2); x++)
                    {
                        callback(source[z, y, x], new Vector3Int(x, y, z), index);
                        index++;
                    }
        }

        public static int Scalar<T>(this T[,] source)
        {
            return source.GetLength(0) * source.GetLength(1);
        }

        public static void Iterate<T>(this T[,] source, Action<T> callback)
        {
            for (int y = 0; y < source.GetLength(1); y++)
                for (int x = 0; x < source.GetLength(2); x++)
                    callback(source[y, x]);
        }

        public static void Iterate<T>(this T[,] source, Action<T, Vector2Int> callback)
        {
            for (int y = 0; y < source.GetLength(1); y++)
                for (int x = 0; x < source.GetLength(2); x++)
                    callback(source[y, x], new Vector2Int(x, y));
        }

        public static void Iterate<T>(this T[,] source, Action<T, Vector2Int, int> callback)
        {
            int index = 0;
            for (int y = 0; y < source.GetLength(1); y++)
                for (int x = 0; x < source.GetLength(2); x++)
                {
                    callback(source[y, x], new Vector2Int(x, y), index);
                    index++;
                }
        }

        public static int RandomIndex<T>(this IList<T> list, int min = 0, int max = 0)
        {
            if (list.Count <= 1) return list.Count - 1;

            if (max == 0)
                max = list.Count;
            return UnityEngine.Random.Range(min, max);
        }

        public static int RandomCount<T>(this IList<T> list, int min = 1, int max = 0)
        {
            if (max == 0)
                max = list.Count;
            return UnityEngine.Random.Range(min, max + 1);
        }

        public static T Random<T>(this IList<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static T KeyByValue<T, W>(this IDictionary<T, W> dict, W val)
        {
            T key = default;
            foreach (KeyValuePair<T, W> pair in dict)
            {
                if (EqualityComparer<W>.Default.Equals(pair.Value, val))
                {
                    key = pair.Key;
                    break;
                }
            }
            return key;
        }
    }
}