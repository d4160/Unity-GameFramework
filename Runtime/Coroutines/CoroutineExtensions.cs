using System.Collections;
using UnityEngine;

namespace d4160.Coroutines
{
    public static class CoroutineExtensions
    {
        public static Coroutine StartCoroutine(this IEnumerator instance)
        {
            return CoroutineHelper.Instance.StartCoroutine(instance);
        }

        public static void StopCoroutine(this IEnumerator instance)
        {
            CoroutineHelper.Instance.StopCoroutine(instance);
        }

        public static void StopCoroutine(this Coroutine instance)
        {
            CoroutineHelper.Instance.StopCoroutine(instance);
        }
    }
}
