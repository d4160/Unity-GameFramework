using d4160.ResourceUtils;
using UnityEngine;

namespace d4160.Singleton
{
    [DisallowMultipleComponent]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                    SetSingleton();

                return _instance;
            }

            protected set => _instance = value;
        }

        public static bool Instanced => _instance;

        protected virtual bool DontDestroyOnLoadProp { get; } = false;
        protected virtual bool HideInHierarchy { get; } = false;

        private static string ResourcesName => $"{typeof(T)} Singleton (R)";
        private static string NewName => $"{typeof(T)} Singleton (New)";

        public TConcrete As<TConcrete>() where TConcrete : class => _instance as TConcrete;

        protected virtual void Awake()
        {
            SetSingletonOnAwake();

            if (DontDestroyOnLoadProp) SetDontDestroyOnLoad();
            if (HideInHierarchy) SetHideInHierarchy();
        }

        public void SetHideInHierarchy()
        {
#if UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideAndDontSave
                                | HideFlags.HideInInspector;
#endif
        }

        public void SetDontDestroyOnLoad()
        {
            if (transform.parent != null)
                transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }

        protected virtual void SetSingletonOnAwake()
        {
            if (!_instance)
            {
                Instance = this as T;
            }
            else
            {
                if (_instance != this as T)
                {
                    OnDestroyImmediateCallback();

                    DestroyImmediate(gameObject);
                }
            }
        }

        protected virtual void OnDestroyImmediateCallback() { }

        private static void SetSingleton()
        {
            var instancesCount = SearchInstancesOfT();

            if (instancesCount > 0) return;

            InstanceSingleton();
        }

        public static void InstanceSingleton()
        {
            System.Type type = typeof(T);

            T prefab = null;

            if (System.Attribute.GetCustomAttribute(type, typeof(ResourcesPathAttribute)) is ResourcesPathAttribute attribute)
            {
                prefab = Resources.Load<T>(attribute.path);
            }

            if (prefab == null)
            {
                if (type.IsAbstract)
                    return;

                var go = new GameObject(NewName);
                Instance = go.AddComponent<T>();
            }
            else
            {
                var instance = Instantiate(prefab);
                instance.name = ResourcesName;
                instance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

                Instance = instance;
            }
        }

        private static int SearchInstancesOfT()
        {
            var objects = FindObjectsByType<T>(FindObjectsSortMode.None);
            var length = objects.Length;

            if (length > 0)
                Instance = objects[0];

            if (length > 1)
            {
#if DEBUG_Singleton
                var type = typeof(T);
                Debug.LogWarning($"There is more than one _instance of Singleton of type '{type}'. Keeping the first. Destroying the others.");
#endif
                for (var i = 1; i < objects.Length; i++) DestroyImmediate(objects[i].gameObject);
            }

            return length;
        }
    }
}
