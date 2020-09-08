using UnityEngine;

namespace d4160.Core.MonoBehaviours
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        [SerializeField] protected bool _activeSelf = true;

        protected virtual void Awake()
        {
            SetDontDestroyOnLoad();
        }

        public virtual void SetDontDestroyOnLoad()
        {
            if(!Application.isPlaying)
                return;

            if (_activeSelf)
            {
                if (transform.parent != null)
                    transform.SetParent(null);

                DontDestroyOnLoad(gameObject);
            }
        }
    }
}