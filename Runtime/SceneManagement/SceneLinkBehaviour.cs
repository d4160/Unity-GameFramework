using d4160.Core;
using d4160.MonoBehaviourData;
using NaughtyAttributes;
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

        [Button]
        public void ContinueLoadAsync () {
            _data.ContinueLoadAsync();
        }

        public void ContinueLoadAsync (AssetManagementType sceneAssetType) {
            _data.ContinueLoadAsync(sceneAssetType);
        }

        public void ContinueLoadAsyncDefault () {
            _data.ContinueLoadAsyncDefault ();
        }

        public void ContinueLoadAsyncAddressables () {
            _data.ContinueLoadAsyncAddressables ();
        }
    }
}