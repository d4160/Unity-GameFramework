using UnityEngine;

namespace d4160.UnityUtils
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Search the Component inside the GameObject with options
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="c"></param>
        /// <param name="searchInThis"></param>
        /// <param name="searchInChildren"></param>
        /// <param name="searchInParent"></param>
        /// <param name="parentRecursive"></param>
        /// <returns></returns>
        public static T GetComponent<T>(this Component c, bool searchInThis, bool searchInChildren = false, bool searchInParent = false, bool parentRecursive = false) where T : class
        {
            T t = null;

            if (searchInThis)
                t = c.GetComponent<T>();
            
            if ((t == null || t.ToString() == "null") && searchInChildren)
            {
                t = c.GetComponentInChildren<T>();
            }

            if (searchInParent && (t == null || t.ToString() == "null"))
            {
                t = c.transform.parent.GetComponent<T>();
            }

            if ((t == null || t.ToString() == "null") && parentRecursive)
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