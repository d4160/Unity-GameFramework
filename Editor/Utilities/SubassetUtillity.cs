using UnityEditor;
using UnityEngine;

namespace d4160.Core.Editors.Utilities
{
    public static class SubassetUtillity
    {
        public static void AddSubasset(this Object asset, Object subasset)
        {
            AssetDatabase.AddObjectToAsset(subasset, asset);
            asset.Reimport();
        }

        public static void RemoveSubAsset(this Object asset, Object subasset)
        {
            AssetDatabase.RemoveObjectFromAsset(subasset);
            asset.Reimport();
        }

        public static void Reimport(this Object asset)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
        }
    }
}