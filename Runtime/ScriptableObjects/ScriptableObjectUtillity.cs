#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace d4160.ScriptableObjects.Editors.Utilities
{
    public static class ScriptableObjectUtillity
    {
        public static void AddSubasset(this Object asset, Object subasset, bool reimport = true)
        {
            AssetDatabase.AddObjectToAsset(subasset, asset);
            if (reimport) asset.Reimport();
        }

        public static void RemoveSubAsset(this Object asset, Object subasset, bool reimport = true)
        {
            AssetDatabase.RemoveObjectFromAsset(subasset);
            if (reimport) asset.Reimport();
        }

        public static void Reimport(this Object asset)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
        }

        public static List<T> GetSubObjectsOfType<T>(this ScriptableObject asset) where T : ScriptableObject
        {
            Object[] objs = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(asset));

            List<T> objects = new List<T>();

            foreach (Object o in objs)
            {
                if (o.GetType() == typeof(T)){
                    objects.Add(o as T);
                }
            }

            return objects;
        }

        public static T CreateCopy<T>(this ScriptableObject tObject, T copyFrom) where T : ScriptableObject
        {
            if (tObject is IScriptableCopy<T> sCopy)
            {
                var newInstance = ScriptableObject.CreateInstance<T>();
                sCopy.Copy(copyFrom, newInstance);

                return newInstance;
            }

            return null;
        }
    }
}
#endif