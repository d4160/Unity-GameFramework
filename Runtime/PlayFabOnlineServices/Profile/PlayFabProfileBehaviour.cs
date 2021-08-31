#if PLAYFAB
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using PlayFab;
using PlayFab.ClientModels;
using UltEvents;
using UnityEngine;

namespace d4160.Profile.PlayFab {
    public class PlayFabProfileBehaviour : MonoBehaviourUnityData<PlayFabProfileSO> {

        [Header ("EVENTS")]
        [SerializeField] private UltEvent<GetPlayerProfileResult> _onGetPlayerProfile;
        [SerializeField] private UltEvent<UpdateUserTitleDisplayNameResult> _onUpdateDisplayName;
        [SerializeField] private UltEvent<AddOrUpdateContactEmailResult> _onAddOrUpdateContactEmail;
        [SerializeField] private UltEvent<RemoveContactEmailResult> _onRemoveContactEmail;
        [SerializeField] private UltEvent<PlayFabError> _onPlayFabError;

        public string DisplayName => _data?.DisplayName;

        void OnEnable() {
            if (_data) {
                _data.RegisterEvents();
                _data.OnGetPlayerProfile += _onGetPlayerProfile.Invoke;
                _data.OnUpdateDisplayName += _onUpdateDisplayName.Invoke;
                _data.OnAddOrUpdateContactEmail += _onAddOrUpdateContactEmail.Invoke;
                _data.OnRemoveContactEmail += _onRemoveContactEmail.Invoke;
                _data.OnPlayFabError += _onPlayFabError.Invoke;
            }
        }

        void OnDisable() {
            if (_data) {
                _data.UnregisterEvents();
                _data.OnGetPlayerProfile -= _onGetPlayerProfile.Invoke;
                _data.OnUpdateDisplayName -= _onUpdateDisplayName.Invoke;
                _data.OnAddOrUpdateContactEmail -= _onAddOrUpdateContactEmail.Invoke;
                _data.OnRemoveContactEmail -= _onRemoveContactEmail.Invoke;
                _data.OnPlayFabError -= _onPlayFabError.Invoke;
            }
        }

        [Button]
        public void UpdateDisplayName () {
            if (_data)
            {
                _data.UpdateDisplayName();
            }
        }

        [Button]
        public void GetPlayerProfile () {
            if (_data)
            {
                _data.GetPlayerProfile();
            }
        }

        [Button]
        public void UpdateAvatarUrl() {
            if (_data)
            {
                _data.UpdateAvatarUrl();
            }
        }

        [Button]
        public void AddOrUpdateContactEmail() {
            if (_data)
            {
                _data.AddOrUpdateContactEmail();
            }
        }

        [Button]
        public void RemoveContactEmail () {
            if (_data)
            {
                _data.RemoveContactEmail();
            }
        }
    }
}
#endif