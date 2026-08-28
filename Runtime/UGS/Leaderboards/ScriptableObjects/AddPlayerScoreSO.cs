using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using System;
using System.Threading.Tasks;
using Unity.Services.Leaderboards.Models;
using Unity.Services.Leaderboards.Exceptions;
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
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                Debug.LogWarning("[AddPlayerScoreSO] UnityServices is not initialized. Skipping AddPlayerScore.");
                return null;
            }

            if (AuthenticationService.Instance == null || !AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogWarning("[AddPlayerScoreSO] AuthenticationService is not signed in. Skipping AddPlayerScore.");
                return null;
            }

            try
            {
                string id = _leaderboardId != null ? _leaderboardId.Value : string.Empty;
                if (string.IsNullOrEmpty(id))
                {
                    Debug.LogWarning("[AddPlayerScoreSO] LeaderboardId is null or empty. Skipping AddPlayerScore.");
                    return null;
                }

                return await LeaderboardsService.Instance.AddPlayerScoreAsync(id, score);
            }
            catch (LeaderboardsException ex)
            {
                Debug.LogWarning($"[AddPlayerScoreSO] LeaderboardsException: {ex.Reason} (Code: {ex.ErrorCode})");
                return null;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[AddPlayerScoreSO] Failed to add player score: {ex.Message}");
                return null;
            }
        }
    }
}