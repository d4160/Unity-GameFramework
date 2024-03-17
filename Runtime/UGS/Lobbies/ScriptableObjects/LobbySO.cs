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
            return player.Data[playerNameKey].Value;
        }

        internal void PrintPlayers(Lobby lobby)
        {
            Debug.Log($"Players in Lobby {lobby.Name} {lobby.Players.Count}");
            for (int i = 0; i < lobby.Players.Count; i++)
            {
                Player p = lobby.Players[i];
                Debug.Log($"{p.Id} {p.Data[playerNameKey].Value}");
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
                    Debug.LogException(e);
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
                    Debug.LogException(e);
                }
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

        private async void TryMigrateHost(string currentHostId)
        {
            await GetLobbyAsync();

            string newHostId = string.Empty;

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
                await MigrateHostAsync(newHostId);
            }
            else
            {
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

        public async void LeaveOrDeleteLobby(bool migrateHost = false)
        {
            if (Lobby != null)
            {
                if (IsHost)
                {
                    if (migrateHost)
                    {
                        TryMigrateHost(Lobby.HostId);
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
