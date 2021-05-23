using d4160.Core;
using d4160.Singleton;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.SceneManagement {
    public class SceneManager : Singleton<SceneManager> {

        [Expandable]
        [SerializeField] protected SceneManagerSO _sceneManagerAsset;
        [SerializeField] protected UnityLifetimeMethod _loadSceneAt;
        [DropdownIndex("SceneCollectionsNames")]
        [SerializeField] protected int _sceneCollection;

#if UNITY_EDITOR
        private string[] SceneCollectionsNames => _sceneManagerAsset?.GetSceneCollectionNames;
#endif

        /* LOAD */
        public static void LoadSceneCollectionAsync (string label) {
            SceneManager.Instance._sceneManagerAsset?.LoadSceneCollectionAsync(label);
        }

        public static void LoadSceneCollectionAsync (int index) {
            SceneManager.Instance._sceneManagerAsset?.LoadSceneCollectionAsync(index);
        }

        public static void LoadSceneCollectionAsync (string label, AssetManagementType sceneAssetType) {
            SceneManager.Instance._sceneManagerAsset?.LoadSceneCollectionAsync (label, sceneAssetType);
        }

        public static void LoadSceneCollectionAsync (int index, AssetManagementType sceneAssetType) {
            SceneManager.Instance._sceneManagerAsset?.LoadSceneCollectionAsync (index, sceneAssetType);
        }

        /* CONTINUE */
        public static void ContinueCollectionLoadAsync (string label) {
            SceneManager.Instance._sceneManagerAsset?.ContinueCollectionLoadAsync(label);
        }

        public static void ContinueCollectionLoadAsync (int index) {
            SceneManager.Instance._sceneManagerAsset?.ContinueCollectionLoadAsync(index);
        }

        public static void ContinueCollectionLoadAsync (string label, AssetManagementType sceneAssetType) {
            SceneManager.Instance._sceneManagerAsset?.ContinueCollectionLoadAsync (label, sceneAssetType);
        }

        public static void ContinueCollectionLoadAsync (int index, AssetManagementType sceneAssetType) {
            SceneManager.Instance._sceneManagerAsset?.ContinueCollectionLoadAsync (index, sceneAssetType);
        }

        /* UNLOAD */
        public static void UnloadSceneCollectionAsync (string label) {
            SceneManager.Instance._sceneManagerAsset?.UnloadSceneCollectionAsync(label);
        }

        public static void UnloadSceneCollectionAsync (int index) {
            SceneManager.Instance._sceneManagerAsset?.UnloadSceneCollectionAsync(index);
        }

        public static void UnloadSceneCollectionAsync (string label, AssetManagementType sceneAssetType) {
            SceneManager.Instance._sceneManagerAsset?.UnloadSceneCollectionAsync (label, sceneAssetType);
        }

        public static void UnloadSceneCollectionAsync (int index, AssetManagementType sceneAssetType) {
            SceneManager.Instance._sceneManagerAsset?.UnloadSceneCollectionAsync (index, sceneAssetType);
        }

        protected override void Awake () {
            base.Awake();
            if (_loadSceneAt == UnityLifetimeMethod.Awake) {
                LoadSceneCollectionAsync(_sceneCollection);
            }
        }

        protected virtual void Start () {
            if (_loadSceneAt == UnityLifetimeMethod.Start) {
                LoadSceneCollectionAsync(_sceneCollection);
            }
        }

        protected virtual void OnEnable () {
            if (_loadSceneAt == UnityLifetimeMethod.OnEnable) {
                LoadSceneCollectionAsync(_sceneCollection);
            }
        }

        [Button]
        public void LoadSceneCollectionAsync () {
            LoadSceneCollectionAsync(_sceneCollection);
        }

        [Button]
        public void ContinueCollectionLoadAsync () {
            ContinueCollectionLoadAsync(_sceneCollection);
        }

        [Button]
        public void UnloadSceneCollectionAsync () {
            UnloadSceneCollectionAsync(_sceneCollection);
        }
    }
}