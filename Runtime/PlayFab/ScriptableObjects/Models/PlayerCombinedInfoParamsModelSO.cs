using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using d4160.Variables;

namespace d4160.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/PlayFab/Models/PlayerCombinedInfoParams")]
    public class PlayerCombinedInfoParamsModelSO : ScriptableObject
    {
        public GetPlayerCombinedInfoRequestParams requestParams;
    }
}