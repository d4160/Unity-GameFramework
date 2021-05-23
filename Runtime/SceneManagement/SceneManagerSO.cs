using System.Collections;
using System.Collections.Generic;
using d4160.Collections;
using d4160.Core;
using InspectInLine;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.SceneManagement {
    [CreateAssetMenu (menuName = "d4160/SceneManagement/Scene Manager")]
    public class SceneManagerSO : ScriptableObject {

        [InspectInline (canEditRemoteTarget = true)]
        [SerializeField] private SceneCollectionSO[] _sceneCollections;
        [SerializeField] private AssetManagementType _sceneAssetType;

        public AssetManagementType SceneAssetType { get => _sceneAssetType; set => _sceneAssetType = value; }

        #if UNITY_EDITOR
        public string[] GetSceneCollectionNames { 
            get {
                if(_sceneCollections == null) return new string[0];
                string[] names = new string[_sceneCollections.Length];
                for (int i = 0; i < names.Length; i++) {
                    names[i] = _sceneCollections[i]?.Label;
                }
                return names;
            } 
        }
        #endif

        public SceneCollectionSO GetSceneCollectionAt(int index) {
            if (_sceneCollections.IsValidIndex(index)) {
                return _sceneCollections[index];
            }

            return null;
        }

        public SceneCollectionSO GetSceneCollection(string label) {
            for (int i = 0; i < _sceneCollections.Length; i++) {
                SceneCollectionSO sceneCollec = _sceneCollections[i];
                if (sceneCollec.CompareLabel(label)) {
                    return sceneCollec;
                }
            }

            return null;
        }

        /* LOAD */

        public void LoadSceneCollectionAsync(string label) {
            LoadSceneCollectionAsync (label, _sceneAssetType);
        }

        public void LoadSceneCollectionAsync(int index) {
            LoadSceneCollectionAsync (index, _sceneAssetType);
        }

        public void LoadSceneCollectionAsync(string label, AssetManagementType sceneAssetType) {
            switch (sceneAssetType) {
                case AssetManagementType.Default:
                    LoadSceneCollectionAsyncDefault (label);
                    break;
                case AssetManagementType.Addressables:
                    LoadSceneCollectionsAsyncAddressables (label);
                    break;
                default:
                    break;
            }
        }

        public void LoadSceneCollectionAsync(int index, AssetManagementType sceneAssetType) {
            switch (sceneAssetType) {
                case AssetManagementType.Default:
                    LoadSceneCollectionAsyncDefault (index);
                    break;
                case AssetManagementType.Addressables:
                    LoadSceneCollectionAsyncAddressables (index);
                    break;
                default:
                    break;
            }
        }

        public void LoadSceneCollectionAsyncDefault(string label) {
            LoadSceneCollectionAsyncDefault(GetSceneCollection(label));
        }

        public void LoadSceneCollectionsAsyncAddressables(string label) {
            LoadSceneCollectionAsyncAddressables(GetSceneCollection(label));
        }

        public void LoadSceneCollectionAsyncDefault(int index) {
            LoadSceneCollectionAsyncDefault(GetSceneCollectionAt(index));
        }

        public void LoadSceneCollectionAsyncAddressables(int index) {
            LoadSceneCollectionAsyncAddressables(GetSceneCollectionAt(index));
        }

        public void LoadSceneCollectionAsyncAddressables(SceneCollectionSO sceneCollection) {
            sceneCollection?.LoadScenesAsyncAddressables();
        }

        public void LoadSceneCollectionAsyncDefault(SceneCollectionSO sceneCollection) {
            sceneCollection?.LoadScenesAsyncDefault();
        }

        /* CONTINUE */

        public void ContinueCollectionLoadAsync(string label) {
            ContinueCollectionLoadAsync (label, _sceneAssetType);
        }

        public void ContinueCollectionLoadAsync(int index) {
            ContinueCollectionLoadAsync (index, _sceneAssetType);
        }

        public void ContinueCollectionLoadAsync(string label, AssetManagementType sceneAssetType) {
            switch (sceneAssetType) {
                case AssetManagementType.Default:
                    ContinueCollectionLoadAsyncDefault (label);
                    break;
                case AssetManagementType.Addressables:
                    ContinueCollectionLoadAsyncAddressables (label);
                    break;
                default:
                    break;
            }
        }

        public void ContinueCollectionLoadAsync(int index, AssetManagementType sceneAssetType) {
            switch (sceneAssetType) {
                case AssetManagementType.Default:
                    ContinueCollectionLoadAsyncDefault (index);
                    break;
                case AssetManagementType.Addressables:
                    ContinueCollectionLoadAsyncAddressables (index);
                    break;
                default:
                    break;
            }
        }

        public void ContinueCollectionLoadAsyncDefault(SceneCollectionSO sceneCollection) {
            sceneCollection?.ContinueLoadAsyncDefault();
        }

        public void ContinueCollectionLoadAsyncAddressables(SceneCollectionSO sceneCollection) {
            sceneCollection?.ContinueLoadAsyncAddressables();
        }

        public void ContinueCollectionLoadAsyncDefault(int index) {
            ContinueCollectionLoadAsyncDefault(GetSceneCollectionAt(index));
        }

        public void ContinueCollectionLoadAsyncDefault(string label) {
            ContinueCollectionLoadAsyncDefault(GetSceneCollection(label));
        }

        public void ContinueCollectionLoadAsyncAddressables(string label) {
            ContinueCollectionLoadAsyncAddressables(GetSceneCollection(label));
        }

        public void ContinueCollectionLoadAsyncAddressables(int index) {
            ContinueCollectionLoadAsyncAddressables(GetSceneCollectionAt(index));
        }

        /* UNLOAD */

        public void UnloadSceneCollectionAsync(string label) {
            UnloadSceneCollectionAsync (label, _sceneAssetType);
        }

        public void UnloadSceneCollectionAsync(int index) {
            UnloadSceneCollectionAsync (index, _sceneAssetType);
        }

        public void UnloadSceneCollectionAsync(int index, AssetManagementType sceneAssetType) {
            switch (sceneAssetType) {
                case AssetManagementType.Default:
                    UnloadSceneCollectionAsyncDefault (index);
                    break;
                case AssetManagementType.Addressables:
                    UnloadSceneCollectionAsyncAddressables (index);
                    break;
                default:
                    break;
            }
        }

        public void UnloadSceneCollectionAsync(string label, AssetManagementType sceneAssetType) {
            switch (sceneAssetType) {
                case AssetManagementType.Default:
                    UnloadSceneCollectionAsyncDefault (label);
                    break;
                case AssetManagementType.Addressables:
                    UnloadSceneCollectionAsyncAddressables (label);
                    break;
                default:
                    break;
            }
        }

        public void UnloadSceneCollectionAsyncDefault(SceneCollectionSO sceneCollection) {
            sceneCollection?.UnloadScenesAsyncDefault();
        }

        public void UnloadSceneCollectionAsyncAddressables(SceneCollectionSO sceneCollection) {
            sceneCollection?.UnloadScenesAsyncAddressables();
        }

        public void UnloadSceneCollectionAsyncDefault(int index) {
            UnloadSceneCollectionAsyncDefault(GetSceneCollectionAt(index));
        }

        public void UnloadSceneCollectionAsyncDefault(string label) {
            UnloadSceneCollectionAsyncDefault(GetSceneCollection(label));
        }

        public void UnloadSceneCollectionAsyncAddressables(string label) {
            UnloadSceneCollectionAsyncAddressables(GetSceneCollection(label));
        }

        public void UnloadSceneCollectionAsyncAddressables(int index) {
            UnloadSceneCollectionAsyncAddressables(GetSceneCollectionAt(index));
        }
    }
}