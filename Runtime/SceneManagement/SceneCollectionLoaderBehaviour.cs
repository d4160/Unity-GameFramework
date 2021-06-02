using d4160.Core;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.SceneManagement {
    public class SceneCollectionLoaderBehaviour : MonoBehaviourUnityData<SceneCollectionSO> {

        [SerializeField] private UnityLifetimeMethodType _loadSceneAt;

        void Awake () {
            if (_loadSceneAt == UnityLifetimeMethodType.Awake) {
                LoadScenesAsync ();
            }
        }

        void Start () {
            if (_loadSceneAt == UnityLifetimeMethodType.Start) {
                LoadScenesAsync ();
            }
        }

        void OnEnable () {
            if (_loadSceneAt == UnityLifetimeMethodType.OnEnable) {
                LoadScenesAsync ();
            }
        }

        [Button]
        public void LoadScenesAsync () {
            _data.LoadScenesAsync();
        }

        [Button]
        public void ContinueLoadAsync () {
            _data.ContinueLoadAsync();
        }

        [Button]
        public void UnloadScenesAsync () {
            _data.UnloadScenesAsync();
        }
    }
}