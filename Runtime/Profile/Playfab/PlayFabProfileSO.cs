#if PLAYFAB
using System;
using NaughtyAttributes;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace d4160.Profile.PlayFab {

    [CreateAssetMenu (menuName = "d4160/Profile/PlayFab")]
    public class PlayFabProfileSO : ScriptableObject {

        [SerializeField] private string _displayName;
        [SerializeField] private string _email;

        public event Action<GetPlayerProfileResult> OnGetPlayerProfile;
        public event Action<UpdateUserTitleDisplayNameResult> OnUpdateDisplayName;
        public event Action<AddOrUpdateContactEmailResult> OnAddOrUpdateContactEmail;
        public event Action<RemoveContactEmailResult> OnRemoveContactEmail;
        public event Action<PlayFabError> OnPlayFabError;

        private readonly PlayFabProfileService _profileService = PlayFabProfileService.Instance;

        public string DisplayName { get => _displayName; set => _displayName = value; }
        public string Email { get => _email; set => _email = value; }

        public void RegisterEvents() {
            PlayFabProfileService.OnGetPlayerProfile += OnGetPlayerProfile.Invoke;
            PlayFabProfileService.OnUpdateDisplayName += OnUpdateDisplayName.Invoke;
            PlayFabProfileService.OnAddOrUpdateContactEmail += OnAddOrUpdateContactEmail.Invoke;
            PlayFabProfileService.OnRemoveContactEmail += OnRemoveContactEmail.Invoke;
            PlayFabProfileService.OnPlayFabError += OnPlayFabError.Invoke;
        }

        public void UnregisterEvents() {
            PlayFabProfileService.OnGetPlayerProfile -= OnGetPlayerProfile.Invoke;
            PlayFabProfileService.OnUpdateDisplayName -= OnUpdateDisplayName.Invoke;
            PlayFabProfileService.OnAddOrUpdateContactEmail -= OnAddOrUpdateContactEmail.Invoke;
            PlayFabProfileService.OnRemoveContactEmail -= OnRemoveContactEmail.Invoke;
            PlayFabProfileService.OnPlayFabError -= OnPlayFabError.Invoke;
        }

        [Button]
        public void UpdateDisplayName () {
            _profileService.UpdateDisplayName(_displayName);
        }

        [Button]
        public void GetPlayerProfile () {
            _profileService.GetPlayerProfile ();
        }

        [Button]
        public void AddOrUpdateContactEmail() {

            _profileService.Email = _email;
            _profileService.AddOrUpdateContactEmail ();
        }

        [Button]
        public void RemoveContactEmail () {

            _profileService.RemoveContactEmail ();
        }
    }
}
#endif