namespace d4160.Utilities
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    using UnityEngine.Events;
    using d4160.Core;

    public static class UnityUtilities
    {
#if UNITY_EDITOR
        public static BuildTargetGroup ConvertBuildTarget(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.WebGL:
                    return BuildTargetGroup.WebGL;
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.PS4:
                    return BuildTargetGroup.PS4;
                case BuildTarget.XboxOne:
                    return BuildTargetGroup.XboxOne;
                case BuildTarget.tvOS:
                    return BuildTargetGroup.tvOS;
                case BuildTarget.Switch:
                    return BuildTargetGroup.Switch;
                case BuildTarget.NoTarget:
                default:
                    return BuildTargetGroup.Standalone;
            }
        }

        public static MonoBehaviour GetObjectOfType(MonoBehaviour[] objects, System.Type type)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].GetType() == type)
                {
                    return objects[i];
                }
            }

            return null;
        }

        public static MonoBehaviour GetObjectOfType(MonoBehaviour[] objects, string typeName)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].GetType().Name == typeName)
                {
                    return objects[i];
                }
            }

            return null;
        }

        public static MonoBehaviour GetObjectOfGenericType(MonoBehaviour[] objects, System.Type genericType)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                if (ReflectionUtilities.IsSubclassOfRawGeneric(typeof(Singleton<>), objects[i].GetType()))
                {
                    return objects[i];
                }
            }

            return null;
        }
#endif

        public static bool IsLayer(LayerMask mask, int layer)
        {
            return (mask.value & 1 << layer) != 0; 
        }

        [System.Serializable]
        public class BoolEvent : UnityEvent<bool>
        {
        }

        [System.Serializable]
        public class IntEvent : UnityEvent<int>
        {
        }

        [System.Serializable]
        public class FloatEvent : UnityEvent<float>
        {
        }

        [System.Serializable]
        public class Vector2Event : UnityEvent<Vector2>
        {
        }

        [System.Serializable]
        public class Vector3Event : UnityEvent<Vector3>
        {
        }

        [System.Serializable]
        public class StringEvent : UnityEvent<string>
        {
        }
    }
}
