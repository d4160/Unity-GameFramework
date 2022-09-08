using System.Collections;
using System.Collections.Generic;
using d4160.Core;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace d4160.SceneManagement {
    [CreateAssetMenu (menuName = "d4160/SceneManagement/Scene Link")]
    public class SceneLinkSO : ScriptableObject {
        [SerializeField] private SceneCollectionSO _sceneCollection;
        [SerializeField] private AssetManagementType _sceneAssetType;

        private LoadSceneMode _loadSceneMode;
        private bool _activateOnLoad;
        private int _priority;

        public SceneCollectionSO SceneCollection { get => _sceneCollection; set => _sceneCollection = value; }
        public AssetManagementType SceneAssetType { get => _sceneAssetType; set => _sceneAssetType = value; }

        public void SetLoadInfo (LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100) {
            _loadSceneMode = loadSceneMode;
            _activateOnLoad = activateOnLoad;
            _priority = priority;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ContinueLoadAsync () {
            ContinueLoadAsync(_sceneAssetType);
        }

        public void ContinueLoadAsync (AssetManagementType sceneAssetType) {
            switch (sceneAssetType) {
                case AssetManagementType.Default:
                    ContinueLoadAsyncDefault ();
                    break;
#if ADDRESSABLES
                case AssetManagementType.Addressables:
                    ContinueLoadAsyncAddressables ();
                    break;
#endif
                default:
                    break;
            }
        }

        public void ContinueLoadAsyncDefault () {
            if (SceneCollection) {
                SceneCollection.ContinueLoadAsyncDefault (_loadSceneMode, _activateOnLoad);
            }
        }

#if ADDRESSABLES
        public void ContinueLoadAsyncAddressables () {
            if (SceneCollection) {
                SceneCollection.ContinueLoadAsyncAddressables (_loadSceneMode, _activateOnLoad, _priority);
            }
        }
#endif
    }
}