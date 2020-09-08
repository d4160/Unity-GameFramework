using System;
using System.Collections.Generic;
using System.Linq;
using d4160.Core;
using d4160.Utilities;
using DG.Tweening;
using Malee;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace d4160.GameFramework.SceneManagement
{
    [System.Serializable]
    public class SceneListDefinition
    {
        [SerializeField] protected bool _activeOnLoad;
        [SerializeField] protected bool _allowDuplicates;
        [Reorderable] [SerializeField] protected AssetReferenceArray _scenes;
        [Dropdown("GetAssetNames")] [SerializeField] protected string _sceneToSetAsMainActive;

#if UNITY_EDITOR
        public IDropdownList GetAssetNames()
        {
            var values = new DropdownList<string>()
            {
                { "None", string.Empty }
            };

            for (int i = 0; i < _scenes.Length; i++)
            {
                values.Add(_scenes[i].editorAsset.name, _scenes[i].AssetGUID);
            }

            return values;
        }
#endif

        [ShowNonSerializedField] protected int _loadsCount;
        [ShowNonSerializedField] protected int _activatesCount;

        protected readonly Queue<SceneActivationHandle> _loadingAsyncOpHandles =
            new Queue<SceneActivationHandle>();

        protected readonly Stack<SceneGuid> _loadedScenes = new Stack<SceneGuid>(); 

        public Queue<SceneActivationHandle> LoadingAsyncOperationHandles => _loadingAsyncOpHandles;
        public Stack<SceneGuid> LoadedScenes => _loadedScenes;
        public AssetReferenceArray Scenes => _scenes;

        public string SceneToSetAsMainActive => _sceneToSetAsMainActive;
        public bool ActiveOnLoad => _activeOnLoad;
        public bool AllowDuplicates => _allowDuplicates;

        public bool IsLoaded => _loadsCount > 0;
        public bool IsActived => _activatesCount > 0;
        public bool IsLoadedAndActived => IsActived && IsLoaded;

        public int LoadsCount
        {
            get => _loadsCount;
            set
            {
                if (value < 0)
                    value = 0;
                _loadsCount = value;
            }
        }

        public int ActivatesCount
        {
            get => _activatesCount;
            set
            {
                if (value < 0)
                    value = 0;
                _activatesCount = value;
            }
        }

        public AssetReference this[int idx] => _scenes.IsValidIndex(idx) ? _scenes[idx] : null;

        public void LoadScenesAfterAnother(Action<AsyncOperationHandle<SceneInstance>, float> partialCompleted = null,
            Action<AsyncOperationHandle, float> partialCompletedTypeless = null, Action completed = null,
            Action<float> onProgress = null, Action<AsyncOperationHandle> destroyed = null, int priority = 100)
        {
            if (!_allowDuplicates && IsLoaded)
                return;

            AddressablesSceneLoader.LoadScenesAfterAnother(
                this,
                _allowDuplicates,
                _activeOnLoad,
                partialCompleted,
                partialCompletedTypeless,
                completed,
                onProgress,
                destroyed,
                priority);
        }

        public void LoadScenesInParallel(Action<AsyncOperationHandle<SceneInstance>, float> partialCompleted = null,
            Action<AsyncOperationHandle, float> partialCompletedTypeless = null, Action completed = null,
            Action<float> onProgress = null, Action<AsyncOperationHandle> destroyed = null, int priority = 100)
        {
            if (!_allowDuplicates && IsLoaded)
                return;

            AddressablesSceneLoader.LoadScenesInParallel(
                this,
                _allowDuplicates,
                _activeOnLoad,
                partialCompleted,
                partialCompletedTypeless,
                completed,
                onProgress,
                destroyed,
                priority);
        }

        public void LoadScenesAfterAnother(bool activeOnLoad,
            Action<AsyncOperationHandle<SceneInstance>, float> partialCompleted = null,
            Action<AsyncOperationHandle, float> partialCompletedTypeless = null, Action completed = null,
            Action<float> onProgress = null, Action<AsyncOperationHandle> destroyed = null, int priority = 100)
        {
            if (!_allowDuplicates && IsLoaded)
                return;

            AddressablesSceneLoader.LoadScenesAfterAnother(
                this,
                _allowDuplicates,
                activeOnLoad,
                partialCompleted,
                partialCompletedTypeless,
                completed,
                onProgress,
                destroyed,
                priority);
        }

        public void LoadScenesInParallel(bool activeOnLoad,
            Action<AsyncOperationHandle<SceneInstance>, float> partialCompleted = null,
            Action<AsyncOperationHandle, float> partialCompletedTypeless = null, Action completed = null,
            Action<float> onProgress = null, Action<AsyncOperationHandle> destroyed = null, int priority = 100)
        {
            if (!_allowDuplicates && IsLoaded)
                return;

            AddressablesSceneLoader.LoadScenesInParallel(
                this,
                _allowDuplicates,
                activeOnLoad,
                partialCompleted,
                partialCompletedTypeless,
                completed,
                onProgress,
                destroyed,
                priority);
        }

        public void ActivateScenesInParallel(Action<AsyncOperation, float> partialCompleted = null,
            Action completed = null, Action<float> onProgress = null)
        {
            if(IsLoaded)
                AddressablesSceneLoader.ActivateScenesInParallel(this, partialCompleted, completed, onProgress);
        }

        public void ActivateScenesAfterAnother(Action<AsyncOperation, float> partialCompleted = null,
            Action completed = null, Action<float> onProgress = null)
        {
            if(IsLoaded)
                AddressablesSceneLoader.ActivateScenesAfterAnother(this, partialCompleted, completed, onProgress);
        }

        public void UnloadScenesInParallel(Action<AsyncOperation, float> partialCompleted = null,
            Action completed = null, Action<float> onProgress = null)
        {
            if (!IsLoadedAndActived)
                return;

            AddressablesSceneLoader.UnloadScenesInParallel(this, partialCompleted, completed, onProgress);
        }

        public void UnloadScenesAfterAnother(Action<AsyncOperation, float> partialCompleted = null,
            Action completed = null, Action<float> onProgress = null)
        {
            if (!IsLoadedAndActived)
                return;

            AddressablesSceneLoader.UnloadScenesAfterAnother(this, partialCompleted, completed, onProgress);
        }
    }

    [Serializable]
    public class AssetReferenceArray : ReorderableArray<AssetReference>
    {
    }

    public struct SceneGuid
    {
        public Scene scene;
        public string guid;
    }

    public struct SceneActivationHandle
    {
        public AsyncOperationHandle<SceneInstance> asyncOpHandle;
        public bool setAsActiveScene;
    }
}