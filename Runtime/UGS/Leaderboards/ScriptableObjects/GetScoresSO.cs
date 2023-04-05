using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using d4160.Variables;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.LeaderBoards
{
    [CreateAssetMenu(menuName = "d4160/UGS/Leaderboards/GetScores")]
    public class GetScoresSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _leaderboardId;

        public async Task<LeaderboardScoresPage> GetScoresAsync()
        {
            return await LeaderboardsService.Instance.GetScoresAsync(_leaderboardId);
        }
    }
}