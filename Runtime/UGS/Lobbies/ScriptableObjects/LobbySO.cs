using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using static Unity.Services.Lobbies.Models.DataObject;
using d4160.Variables;
using Unity.Services.Authentication;
using d4160.Events;
using System.Threading.Tasks;
using d4160.Collections;

namespace d4160.UGS.Lobbies
{
    [CreateAssetMenu(menuName = "d4160/UGS/Lobbies/Lobby")]
    public class LobbySO : ScriptableObject
    {
        public StringReference playerNameKey;
        [SerializeField] private VoidEventSO _onGetLobby;

        public Lobby Lobby { get; internal set; }
        public bool IsHost => Lobby != null && Lobby.HostId == AuthenticationService.Instance.PlayerId;
        public int PlayersCount => Lobby != null ? (Lobby.MaxPlayers - Lobby.AvailableSlots) : 0;

        public string GetData(LobbyDataSO data, int index)
        {
            if (Lobby == null) return string.Empty;
            if (!data.lobbyData.IsValidIndex(index)) return string.Empty;
            if (!Lobby.Data.ContainsKey(data.lobbyData[index].key)) return string.Empty;

            return Lobby.Data[data.lobbyData[index].key].Value;
        }

        public string GetPlayerName(Player player)
        {
            if (player.Data != null && player.Data.ContainsKey(playerNameKey))
                return player.Data[playerNameKey].Value;
            return "Desconocido";
        }

        internal void PrintPlayers(Lobby lobby)
        {
            if (lobby == null || lobby.Players == null) return;

            Debug.Log($"Players in Lobby {lobby.Name} {lobby.Players.Count}");
            for (int i = 0; i < lobby.Players.Count; i++)
            {
                Player p = lobby.Players[i];
                string pName = (p.Data != null && p.Data.ContainsKey(playerNameKey))
                    ? p.Data[playerNameKey].Value : "N/A";
                Debug.Log($"{p.Id} {pName}");
            }
        }

