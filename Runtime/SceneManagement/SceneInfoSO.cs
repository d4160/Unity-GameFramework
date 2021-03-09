using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace d4160.SceneManagement
{
    [CreateAssetMenu(menuName = "d4160/SceneManagement/Scene Info")]
    public class SceneInfoSO : ScriptableObject
    {
        public ScenePackSO scenePack;

        private LoadSceneMode _loadSceneMode;
        private bool _activateOnLoad;
        private int _priority;

        public void SetLoadInfo(LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            _loadSceneMode = loadSceneMode;
            _activateOnLoad = activateOnLoad;
            _priority = priority;
        }
    
        [Button]
        public void ContinueLoadAsync()
        {
            if (scenePack)
            {
                scenePack.ContinueLoadAsync(_loadSceneMode, _activateOnLoad);
            }
        }
        
        [Button]
        public void ContinueLoadAsyncAddressables()
        {
            if (scenePack)
            {
                scenePack.ContinueLoadAsyncAddressables(_loadSceneMode, _activateOnLoad, _priority);
            }
        }
    }
}