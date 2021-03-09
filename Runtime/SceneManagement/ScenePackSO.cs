using System.Collections;
using System.Collections.Generic;
using System.Linq;
using d4160.Coroutines;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityExtensions;

namespace d4160.SceneManagement
{
    [CreateAssetMenu(menuName = "d4160/SceneManagement/Scene Pack")]
    public class ScenePackSO : ScriptableObject
    {
        public string label;
        [TextArea] public string description;

        [Space] public SceneReference loadingScene;
        public SceneInfoSO loadingSceneInfo;
        
        [Space][ContextMenuItem("Open Pack Scenes Single", "OpenPackScenesSingle")]
        [ContextMenuItem("Open Pack Scenes Additive", "OpenPackScenesAdditive")]
        public SceneReference[] packScenes;
        [DropdownIndex("SceneNames")]
        public int activableScene;

        [InspectInline(canEditRemoteTarget = true)]
        [Space] public ScriptableObject additionalData;

        private readonly List<AsyncOperation> _sceneOperations = new List<AsyncOperation>();
        private readonly List<AsyncOperationHandle<SceneInstance>> _addressablesOperation = new List<AsyncOperationHandle<SceneInstance>>();

        public List<AsyncOperation> SceneOperationHandles => _sceneOperations;
        public List<AsyncOperationHandle<SceneInstance>> AddressablesOperationHandles => _addressablesOperation;

#if UNITY_EDITOR
        private string[] SceneNames => packScenes.Select(x => x.SceneAsset != null ? x.SceneAsset.name : "- NULL -").ToArray();

        [Button]
        private void OpenPackScenesSingle()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                for (int i = 0; i < packScenes.Length; i++)
                {
                    EditorSceneManager.OpenScene(packScenes[i].scenePath,
                        i == 0 ? OpenSceneMode.Single : OpenSceneMode.Additive);
                }

                SceneManager.SetActiveScene(SceneManager.GetSceneByPath(packScenes[activableScene].scenePath));
            }
        }

        [Button]
        private void OpenPackScenesAdditive()
        {
            for (int i = 0; i < packScenes.Length; i++)
            {
                EditorSceneManager.OpenScene(packScenes[i].scenePath, OpenSceneMode.Additive);
            }

            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(packScenes[activableScene].scenePath));
        }
#endif

        [Button]
        private void LoadScenesAsync()
        {
            LoadScenesAsync(LoadSceneMode.Single);
        }

        public void LoadScenesAsync(LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            if (!loadingScene.IsNull)
            {
                LoadScenesAsyncRoutine(loadSceneMode, activateOnLoad).StartCoroutine();
            }
            else
            {
                LoadScenesAsyncInternal(loadSceneMode, activateOnLoad);
            }
        }

        private IEnumerator LoadScenesAsyncRoutine(LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            AsyncOperation loadingSceneOperation = loadingScene.LoadSceneAsync(LoadSceneMode.Additive);

            yield return loadingSceneOperation;

            while (!loadingSceneOperation.isDone)
            {
                yield return null;
            }

            if (loadingSceneInfo)
            {
                loadingSceneInfo.scenePack = this;
                loadingSceneInfo.SetLoadInfo(loadSceneMode, activateOnLoad);
            }

            // Call ContinueLoadAsync from SceneInfoSO
        }

        public void ContinueLoadAsync(LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            LoadScenesAsyncInternal(loadSceneMode, activateOnLoad);
        }

        private void LoadScenesAsyncInternal(LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
        {
            _sceneOperations.Clear();
            for (int i = 0; i < packScenes.Length; i++)
            {
                if (!activateOnLoad)
                {
                    _sceneOperations.Add(packScenes[i]
                        .LoadSceneAsync(i == 0 ? loadSceneMode : LoadSceneMode.Additive, false));
                }
                else
                {
                    packScenes[i].LoadSceneAsync(i == 0 ? loadSceneMode : LoadSceneMode.Additive);
                }
            }
        }

        [Button]
        public void UnloadScenesAsync()
        {
            for (int i = 0; i < packScenes.Length; i++)
            {
                packScenes[i].UnloadSceneAsync();
            }
        }

        [Button]
        private void LoadScenesAsyncAddressables()
        {
            LoadScenesAsyncAddressables(LoadSceneMode.Single);
        }

        public void LoadScenesAsyncAddressables(LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            if (!loadingScene.IsNull)
            {
                LoadScenesAsyncAddressablesRoutine(loadSceneMode, activateOnLoad).StartCoroutine();
            }
            else
            {
                LoadScenesAsyncAddressablesInternal(loadSceneMode, activateOnLoad, priority);
            }
        }

        private IEnumerator LoadScenesAsyncAddressablesRoutine(LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            AsyncOperationHandle<SceneInstance> loadingSceneOperation = loadingScene.LoadSceneAsyncAddressables(LoadSceneMode.Additive);

            yield return loadingSceneOperation;

            while (!loadingSceneOperation.IsDone)
            {
                yield return null;
            }
            
            if (loadingSceneInfo)
            {
                loadingSceneInfo.scenePack = this;
                loadingSceneInfo.SetLoadInfo(loadSceneMode, activateOnLoad);
            }

            // Call ContinueLoadAsyncAddressables from SceneInfoSO
        }
        
        public void ContinueLoadAsyncAddressables(LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            LoadScenesAsyncAddressablesInternal(loadSceneMode, activateOnLoad, priority);
        }

        private void LoadScenesAsyncAddressablesInternal(LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true, int priority = 100)
        {
            _addressablesOperation.Clear();
            for (int i = 0; i < packScenes.Length; i++)
            {
                if (!activateOnLoad)
                {
                    _addressablesOperation.Add(packScenes[i]
                        .LoadSceneAsyncAddressables(i == 0 ? loadSceneMode : LoadSceneMode.Additive, false, priority));
                }
                else
                {
                    packScenes[i].LoadSceneAsyncAddressables(i == 0 ? loadSceneMode : LoadSceneMode.Additive, true, priority);
                }
            }
        }

        [Button]
        public void UnloadScenesAsyncAddressables()
        {
            for (int i = 0; i < packScenes.Length; i++)
            {
                packScenes[i].UnloadSceneAsyncAddressables();
            }
        }
    }
}