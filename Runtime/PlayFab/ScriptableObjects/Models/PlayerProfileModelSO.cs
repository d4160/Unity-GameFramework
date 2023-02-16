using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/PlayFab/Models/PlayerProfile")]
    public class PlayerProfileModelSO : ScriptableObject
    {
        public PlayerProfileModel profile;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public StringVariableSO displayNameVar;

        public void SetPlayerProfile(PlayerProfileModel profile)
        {
            this.profile = profile;
            displayNameVar.Value = profile.DisplayName;
        }
    }
}