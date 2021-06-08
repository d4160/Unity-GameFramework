using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace d4160.SceneManagement {
    [System.Serializable]
    public struct SceneReference : ISerializationCallbackReceiver {
#if UNITY_EDITOR 
        [SerializeField] private UnityEditor.SceneAsset _sceneAsset;
        [SerializeField] private UnityEditor.SceneAsset _prevSceneAsset;

        public UnityEditor.SceneAsset SceneAsset => _sceneAsset;
#endif
        public string scenePath;
        public string sceneGUID;

        public bool IsNull => string.IsNullOrEmpty (scenePath) || string.IsNullOrEmpty (sceneGUID);

        private AsyncOperation _sceneOperation;
        public AsyncOperation SceneOperationHandle => _sceneOperation;

        /// <summary>
        /// Get the loading status of the internal operation.
        /// </summary>
        public bool IsDone => _sceneOperation?.isDone ?? false;

        public AsyncOperation LoadSceneAsync (LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true) {
            if (IsDone)
                Debug.LogError ("Attempting to load Scene that has already been loaded. Operation is exposed through getter SceneOperation");
            else {
                _sceneOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (scenePath, loadMode);
                _sceneOperation.allowSceneActivation = activateOnLoad;
            }

            return _sceneOperation;
        }

        public void UnloadSceneAsync () {
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync (scenePath);
            _sceneOperation = null;
        }

        public void Clear () {
            _sceneOperation = null;
        }

        private AsyncOperationHandle<SceneInstance> _addressablesOperation;
        /// <summary>
        /// The AsyncOperationHandle currently being used by the AssetReference.
        /// For example, if you call AssetReference.LoadAssetAsync, this property will return a handle to that operation.
        /// </summary>
        public AsyncOperationHandle<SceneInstance> AddressablesOperationHandle => _addressablesOperation;

        public bool IsAddresssableDone => _addressablesOperation.IsDone;

        /// <summary>
        /// Returns the state of the internal operation.
        /// </summary>
        /// <returns>True if the operation is valid.</returns>
        public bool IsAddressableValid => _addressablesOperation.IsValid ();

        /// <summary>
        /// Loads the reference as a scene.
        /// </summary>
        /// <param name="loadMode">Scene load mode.</param>
        /// <param name="activateOnLoad">If false, the scene will load but not activate (for background loading).  The SceneInstance returned has an Activate() method that can be called to do this at a later point.</param>
        /// <param name="priority">Async operation priority for scene loading.</param>
        /// <returns>The operation handle for the request if there is not a valid cached operation, otherwise return default operation</returns>
        public AsyncOperationHandle<SceneInstance> LoadSceneAsyncAddressables (LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100) {
            AsyncOperationHandle<SceneInstance> result = default;
            if (_addressablesOperation.IsValid ())
                Debug.LogError ("Attempting to load Scene that has already been loaded. Handle is exposed through getter OperationHandle");
            else {
                result = Addressables.LoadSceneAsync(sceneGUID, loadMode, activateOnLoad, priority);
                _addressablesOperation = result;
            }
            return result;
        }

        /// <summary>
        /// Unloads the reference as a scene.
        /// </summary>
        /// <returns>The operation handle for the scene load.</returns>
        public AsyncOperationHandle<SceneInstance> UnloadSceneAsyncAddressables () {
            return Addressables.UnloadSceneAsync (_addressablesOperation);
        }

        public void OnBeforeSerialize () {
#if UNITY_EDITOR
            if (_sceneAsset && _sceneAsset != _prevSceneAsset) {
                scenePath = UnityEditor.AssetDatabase.GetAssetOrScenePath (_sceneAsset);
                sceneGUID = UnityEditor.AssetDatabase.AssetPathToGUID (scenePath);
            } else {
                scenePath = string.Empty;
                sceneGUID = string.Empty;
            }
#endif
        }

        public void OnAfterDeserialize () { }
    }
}