        public async Task SendHeartbeatPingAsync()
        {
            if (Lobby != null && Lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(Lobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    if (e.Reason == LobbyExceptionReason.RateLimited)
                    {
                        Debug.LogWarning("[BugFix#71] SendHeartbeatPingAsync: Rate limited (429). Will retry next cycle.");
                    }
                    else if (e.Reason == LobbyExceptionReason.LobbyNotFound)
                    {
                        Debug.LogWarning("[BugFix#71] SendHeartbeatPingAsync: Lobby expired (404). Clearing reference.");
                        Lobby = null;
                    }
                    else
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        public async Task GetLobbyAsync()
        {
            if (Lobby != null)
            {
                try
                {
                    Lobby = await LobbyService.Instance.GetLobbyAsync(Lobby.Id);
                    if (_onGetLobby) _onGetLobby.Invoke();
                }
                catch (LobbyServiceException e)
                {
                    if (e.Reason == LobbyExceptionReason.LobbyNotFound)
                    {
                        Debug.LogWarning($"[BugFix#71] GetLobbyAsync: Lobby expired (404). Clearing reference.");
                        Lobby = null;
                    }
                    else
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }

        /// <summary>
        /// BugFix#68: Fetches a lobby by ID and assigns it to this SO.
        /// Used after CreateSessionAsync/CreateOrJoinSessionAsync (UMS) to populate
        /// the Lobby reference so LobbyMono polling, heartbeat, and
        /// ParticipantsInClassUI can function.
        /// This is a READ-only operation (no join/create), safe from 429 errors.
        /// </summary>
        public async Task FetchAndSetLobbyAsync(string lobbyId)
        {
            try
            {
                Lobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
                Debug.Log($"[BugFix#68] FetchAndSetLobbyAsync: Lobby '{Lobby.Name}' loaded, {PlayersCount} player(s)");
                if (_onGetLobby) _onGetLobby.Invoke();
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning($"[BugFix#68] FetchAndSetLobbyAsync failed for lobbyId={lobbyId}: {e.Message}");
            }
        }

        /// <summary>
        /// BugFix#73: Uploads local player data (name, role, etc.) to the lobby.
        /// Must be called AFTER FetchAndSetLobbyAsync so Lobby reference is valid.
        /// Fixes "Desconocido" in ParticipantsInClass — player data was never uploaded
        /// because DisplayNameVar.OnValueChange fired before lobby existed.
        /// </summary>
        public async Task UpdateLocalPlayerDataAsync(LobbyPlayerSO playerData)
        {
            if (Lobby == null || playerData == null) return;

            try
            {
                var options = new UpdatePlayerOptions
                {
                    Data = LobbyPlayerData.GetPlayerData(playerData.playerData)
                };

                Lobby = await LobbyService.Instance.UpdatePlayerAsync(
                    Lobby.Id,
                    AuthenticationService.Instance.PlayerId,
                    options);

                Debug.Log($"[BugFix#73] UpdateLocalPlayerDataAsync: Player data uploaded to lobby '{Lobby.Name}'");
                PrintPlayers(Lobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning($"[BugFix#73] UpdateLocalPlayerDataAsync failed: {e.Message}");
            }
        }

        public async Task LeaveLobbyAsync()
        {
            if (Lobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(Lobby.Id, AuthenticationService.Instance.PlayerId);
                    Lobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public async Task RemovePlayerAsync(string playerId)
        {
            if (Lobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(Lobby.Id, playerId);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public async Task MigrateHostAsync(string playerId)
        {
            if (Lobby != null)
            {
                try
                {
                    Lobby = await LobbyService.Instance.UpdateLobbyAsync(Lobby.Id,
                        new UpdateLobbyOptions()
                        {
                            HostId = playerId
                        });
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        // BugFix#74: Changed from async void to async Task so LeaveOrDeleteLobby can await it
        private async Task TryMigrateHost(string currentHostId)
        {
            Debug.Log($"[BugFix#74] TryMigrateHost: Starting migration from host '{currentHostId}'");
            await GetLobbyAsync();

            string newHostId = string.Empty;

            if (Lobby == null || Lobby.Players == null)
            {
                Debug.LogWarning("[BugFix#74] TryMigrateHost: Lobby is null after refresh — cannot migrate.");
                return;
            }

            Debug.Log($"[BugFix#74] TryMigrateHost: Lobby has {Lobby.Players.Count} players");
            for (int i = 0; i < Lobby.Players.Count; i++)
            {
                if (Lobby.Players[i].Id == currentHostId)
                {
                    continue;
                }

                newHostId = Lobby.Players[i].Id;
            }

            if (!string.IsNullOrEmpty(newHostId))
            {
                Debug.Log($"[BugFix#74] TryMigrateHost: Migrating host to '{newHostId}'");
                await MigrateHostAsync(newHostId);
                Debug.Log($"[BugFix#74] TryMigrateHost: Host migrated. Now leaving lobby...");
                await LeaveLobbyAsync();
                Debug.Log("[BugFix#74] TryMigrateHost: Left lobby successfully.");
            }
            else
            {
                Debug.LogWarning("[BugFix#74] TryMigrateHost: No other player found — deleting lobby.");
                await DeleteLobbyAsync();
            }
        }

        public async Task DeleteLobbyAsync()
        {
            if (Lobby != null)
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(Lobby.Id);
                    Lobby = null;
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public async Task LeaveOrDeleteLobby(bool migrateHost = false)
        {
            try
            {
                if (Lobby != null)
                {
                    bool isHost = false;
                    try
                    {
                        if (AuthenticationService.Instance.IsSignedIn)
                        {
                            isHost = IsHost;
                        }
                    }
                    catch {}

                    if (isHost)
                    {
                        if (migrateHost)
                        {
                            // BugFix#74: Await migration so it completes before Lobby = null
                            await TryMigrateHost(Lobby.HostId);
                        }
                        else
                        {
                            await DeleteLobbyAsync();
                        }
                    }
                    else
                    {
                        await LeaveLobbyAsync();
                    }

                    Lobby = null;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[BugFix#74] LeaveOrDeleteLobby failed: {ex.Message}");
                Lobby = null;
            }
        }

        /// <summary>
        /// BugFix#74v3 / Sprint07: Leaves all active UMS sessions gracefully.
        /// In DA mode (non-server), LeaveAsync() does NOT delete the session — it just
        /// removes the player and disconnects transport cleanly.
        /// Iterates over a snapshot list of sessions to avoid 'Collection was modified' exceptions.
        /// </summary>
        public static async Task TryLeaveUmsSessionsAsync()
        {
            try
            {
                if (Unity.Services.Multiplayer.MultiplayerService.Instance == null ||
                    Unity.Services.Multiplayer.MultiplayerService.Instance.Sessions == null)
                    return;

                var sessions = Unity.Services.Multiplayer.MultiplayerService.Instance.Sessions;
                var sessionList = new List<KeyValuePair<string, Unity.Services.Multiplayer.ISession>>(sessions);

                foreach (var kvp in sessionList)
                {
                    if (kvp.Value != null && kvp.Value.State == Unity.Services.Multiplayer.SessionState.Connected)
                    {
                        Debug.Log($"[BugFix#74] Leaving UMS session '{kvp.Key}'...");
                        try
                        {
                            await kvp.Value.LeaveAsync();
                            Debug.Log($"[BugFix#74] UMS session '{kvp.Key}' left successfully.");
                        }
                        catch (System.Exception leaveEx)
                        {
                            Debug.LogWarning($"[BugFix#74] LeaveAsync for session '{kvp.Key}' error: {leaveEx.Message}");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"[BugFix#74] TryLeaveUmsSessionsAsync failed: {ex.Message}");
            }
        }
    }

    [System.Serializable]
    public struct LobbyData
    {
        public StringReference key;
        public VisibilityOptions visibility;
        public VariableReferenceBase<VariableSOBase, string> value;
        public IndexOptions index;

        public static Dictionary<string, DataObject> GetLobbyData(LobbyData[] lobbyData)
        {
            Dictionary<string, DataObject> lData = null;
            if (lobbyData != null && lobbyData.Length > 0)
            {
                lData = new(lobbyData.Length);
                for (int i = 0; i < lobbyData.Length; i++)
                {
                    LobbyData l = lobbyData[i];
                    lData.Add(l.key, new(l.visibility, l.value.StringValue, l.index));
                }
            }

            return lData;
        }
    }

    [System.Serializable]
    public struct LobbyPlayerData
    {
        public StringReference key;
        public PlayerDataObject.VisibilityOptions visibility;
        public VariableReferenceBase<VariableSOBase, string> value;

        public static Dictionary<string, PlayerDataObject> GetPlayerData(LobbyPlayerData[] playerData)
        {
            Dictionary<string, PlayerDataObject> pData = null;
            if (playerData != null && playerData.Length > 0)
            {
                pData = new(playerData.Length);
                for (int i = 0; i < playerData.Length; i++)
                {
                    LobbyPlayerData p = playerData[i];
                    pData.Add(p.key, new(p.visibility, p.value.StringValue));
                }
            }

            return pData;
        }


    }
}
