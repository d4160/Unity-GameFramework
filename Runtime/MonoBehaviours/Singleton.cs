#define DEBUG_Singleton

namespace d4160.Core
{
    using d4160.Core.Attributes;
    using UnityEngine;

    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T m_instance;
        protected static bool m_instanced;

        public static T Instance
        {
            get
            {
                if (!m_instance) 
                    SetSingleton();

                return m_instance;
            }

            protected set
            {
                m_instance = value;

                if (m_instance)
                    m_instanced = true;
            }
        }

        public static bool Instanced => m_instanced;

        protected virtual void Awake()
        {
            if (!m_instanced)
            {
                Instance = this as T;
            }
            else
            {
                if (m_instance != this as T)
                {
                    OnDestroyImmediateCallback();

                    DestroyImmediate(gameObject);
                }
            }
        }

        protected virtual void OnDisable()
        {
            m_instanced = false;
        }

        protected virtual void OnDestroyImmediateCallback(){}

        private static void SetSingleton()
        {
            var instancesCount = SearchInstancesOfT();

            if (instancesCount > 0) return;

            InstanceSingleton();
        }

        private static void InstanceSingleton()
        {
            System.Type type = typeof(T);

            ResourcesPathAttribute attribute = System.Attribute.GetCustomAttribute(type, typeof(ResourcesPathAttribute)) as ResourcesPathAttribute;
            T prefab = null;

            if (attribute != null)
            {
                prefab = Resources.Load<T>(attribute.path);
            }

            if (prefab == null)
            {
                var go = new GameObject($"'{type}' Singleton");
                Instance = go.AddComponent<T>();
            }
            else
            {
                Instance = Instantiate(prefab);
            }
        }

        private static int SearchInstancesOfT()
        {
            var type = typeof(T);
            var objects = FindObjectsOfType<T>();
            var length = objects.Length;

            if (length > 0)
                Instance = objects[0];

            if (length > 1)
            {
#if DEBUG_Singleton
                Debug.LogWarning($"There is more than one instance of Singleton of type '{type}'. Keeping the first. Destroying the others.");
#endif
                for (var i = 1; i < objects.Length; i++) DestroyImmediate(objects[i].gameObject);
            }

            return length;
        }
    }
}
