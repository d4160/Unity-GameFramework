using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Leaderboards.Exceptions;
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
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.LogWarning("[GetScoresSO] UnityServices is not initialized. Cannot get leaderboard scores.");
                return default;
            }

            if (AuthenticationService.Instance == null || !AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogWarning("[GetScoresSO] AuthenticationService is not signed in. Cannot get leaderboard scores.");
                return default;
            }

            try
            {
                string id = _leaderboardId != null ? _leaderboardId.Value : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    Debug.LogWarning("[GetScoresSO] LeaderboardId is null or empty.");
                    return default;
                }

                return await LeaderboardsService.Instance.GetScoresAsync(id, new GetScoresOptions());
            }
            catch (LeaderboardsException ex)
            {
                Debug.LogWarning($"[GetScoresSO] LeaderboardsException: {ex.Reason} (Code: {ex.ErrorCode})");
                return default;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[GetScoresSO] Failed to get leaderboard scores: {ex.Message}");
                return default;
            }
        }
    }
}