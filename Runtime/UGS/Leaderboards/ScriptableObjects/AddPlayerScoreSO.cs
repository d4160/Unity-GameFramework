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
    [CreateAssetMenu(menuName = "d4160/UGS/Leaderboards/AddPlayerScore")]
    public class AddPlayerScoreSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringVariableSO _leaderboardId;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private DoubleVariableSO _score;

        public async Task<LeaderboardEntry> AddPlayerScoreAsync(double score)
        {
            return await AddPlayerScoreInternalAsync(score);
        }

        public async Task<LeaderboardEntry> AddPlayerScoreAsync()
        {
            return await AddPlayerScoreInternalAsync(_score);
        }

        private async Task<LeaderboardEntry> AddPlayerScoreInternalAsync(double score)
        {
            return await LeaderboardsService.Instance.AddPlayerScoreAsync(_leaderboardId, score);
        }
    }
}