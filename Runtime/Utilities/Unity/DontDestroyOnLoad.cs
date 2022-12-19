#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.UnityUtils
{
    /// <summary>
    /// MonoBehaviour to set persistent GameObject in Play
    /// </summary>
    public class DontDestroyOnLoad : MonoBehaviour
    {
        protected virtual void Awake()
        {
            SetDontDestroyOnLoad();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void SetDontDestroyOnLoad()
        {
            if(!Application.isPlaying)
                return;

            if (transform.parent != null)
                transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }
    }
}