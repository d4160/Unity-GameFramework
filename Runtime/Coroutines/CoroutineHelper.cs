using d4160.Singleton;
using UnityEngine;

namespace d4160.Coroutines
{
    /// <summary>
    /// MonoBehaviour when need to start a Co-routine outside a MonoBehaviour
    /// </summary>
    public class CoroutineHelper : Singleton<CoroutineHelper>
    {
        protected override void Awake()
        {
            base.Awake();

            gameObject.hideFlags = HideFlags.HideAndDontSave
                                | HideFlags.HideInInspector;

            DontDestroyOnLoad(gameObject);
        }
    }
}