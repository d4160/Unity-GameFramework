using System.Collections;
using System.Collections.Generic;
using d4160.Core;
using NaughtyAttributes;
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

        [Button]
        public void ContinueLoadAsync () {
            ContinueLoadAsync(_sceneAssetType);
        }

        public void ContinueLoadAsync (AssetManagementType sceneAssetType) {
            switch (sceneAssetType) {
                case AssetManagementType.Default:
                    ContinueLoadAsyncDefault ();
                    break;
                case AssetManagementType.Addressables:
                    ContinueLoadAsyncAddressables ();
                    break;
                default:
                    break;
            }
        }

        public void ContinueLoadAsyncDefault () {
            if (SceneCollection) {
                SceneCollection.ContinueLoadAsyncDefault (_loadSceneMode, _activateOnLoad);
            }
        }

        public void ContinueLoadAsyncAddressables () {
            if (SceneCollection) {
                SceneCollection.ContinueLoadAsyncAddressables (_loadSceneMode, _activateOnLoad, _priority);
            }
        }
    }
}