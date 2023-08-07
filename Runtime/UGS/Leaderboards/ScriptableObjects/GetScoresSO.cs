using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Leaderboards.Exceptions;
using d4160.Variables;
using System;
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
            //Debug.Log($"GetScoresAsync(): LeaderboardId: {_leaderboardId.Value}");
            try {
                return await LeaderboardsService.Instance.GetScoresAsync(_leaderboardId.Value, new GetScoresOptions() { 
                    
                });
            }
            catch (LeaderboardsException ex)
            {
                //Debug.Log(ex.Reason);
                return default;
            }
        }
    }
}