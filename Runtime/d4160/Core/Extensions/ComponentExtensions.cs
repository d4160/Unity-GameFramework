namespace d4160.Core
{
    using UnityEngine;

    public static class ComponentExtensions
    {
        /// <summary>
        /// Warning: Only works search for Behaviours (when searching on parent) Unity 2019.1.b4
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="searchInThis"></param>
        /// <param name="searchInChildren"></param>
        /// <param name="searchInParent"></param>
        /// <param name="parentRecursive"></param>
        /// <returns></returns>
        public static T GetComponent<T>(this Component c, bool searchInThis, bool searchInChildren = false, bool searchInParent = false, bool parentRecursive = false)
        {
            T t = default;

            if (searchInThis)
                t = c.GetComponent<T>();

            if (t == null && searchInChildren)
            {
                t = c.GetComponentInChildren<T>();
            }

            if (searchInParent && (t == null || t.ToString() == "null"))
            {
                t = c.transform.parent.GetComponent<T>();
            }

            if (t == null && parentRecursive)
            {
                Transform p = c.transform.parent;

                if (p && p.parent)
                {
                    p = p.parent;

                    while (t == null && p)
                    {
                        t = p.GetComponent<T>();

                        p = p.parent;
                    }
                }
            }

            if (t == null)
            {
                if (Debug.isDebugBuild)
                    Debug.LogError($"Component of type {typeof(T)} cannot be found.", c.gameObject);
            }

            return t;
        }
    }
}