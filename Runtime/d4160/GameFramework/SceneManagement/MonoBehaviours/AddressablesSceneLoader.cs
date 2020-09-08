using System;
using System.Collections;
using d4160.Core;
using d4160.Core.MonoBehaviours;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace d4160.GameFramework.SceneManagement
{

    public class AddressablesSceneLoader : MonoBehaviour
    {
        public static bool LoadScene(AssetReference sceneReference, LoadSceneMode loadSceneMode, bool allowDuplicates,
            bool activateOnLoad = true, Action<AsyncOperationHandle<SceneInstance>> completed = null,
            Action<AsyncOperationHandle> completedTypeless = null, Action<float> onProgress = null,
            Action<AsyncOperationHandle> destroyed = null, int priority = 100)
        {
            if (sceneReference == null)
                return false;

            if (!allowDuplicates)
            {
                if (MySceneManager.IsLoaded(sceneReference.AssetGUID))
                {

                    return false;
                }
            }

            CoroutineManager.Instance.StartCoroutine(LoadSceneCo(sceneReference, loadSceneMode, activateOnLoad,
                completed, completedTypeless, onProgress, destroyed, priority));

            return true;
        }

        protected static IEnumerator LoadSceneCo(AssetReference sceneReference, LoadSceneMode loadSceneMode,
            bool activateOnLoad = true, Action<AsyncOperationHandle<SceneInstance>> completed = null,
            Action<AsyncOperationHandle> completedTypeless = null, Action<float> onProgress = null,
            Action<AsyncOperationHandle> destroyed = null, int priority = 100)
        {
            var asyncOpHandle = sceneReference.LoadSceneAsync(loadSceneMode, activateOnLoad, priority);

            // Complete callbacks are called just before last "Loading 100%" is called
            asyncOpHandle.Completed += completed;
            asyncOpHandle.CompletedTypeless += completedTypeless;

            // In some strange situations maybe?
            asyncOpHandle.Destroyed += destroyed;

            while (!asyncOpHandle.IsDone)
            {
                yield return null;
                onProgress?.Invoke(asyncOpHandle.PercentComplete);
            }
        }

        protected static void LoadSceneRecursive(SceneListDefinition sceneListDefinition, int idx, bool allowDuplicates,
            bool activateOnLoad = true, Action<AsyncOperationHandle<SceneInstance>, float> partialCompleted = null,
            Action<AsyncOperationHandle, float> partialCompletedTypeless = null, Action completed = null,
            Action<float> onProgress = null, Action<AsyncOperationHandle> destroyed = null, int priority = 100)
        {
            var sceneList = sceneListDefinition.Scenes;

            if (!sceneList.IsValidIndex(idx))
                return;

            var asyncOpHandles = sceneListDefinition.LoadingAsyncOperationHandles;
            var loadedScenes = sceneListDefinition.LoadedScenes;
            var currentPercentage = 1.0f / sceneList.Length * (idx + 1);
            var sceneReference = sceneList[idx];

            var result = LoadScene(sceneReference, LoadSceneMode.Additive, allowDuplicates, activateOnLoad,
                (ao) =>
                {
                    bool setAsActiveScene = sceneListDefinition.SceneToSetAsMainActive == sceneReference.AssetGUID;

                    if (!activateOnLoad)
                    {
                        asyncOpHandles.Enqueue(new SceneActivationHandle(){ asyncOpHandle = ao , setAsActiveScene = setAsActiveScene });
                    }
                    else
                    {
                        if(setAsActiveScene)
                            SceneManager.SetActiveScene(ao.Result.Scene);
                    }

                    partialCompleted?.Invoke(ao, currentPercentage);

                    MySceneManager.RegisterLoadedScene(sceneReference.AssetGUID);
                    loadedScenes.Push(new SceneGuid(){scene = ao.Result.Scene, guid = sceneReference.AssetGUID });

                    if (!sceneList.IsValidIndex(idx + 1))
                    {
                        sceneListDefinition.LoadsCount++;

                        if (activateOnLoad)
                            sceneListDefinition.ActivatesCount++;

                        completed?.Invoke();
                    }

                    LoadSceneRecursive(sceneListDefinition, idx + 1, allowDuplicates, activateOnLoad, partialCompleted,
                        partialCompletedTypeless, completed, onProgress, destroyed, priority);
                }, (ao) => { partialCompletedTypeless?.Invoke(ao, currentPercentage); }, onProgress, destroyed,
                priority);

            if (!result)
            {
                if (!sceneList.IsValidIndex(idx + 1))
                {
                    sceneListDefinition.LoadsCount++;

                    if (activateOnLoad)
                        sceneListDefinition.ActivatesCount++;

                    completed?.Invoke();
                }

                LoadSceneRecursive(sceneListDefinition, idx + 1, allowDuplicates, activateOnLoad, partialCompleted,
                    partialCompletedTypeless, completed, onProgress, destroyed, priority);
            }
        }

        public static void LoadScenesInParallel(SceneListDefinition sceneListDefinition, bool allowDuplicates, bool activateOnLoad = true,
            Action<AsyncOperationHandle<SceneInstance>, float> partialCompleted = null,
            Action<AsyncOperationHandle, float> partialCompletedTypeless = null, Action completed = null,
            Action<float> onProgress = null, Action<AsyncOperationHandle> destroyed = null, int priority = 100)
        {
            var sceneList = sceneListDefinition.Scenes;
            var asyncOpHandles = sceneListDefinition.LoadingAsyncOperationHandles;
            var loadedScenes = sceneListDefinition.LoadedScenes;
            var length = sceneList.Length;
            var count = 0;
            var percentageStep = 1.0f / length;
            var currentPercentage = percentageStep;

            for (int i = 0; i < sceneList.Length; i++)
            {
                var sceneReference = sceneList[i];

                if (sceneReference == null)
                {
                    length--;

                    if (count >= length)
                        completed?.Invoke();

                    continue;
                }

                if (!allowDuplicates)
                {
                    if (MySceneManager.IsLoaded(sceneReference.AssetGUID))
                    {
                        continue;
                    }
                }

                CoroutineManager.Instance.StartCoroutine(
                    LoadSceneCo(
                        sceneReference,
                        LoadSceneMode.Additive,
                        activateOnLoad,
                        (ao) =>
                        {
                            bool setAsActiveScene = sceneListDefinition.SceneToSetAsMainActive == sceneReference.AssetGUID;

                            if (!activateOnLoad)
                            {
                                asyncOpHandles.Enqueue(new SceneActivationHandle(){ asyncOpHandle = ao , setAsActiveScene = setAsActiveScene });
                            }
                            else
                            {
                                if (setAsActiveScene)
                                    SceneManager.SetActiveScene(ao.Result.Scene);
                            }

                            partialCompleted?.Invoke(ao, currentPercentage);

                            count++;

                            // Last invoked is non typeless
                            currentPercentage += percentageStep;

                            MySceneManager.RegisterLoadedScene(sceneReference.AssetGUID);
                            loadedScenes.Push(new SceneGuid(){ scene = ao.Result.Scene, guid = sceneReference.AssetGUID });

                            if (count >= length)
                            {
                                sceneListDefinition.LoadsCount++;

                                if (activateOnLoad)
                                    sceneListDefinition.ActivatesCount++;

                                completed?.Invoke();
                            }
                        },
                        (ao) => { partialCompletedTypeless?.Invoke(ao, currentPercentage); },
                        onProgress,
                        destroyed,
                        priority));
            }
        }

        public static void LoadScenesAfterAnother(SceneListDefinition sceneListDefinition, bool allowDuplicates, bool activateOnLoad = true,
            Action<AsyncOperationHandle<SceneInstance>, float> partialCompleted = null,
            Action<AsyncOperationHandle, float> partialCompletedTypeless = null, Action completed = null,
            Action<float> onProgress = null, Action<AsyncOperationHandle> destroyed = null, int priority = 100)
        {
            LoadSceneRecursive(sceneListDefinition, 0, allowDuplicates, activateOnLoad, partialCompleted, partialCompletedTypeless,
                completed, onProgress, destroyed,
                priority);
        }

        protected static IEnumerator ActivateSceneCo(SceneInstance scene, Action<AsyncOperation> completed = null,
            Action<float> onProgress = null)
        {
            var asyncOp = scene.ActivateAsync();

            // Completed callback is called before last "Activating 100%" is called
            asyncOp.completed += completed;

            while (!asyncOp.isDone)
            {
                yield return null;
                onProgress?.Invoke(asyncOp.progress);
            }
        }

        public static void ActivateScene(SceneInstance sceneInstance, Action<AsyncOperation> completed = null,
            Action<float> onProgress = null)
        {
            CoroutineManager.Instance.StartCoroutine(ActivateSceneCo(sceneInstance, completed, onProgress));
        }

        protected static void ActivateSceneRecursive(SceneListDefinition sceneListDefinition, int count,
            Action<AsyncOperation, float> partialCompleted = null, Action completed = null,
            Action<float> onProgress = null)
        {
            if (count <= 0)
                return;

            var asyncOpHandles = sceneListDefinition.LoadingAsyncOperationHandles;

            var currentPercentage = 1.0f / count;

            var activationHandle = asyncOpHandles.Dequeue();

            while (!activationHandle.asyncOpHandle.IsDone)
            {
                asyncOpHandles.Enqueue(activationHandle);

                activationHandle = asyncOpHandles.Dequeue();

                count--;

                currentPercentage = 1.0f / count;

                if (count <= 0)
                    return;
            }

            ActivateScene(
                activationHandle.asyncOpHandle.Result,
                (ao) =>
                {
                    partialCompleted?.Invoke(ao, currentPercentage);

                    if (activationHandle.setAsActiveScene)
                        SceneManager.SetActiveScene(activationHandle.asyncOpHandle.Result.Scene);

                    if (count - 1 <= 0)
                    {
                        sceneListDefinition.ActivatesCount++;

                        completed?.Invoke();
                    }

                    ActivateSceneRecursive(sceneListDefinition, count - 1, partialCompleted, completed, onProgress);
                },
                onProgress);
        }

        public static void ActivateScenesInParallel(SceneListDefinition sceneListDefinition,
            Action<AsyncOperation, float> partialCompleted = null, Action completed = null,
            Action<float> onProgress = null)
        {
            var asyncOpHandles = sceneListDefinition.LoadingAsyncOperationHandles;

            int initialLength = asyncOpHandles.Count;
            int correctsLength = initialLength;
            int correctsCount = 0;
            var percentageStep = 1.0f / correctsLength;
            var currentPercentage = percentageStep;

            for (int i = 0; i < initialLength; i++)
            {
                var activationHandle = asyncOpHandles.Dequeue();

                if (!activationHandle.asyncOpHandle.IsDone)
                {
                    asyncOpHandles.Enqueue(activationHandle);
                    correctsLength--;

                    if (correctsCount >= correctsLength)
                    {
                        sceneListDefinition.ActivatesCount++;

                        completed?.Invoke();
                    }

                    continue;
                }

                ActivateScene(activationHandle.asyncOpHandle.Result, (ao) =>
                {
                    partialCompleted?.Invoke(ao, currentPercentage);

                    if (activationHandle.setAsActiveScene)
                        SceneManager.SetActiveScene(activationHandle.asyncOpHandle.Result.Scene);

                    currentPercentage += percentageStep;

                    correctsCount++;
                    if (correctsCount >= correctsLength)
                    {
                        sceneListDefinition.ActivatesCount++;

                        completed?.Invoke();
                    }
                }, onProgress);
            }
        }

        public static void ActivateScenesAfterAnother(SceneListDefinition sceneListDefinition,
            Action<AsyncOperation, float> partialCompleted = null, Action completed = null,
            Action<float> onProgress = null)
        {
            var asyncOpHandles = sceneListDefinition.LoadingAsyncOperationHandles;
            int handlesCount = asyncOpHandles.Count;

            ActivateSceneRecursive(sceneListDefinition, handlesCount, partialCompleted, completed, onProgress);
        }

        protected static IEnumerator UnloadSceneCo(Scene scene, Action<AsyncOperation> completed = null,
            Action<float> onProgress = null)
        {
            var asyncOp = SceneManager.UnloadSceneAsync(scene);

            // Completed callback is called before last "Activating 100%" is called
            asyncOp.completed += completed;

            while (!asyncOp.isDone)
            {
                yield return null;
                onProgress?.Invoke(asyncOp.progress);
            }
        }

        public static void UnloadScene(Scene scene, Action<AsyncOperation> completed = null,
            Action<float> onProgress = null)
        {
            CoroutineManager.Instance.StartCoroutine(UnloadSceneCo(scene, completed, onProgress));
        }

        protected static IEnumerator UnloadSceneCo(AssetReference sceneReference, Action<AsyncOperationHandle<SceneInstance>> completed = null,
            Action<AsyncOperationHandle> completedTypeless = null, Action<float> onProgress = null,
            Action<AsyncOperationHandle> destroyed = null)
        {
            var asyncOp = sceneReference.UnLoadScene();

            // Completed callback is called before last "Activating 100%" is called
            asyncOp.Completed += completed;
            asyncOp.CompletedTypeless += completedTypeless;

            asyncOp.Destroyed += destroyed;

            while (!asyncOp.IsDone)
            {
                yield return null;
                onProgress?.Invoke(asyncOp.PercentComplete);
            }
        }

        public static void UnloadScene(AssetReference sceneReference, Action<AsyncOperationHandle<SceneInstance>> completed = null,
            Action<AsyncOperationHandle> completedTypeless = null, Action<float> onProgress = null,
            Action<AsyncOperationHandle> destroyed = null)
        {
            CoroutineManager.Instance.StartCoroutine(UnloadSceneCo(sceneReference, completed, completedTypeless, onProgress, destroyed));
        }

        protected static IEnumerator UnloadSceneCo(string sceneName, Action<AsyncOperation> completed = null,
            Action<float> onProgress = null)
        {
            var asyncOp = SceneManager.UnloadSceneAsync(sceneName);

            // Completed callback is called before last "Activating 100%" is called
            asyncOp.completed += completed;

            while (!asyncOp.isDone)
            {
                yield return null;
                onProgress?.Invoke(asyncOp.progress);
            }
        }

        public static void UnloadScene(string sceneName, Action<AsyncOperation> completed = null,
            Action<float> onProgress = null)
        {
            CoroutineManager.Instance.StartCoroutine(UnloadSceneCo(sceneName, completed, onProgress));
        }

        protected static IEnumerator UnloadSceneCo(int sceneBuildIdx, Action<AsyncOperation> completed = null,
            Action<float> onProgress = null)
        {
            var asyncOp = SceneManager.UnloadSceneAsync(sceneBuildIdx);

            // Completed callback is called before last "Activating 100%" is called
            asyncOp.completed += completed;

            while (!asyncOp.isDone)
            {
                yield return null;
                onProgress?.Invoke(asyncOp.progress);
            }
        }

        public static void UnloadScene(int sceneBuildIdx, Action<AsyncOperation> completed = null,
            Action<float> onProgress = null)
        {
            CoroutineManager.Instance.StartCoroutine(UnloadSceneCo(sceneBuildIdx, completed, onProgress));
        }

        protected static void UnloadSceneRecursive(SceneListDefinition sceneListDefinition, int count, Action<AsyncOperation, float> partialCompleted = null,
            Action completed = null, Action<float> onProgress = null)
        {
            if (count <= 0)
                return;

            var loadedScenes = sceneListDefinition.LoadedScenes;

            var currentPercentage = 1.0f / count;

            var sceneGuid = loadedScenes.Pop();

            while (!sceneGuid.scene.isLoaded)
            {
                sceneGuid = loadedScenes.Pop();

                count--;

                currentPercentage = 1.0f / count;

                if (count <= 0)
                    return;
            }

            UnloadScene(sceneGuid.scene, (ao) =>
                {
                    partialCompleted?.Invoke(ao, currentPercentage);

                    MySceneManager.UnregisterLoadedScene(sceneGuid.guid);

                    if (count - 1 <= 0)
                    {
                        sceneListDefinition.LoadsCount--;
                        sceneListDefinition.ActivatesCount--;
                        completed?.Invoke();
                    }

                    UnloadSceneRecursive(sceneListDefinition, count - 1, partialCompleted, completed, onProgress);
                },
                onProgress);
        }

        public static void UnloadScenesInParallel(SceneListDefinition sceneListDefinition,
            Action<AsyncOperation, float> partialCompleted = null, Action completed = null,
            Action<float> onProgress = null)
        {
            var loadedScenes = sceneListDefinition.LoadedScenes;

            int initialLength = loadedScenes.Count;
            int correctsLength = initialLength;
            int correctsCount = 0;
            var percentageStep = 1.0f / correctsLength;
            var currentPercentage = percentageStep;

            for (int i = 0; i < initialLength; i++)
            {
                var sceneGuid = loadedScenes.Pop();

                if (!sceneGuid.scene.isLoaded)
                {
                    correctsLength--;

                    if (correctsCount >= correctsLength)
                        completed?.Invoke();

                    continue;
                }

                UnloadScene(sceneGuid.scene, (ao) =>
                {
                    partialCompleted?.Invoke(ao, currentPercentage);

                    MySceneManager.UnregisterLoadedScene(sceneGuid.guid);

                    currentPercentage += percentageStep;

                    correctsCount++;

                    if (correctsCount >= correctsLength)
                    {
                        sceneListDefinition.LoadsCount--;
                        sceneListDefinition.ActivatesCount--;
                        completed?.Invoke();
                    }
                },
                    onProgress);
            }
        }

        public static void UnloadScenesAfterAnother(SceneListDefinition sceneListDefinition,
            Action<AsyncOperation, float> partialCompleted = null, Action completed = null,
            Action<float> onProgress = null)
        {
            var scenes = sceneListDefinition.Scenes;
            int scenesCount = scenes.Count;

            UnloadSceneRecursive(sceneListDefinition, scenesCount, partialCompleted, completed, onProgress);
        }
    }
}