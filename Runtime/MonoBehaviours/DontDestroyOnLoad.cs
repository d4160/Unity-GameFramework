namespace d4160.Systems.Common
{
    using UnityEngine;

    public class DontDestroyOnLoad : MonoBehaviour
    {
        public bool activeSelf = true;

        private void Awake()
        {
            SetDontDestroyOnLoad();
        }

        protected void SetDontDestroyOnLoad()
        {
            if (activeSelf)
            {
                if (transform.parent != null)
                    transform.SetParent(null);

                DontDestroyOnLoad(gameObject);
            }
        }
    }
}