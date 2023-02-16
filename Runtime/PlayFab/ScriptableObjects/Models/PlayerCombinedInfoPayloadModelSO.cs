using UnityEngine;
using PlayFab.ClientModels;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/PlayFab/Models/PlayerCombinedInfoPayload")]
    public class PlayerCombinedInfoPayloadModelSO : ScriptableObject
    {
        public GetPlayerCombinedInfoResultPayload payload;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public PlayerProfileModelSO profileSO;

        public void SetResultPayload(LoginResult result)
        {
            payload = result.InfoResultPayload;
            profileSO.SetPlayerProfile(payload.PlayerProfile);
        }
    }
}
