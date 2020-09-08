using System.Collections;
using System.Collections.Generic;
using d4160.Core.MonoBehaviours;
using UnityEngine;

namespace d4160.GameFramework.SceneManagement
{
    public class MySceneManager : Singleton<MySceneManager>
    {
        protected static Dictionary<string, int> LoadedScenes = new Dictionary<string, int>();

        public static void RegisterLoadedScene(string assetGuid)
        {
            if (LoadedScenes.ContainsKey(assetGuid))
            {
                LoadedScenes[assetGuid]++;
            }
            else
            {
                LoadedScenes.Add(assetGuid, 1);
            }
        }

        public static void UnregisterLoadedScene(string assetGuid)
        {
            if (!LoadedScenes.ContainsKey(assetGuid)) return;

            var count = LoadedScenes[assetGuid]--;
            if (count < 0)
                LoadedScenes[assetGuid] = 0;
        }

        public static bool IsLoaded(string assetGuid)
        {
            if (!LoadedScenes.ContainsKey(assetGuid))
                return false;

            return LoadedScenes[assetGuid] > 0;
        }
    }
}