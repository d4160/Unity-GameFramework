#if PLAYFAB
using System;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace d4160.Profile.PlayFab {

    [CreateAssetMenu (menuName = "d4160/Profile/PlayFab")]
    public class PlayFabProfileSO : ScriptableObject {

        [SerializeField] private string _email;
        [SerializeField] private PlayerProfileModel _playerProfile;
        [SerializeField] private PlayerProfileViewConstraints _profileConstraints;

        public event Action<GetPlayerProfileResult> OnGetPlayerProfile;
        public event Action<UpdateUserTitleDisplayNameResult> OnUpdateDisplayName;
        public event Action<AddOrUpdateContactEmailResult> OnAddOrUpdateContactEmail;
        public event Action<RemoveContactEmailResult> OnRemoveContactEmail;
        public event Action<PlayFabError> OnPlayFabError;

        private readonly PlayFabProfileService _profileService = PlayFabProfileService.Instance;

        public string DisplayName { get => _playerProfile.DisplayName; set => _playerProfile.DisplayName = value; }
        public string Email { get => _email; set => _email = value; }

        private void CallOnGetPlayerProfile(GetPlayerProfileResult result) => OnGetPlayerProfile?.Invoke(result);
        private void CallOnUpdateDisplayName(UpdateUserTitleDisplayNameResult result) => OnUpdateDisplayName?.Invoke(result);
        private void CallOnAddOrUpdateContactEmail(AddOrUpdateContactEmailResult result) => OnAddOrUpdateContactEmail?.Invoke(result);
        private void CallOnRemoveContactEmail(RemoveContactEmailResult result) => OnRemoveContactEmail?.Invoke(result);
        private void CallOnPlayFabError(PlayFabError error) => OnPlayFabError?.Invoke(error);

        public void RegisterEvents () {
            PlayFabProfileService.OnGetPlayerProfile += CallOnGetPlayerProfile;
            PlayFabProfileService.OnUpdateDisplayName += CallOnUpdateDisplayName;
            PlayFabProfileService.OnAddOrUpdateContactEmail += CallOnAddOrUpdateContactEmail;
            PlayFabProfileService.OnRemoveContactEmail += CallOnRemoveContactEmail;
            PlayFabProfileService.OnPlayFabError += CallOnPlayFabError;
        }

        public void UnregisterEvents () {
            PlayFabProfileService.OnGetPlayerProfile -= CallOnGetPlayerProfile;
            PlayFabProfileService.OnUpdateDisplayName -= CallOnUpdateDisplayName;
            PlayFabProfileService.OnAddOrUpdateContactEmail -= CallOnAddOrUpdateContactEmail;
            PlayFabProfileService.OnRemoveContactEmail -= CallOnRemoveContactEmail;
            PlayFabProfileService.OnPlayFabError -= CallOnPlayFabError;
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void UpdateDisplayName () {
            _profileService.UpdateDisplayName (_playerProfile.DisplayName);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void GetPlayerProfile () {
            _profileService.ProfileConstraints = _profileConstraints;
            _profileService.GetPlayerProfile ((result) => {
                _playerProfile = result.PlayerProfile;
            });
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void UpdateAvatarUrl () {

            _profileService.UpdateAvatarUrl (_playerProfile.AvatarUrl);
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void AddOrUpdateContactEmail () {

            _profileService.Email = _email;
            _profileService.AddOrUpdateContactEmail ();
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void RemoveContactEmail () {

            _profileService.RemoveContactEmail ();
        }
    }
}
#endif