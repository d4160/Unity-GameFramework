using System.Collections;
using System.Collections.Generic;
using System.Linq;
using d4160.Coroutines;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
using InspectInLine;
#endif
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
#if ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif
using UnityEngine.SceneManagement;
using d4160.Core;
using System;

namespace d4160.SceneManagement {
    [CreateAssetMenu (menuName = "d4160/SceneManagement/Scene Collection")]
    public class SceneCollectionSO : ScriptableObject {
        [SerializeField] private string _label;
        [SerializeField] [TextArea] private string _description;
        [SerializeField] private AssetManagementType _sceneAssetType;

        [SerializeField] [Space] private SceneReference _loadingScene;
        [SerializeField] private SceneLinkSO _loadingSceneInfo;

        [Space][ContextMenuItem ("Open Pack Scenes Single", "OpenPackScenesSingle")]
        [ContextMenuItem ("Open Pack Scenes Additive", "OpenPackScenesAdditive")]
        [SerializeField] private SceneReference[] _sceneCollection;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [DropdownIndex ("SceneNames")]
#endif
        [SerializeField] private int _activableScene;
#if ENABLE_NAUGHTY_ATTRIBUTES

        [InspectInline (canEditRemoteTarget = true)]
#endif
        [SerializeField] [Space] private ScriptableObject _additionalData;

        public event Action<int, string> OnCollectionLoaded;

        private SceneManagerSO _sceneManager;
        private int _managerIndex;
        private int _loadedCount;
        private int _loadedCountTarget;

        public string Label => _label;
        public SceneManagerSO SceneManager { get => _sceneManager;  set => _sceneManager = value; }
        public int ManagerIndex { get => _managerIndex; set => _managerIndex = value; }

