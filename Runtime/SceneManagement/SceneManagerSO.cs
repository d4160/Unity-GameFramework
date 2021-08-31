using System;
using d4160.Collections;
using d4160.Core;
using InspectInLine;
using NaughtyAttributes;
#if PHOTON_UNITY_NETWORKING
using Photon.Pun;
#endif
using UnityEngine;

namespace d4160.SceneManagement {
    [CreateAssetMenu (menuName = "d4160/SceneManagement/Scene Manager")]
    public partial class SceneManagerSO : ScriptableObject {

        [InspectInline (canEditRemoteTarget = true)]
        [SerializeField] private SceneCollectionSO[] _sceneCollections;
        [SerializeField] private AssetManagementType _sceneAssetType;
        // TODO Move to another SO
        [DropdownIndex("GetSceneCollectionNames")]
        [SerializeField] private int _selectedSceneCollection;

        private int _lastLoadedIndex;
        private string _lastLoadedLabel;

        public AssetManagementType SceneAssetType { get => _sceneAssetType; set => _sceneAssetType = value; }
        public int SelectedSceneCollection { get => _selectedSceneCollection; set => _selectedSceneCollection = value; }

        public event Action<int, string> OnCollectionLoaded;

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

        public SceneCollectionSO GetSceneCollectionAt(int index, out string label) {
            if (_sceneCollections.IsValidIndex(index)) {
                label = _sceneCollections[index].Label;
                return _sceneCollections[index];
            }

            label = string.Empty;
            return null;
        }

        public SceneCollectionSO GetSceneCollection(string label, out int index) {
            for (int i = 0; i < _sceneCollections.Length; i++) {
                SceneCollectionSO sceneCollec = _sceneCollections[i];
                if (sceneCollec.CompareLabel(label)) {
                    index = i;
                    return sceneCollec;
                }
            }

            index = -1;
            return null;
        }

        public void RegisterEvents(){
            RegisterToCollectionsEvents();
#if PHOTON_UNITY_NETWORKING
            PhotonNetwork.AutomaticallySyncScene = false;
            PhotonNetwork.NetworkingClient.OpResponseReceived += OnOperation;
            PhotonNetwork.AddCallbackTarget(this);
#endif
        }

        public void UnregisterEvents(){
            UnregisterToCollectionsEvents();
#if PHOTON_UNITY_NETWORKING
            PhotonNetwork.NetworkingClient.OpResponseReceived -= OnOperation;
            PhotonNetwork.RemoveCallbackTarget(this);
#endif
        }

        private void RegisterToCollectionsEvents(){
            for (int i = 0; i < _sceneCollections.Length; i++)
            {
                _sceneCollections[i].OnCollectionLoaded += OnCollectionLoadedCallback;
#if PHOTON_UNITY_NETWORKING
                _sceneCollections[i].OnCollectionLoaded += OnCollectionLoadedCallbackPhoton;
#endif
            }
        }

        private void UnregisterToCollectionsEvents(){
            for (int i = 0; i < _sceneCollections.Length; i++)
            {
                _sceneCollections[i].OnCollectionLoaded -= OnCollectionLoadedCallback;
#if PHOTON_UNITY_NETWORKING
                _sceneCollections[i].OnCollectionLoaded -= OnCollectionLoadedCallbackPhoton;
#endif
            }
        }

        [Button]
        public void LoadSceneCollectionAsync() {
            LoadSceneCollectionAsync (_selectedSceneCollection, _sceneAssetType);
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
            SceneCollectionSO scnCollecSO = GetSceneCollection(label, out int index);

            if (index == -1) {
                Debug.LogWarning($"Cannot load the level with index: {index}.");
                return;
            }
            else
            {
                _lastLoadedIndex = index;
                _lastLoadedLabel = label;
            }
            LoadSceneCollectionAsyncDefault(scnCollecSO);
        }

