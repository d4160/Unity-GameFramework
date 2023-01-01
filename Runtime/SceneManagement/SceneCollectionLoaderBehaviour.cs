using d4160.Core;
using d4160.MonoBehaviours;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
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

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void LoadScenesAsync () {
            _data.LoadScenesAsync();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ContinueLoadAsync () {
            _data.ContinueLoadAsync();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void UnloadScenesAsync () {
            _data.UnloadScenesAsync();
        }
    }
}