        private readonly List<AsyncOperation> _sceneOperations = new List<AsyncOperation> ();
#if ADDRESSABLES
        private readonly List<AsyncOperationHandle<SceneInstance>> _addressablesOperation = new List<AsyncOperationHandle<SceneInstance>> ();
#endif
        public List<AsyncOperation> SceneOperationHandles => _sceneOperations;
#if ADDRESSABLES
        public List<AsyncOperationHandle<SceneInstance>> AddressablesOperationHandles => _addressablesOperation;
#endif

#region UNITY_EDITOR
#if UNITY_EDITOR
        private string[] SceneNames => _sceneCollection?.Select (x => x.SceneAsset != null ? x.SceneAsset.name : "- NULL -").ToArray ();

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        private void OpenPackScenesSingle () {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ()) {
                for (int i = 0; i < _sceneCollection.Length; i++) {
                    EditorSceneManager.OpenScene (_sceneCollection[i].scenePath,
                        i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
                }

                UnityEngine.SceneManagement.SceneManager.SetActiveScene (UnityEngine.SceneManagement.SceneManager.GetSceneByPath (_sceneCollection[_activableScene].scenePath));
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        private void OpenPackScenesAdditive () {
            for (int i = 0; i < _sceneCollection.Length; i++) {
                EditorSceneManager.OpenScene (_sceneCollection[i].scenePath, OpenSceneMode.Additive);
            }

            UnityEngine.SceneManagement.SceneManager.SetActiveScene (UnityEngine.SceneManagement.SceneManager.GetSceneByPath (_sceneCollection[_activableScene].scenePath));
        }
#endif
#endregion

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void LoadScenesAsync () {
            switch (_sceneAssetType) {
                case AssetManagementType.Default:
                    LoadScenesAsyncDefault ();
                    break;
#if ADDRESSABLES
                case AssetManagementType.Addressables:
                    LoadScenesAsyncAddressables ();
                    break;
#endif
                default:
                    break;
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ContinueLoadAsync () {
            switch (_sceneAssetType) {
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

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void UnloadScenesAsync () {
            switch (_sceneAssetType) {
                case AssetManagementType.Default:
                    UnloadScenesAsyncDefault ();
                    break;
#if ADDRESSABLES
                case AssetManagementType.Addressables:
                    UnloadScenesAsyncAddressables ();
                    break;
#endif
                default:
                    break;
            }
        }

        public bool CompareLabel(string label) => _label == label;

        public void LoadScenesAsyncDefault (LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true) {
            if (!_loadingScene.IsNull) {
                LoadScenesAsyncRoutine (loadSceneMode, activateOnLoad).StartCoroutine ();
            } else {
                LoadScenesAsyncInternal (loadSceneMode, activateOnLoad);
            }
        }

        private IEnumerator LoadScenesAsyncRoutine (LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true) {
            _loadingScene.Clear ();
            
            if (_loadingSceneInfo) {
                
                _loadingSceneInfo.SceneCollection = this;
                _loadingSceneInfo.SceneAssetType = AssetManagementType.Default;
                _loadingSceneInfo.SetLoadInfo (loadSceneMode, activateOnLoad);
            }

            AsyncOperation loadingSceneOperation = _loadingScene.LoadSceneAsync (LoadSceneMode.Single);

            yield return loadingSceneOperation;

            while (!loadingSceneOperation.isDone) {
                yield return null;
            }

            // Call ContinueLoadAsync from SceneLinkSO
        }

        public void ContinueLoadAsyncDefault (LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true) {
            LoadScenesAsyncInternal (loadSceneMode, activateOnLoad);
        }

        private void LoadScenesAsyncInternal (LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true) {
            if (_sceneOperations.Count > 0) {
                ClearOperations ();
            }

            _loadedCountTarget = _sceneCollection.Length;
            _loadedCount = 0;
            _sceneOperations.Clear ();
            for (int i = 0; i < _loadedCountTarget; i++) {
                if (!activateOnLoad) {
                    AsyncOperation opNoLoad = _sceneCollection[i].LoadSceneAsync(i == 0 ? loadSceneMode : LoadSceneMode.Additive, false);
                    _sceneOperations.Add (opNoLoad);
                    opNoLoad.completed += OnSceneLoadedDefault;
                } else {
                    AsyncOperation opLoad = _sceneCollection[i].LoadSceneAsync(i == 0 ? loadSceneMode : LoadSceneMode.Additive);
                    _sceneOperations.Add (opLoad);
                    opLoad.completed += OnSceneLoadedDefault;
                }
            }
        }

        private void OnSceneLoadedDefault(AsyncOperation asyncOp) {
            _loadedCount++;

            if(_loadedCount >= _loadedCountTarget) {
                OnCollectionLoaded?.Invoke(_managerIndex, _label);
            }
        }

        public void UnloadScenesAsyncDefault () {
            for (int i = 0; i < _sceneCollection.Length; i++) {
                _sceneCollection[i].UnloadSceneAsync ();
            }
        }

        private void ClearOperations () {
            for (var i = 0; i < _sceneCollection.Length; i++) {
                _sceneCollection[i].Clear ();
            }
        }

#if ADDRESSABLES
        private void LoadScenesAsyncAddressables () {
            LoadScenesAsyncAddressables (LoadSceneMode.Single);
        }

        public void LoadScenesAsyncAddressables (LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100) {
            if (!_loadingScene.IsNull) {
                LoadScenesAsyncAddressablesRoutine (loadSceneMode, activateOnLoad).StartCoroutine ();
            } else {
                LoadScenesAsyncAddressablesInternal (loadSceneMode, activateOnLoad, priority);
            }
        }

        private IEnumerator LoadScenesAsyncAddressablesRoutine (LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100) {
            if (_loadingSceneInfo) {
                _loadingSceneInfo.SceneCollection = this;
                _loadingSceneInfo.SceneAssetType = AssetManagementType.Addressables;
                _loadingSceneInfo.SetLoadInfo (loadSceneMode, activateOnLoad);
            }

            AsyncOperationHandle<SceneInstance> loadingSceneOperation = _loadingScene.LoadSceneAsyncAddressables (LoadSceneMode.Single);

            yield return loadingSceneOperation;

            while (!loadingSceneOperation.IsDone) {
                yield return null;
            }

            // Call ContinueLoadAsyncAddressables from SceneLinkSO
        }

        public void ContinueLoadAsyncAddressables (LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100) {
            LoadScenesAsyncAddressablesInternal (loadSceneMode, activateOnLoad, priority);
        }

        private void LoadScenesAsyncAddressablesInternal (LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100) {
            _loadedCountTarget = _sceneCollection.Length;
            _loadedCount = 0;
            _addressablesOperation.Clear ();
            for (int i = 0; i < _loadedCountTarget; i++) {
                if (!activateOnLoad) {
                    AsyncOperationHandle<SceneInstance> opNoLoad = _sceneCollection[i]
                        .LoadSceneAsyncAddressables(i == 0 ? loadSceneMode : LoadSceneMode.Additive, false, priority);
                    _addressablesOperation.Add (opNoLoad);
                    if(opNoLoad.IsValid()) opNoLoad.CompletedTypeless += OnSceneLoadedAddressables;
                } else {
                    AsyncOperationHandle<SceneInstance> opLoad = _sceneCollection[i].LoadSceneAsyncAddressables(i == 0 ? loadSceneMode : LoadSceneMode.Additive, true, priority);
                    _addressablesOperation.Add (opLoad);
                    if(opLoad.IsValid()) opLoad.CompletedTypeless += OnSceneLoadedAddressables;
                }
            }
        }

        private void OnSceneLoadedAddressables(AsyncOperationHandle asyncOp) {
            _loadedCount++;

            if(_loadedCount >= _loadedCountTarget) {
                OnCollectionLoaded?.Invoke(_managerIndex, _label);
            }
        }

        public void UnloadScenesAsyncAddressables () {
            for (int i = 0; i < _sceneCollection.Length; i++) {
                _sceneCollection[i].UnloadSceneAsyncAddressables ();
            }
        }
#endif
    }
}