        public void LoadSceneCollectionsAsyncAddressables(string label) {
            SceneCollectionSO scnCollecSO = GetSceneCollection(label, out int index);
            
            if (index == -1) {
                Debug.LogWarning($"Cannot load the level with index: {index}.");
                return;
            }
            else {
                _lastLoadedIndex = index;
                _lastLoadedLabel = label;
            }
            LoadSceneCollectionAsyncAddressables(scnCollecSO);
        }

        public void LoadSceneCollectionAsyncDefault(int index) {
            SceneCollectionSO scnCollecSO = GetSceneCollectionAt(index, out string label);
            _lastLoadedIndex = index;
            _lastLoadedLabel = label;
            LoadSceneCollectionAsyncDefault(scnCollecSO);
        }

        public void LoadSceneCollectionAsyncAddressables(int index) {
            SceneCollectionSO scnCollecSO = GetSceneCollectionAt(index, out string label);
            _lastLoadedIndex = index;
            _lastLoadedLabel = label;
            LoadSceneCollectionAsyncAddressables(scnCollecSO);
        }

        public void LoadSceneCollectionAsyncAddressables(SceneCollectionSO sceneCollection) {
            sceneCollection.SceneManager = this;
            sceneCollection.ManagerIndex = _lastLoadedIndex;

            OnLevelLoadForPhoton();

            sceneCollection?.LoadScenesAsyncAddressables();
        }

        public void LoadSceneCollectionAsyncDefault(SceneCollectionSO sceneCollection) {
            sceneCollection.SceneManager = this;
            sceneCollection.ManagerIndex = _lastLoadedIndex;

            OnLevelLoadForPhoton();

            sceneCollection?.LoadScenesAsyncDefault();
        }

        private void OnLevelLoadForPhoton(){
            
#if PHOTON_UNITY_NETWORKING
            if (PhotonHandler.AppQuits)
            {
                return;
            }

            if (AutomaticallySyncScene)
            {
                SetLevelInPropsIfSynced(_lastLoadedIndex);
            }

            PhotonNetwork.IsMessageQueueRunning = false;
            _loadingLevelAndPausedNetwork = true;
#endif
        }

        private void OnCollectionLoadedCallback(int index, string label){
            OnCollectionLoaded?.Invoke(index, label);
        }

        /* CONTINUE */
        [Button]
        public void ContinueCollectionLoadAsync() {
            ContinueCollectionLoadAsync (_selectedSceneCollection, _sceneAssetType);
        }

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
            ContinueCollectionLoadAsyncDefault(GetSceneCollectionAt(index, out string label));
            _lastLoadedLabel = label;
            _lastLoadedIndex = index;
        }

        public void ContinueCollectionLoadAsyncDefault(string label) {
            ContinueCollectionLoadAsyncDefault(GetSceneCollection(label, out int index));
            _lastLoadedLabel = label;
            _lastLoadedIndex = index;
        }

        public void ContinueCollectionLoadAsyncAddressables(string label) {
            ContinueCollectionLoadAsyncAddressables(GetSceneCollection(label, out int index));
            _lastLoadedLabel = label;
            _lastLoadedIndex = index;
        }

        public void ContinueCollectionLoadAsyncAddressables(int index) {
            ContinueCollectionLoadAsyncAddressables(GetSceneCollectionAt(index, out string label));
            _lastLoadedLabel = label;
            _lastLoadedIndex = index;
        }

        /* UNLOAD */
        [Button]
        public void UnloadSceneCollectionAsync() {
            UnloadSceneCollectionAsync (_selectedSceneCollection, _sceneAssetType);
        }

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
            UnloadSceneCollectionAsyncDefault(GetSceneCollectionAt(index, out string label));
        }

        public void UnloadSceneCollectionAsyncDefault(string label) {
            UnloadSceneCollectionAsyncDefault(GetSceneCollection(label, out int index));
        }

        public void UnloadSceneCollectionAsyncAddressables(string label) {
            UnloadSceneCollectionAsyncAddressables(GetSceneCollection(label, out int index));
        }

        public void UnloadSceneCollectionAsyncAddressables(int index) {
            UnloadSceneCollectionAsyncAddressables(GetSceneCollectionAt(index, out string label));
        }
    }
}