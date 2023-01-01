using d4160.Core;
using d4160.MonoBehaviours;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UnityEngine;

namespace d4160.SceneManagement {
    public class SceneLinkBehaviour : MonoBehaviourUnityData<SceneLinkSO> {
        [SerializeField] private UnityLifetimeMethodType _continueLoadAt;
        
        void Awake () {
            if (_continueLoadAt == UnityLifetimeMethodType.Awake) {
                ContinueLoadAsync ();
            }
        }

        void Start () {
            if (_continueLoadAt == UnityLifetimeMethodType.Start) {
                ContinueLoadAsync ();
            }
        }

        void OnEnable () {
            if (_continueLoadAt == UnityLifetimeMethodType.OnEnable) {
                ContinueLoadAsync ();
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ContinueLoadAsync () {
            _data.ContinueLoadAsync();
        }

        public void ContinueLoadAsync (AssetManagementType sceneAssetType) {
            _data.ContinueLoadAsync(sceneAssetType);
        }

        public void ContinueLoadAsyncDefault () {
            _data.ContinueLoadAsyncDefault ();
        }

#if ADDRESSABLES
        public void ContinueLoadAsyncAddressables () {
            _data.ContinueLoadAsyncAddressables ();
        }
#endif
    }
}