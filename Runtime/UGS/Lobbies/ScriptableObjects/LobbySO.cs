using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using static Unity.Services.Lobbies.Models.DataObject;
using d4160.Variables;
using NaughtyAttributes;
using Unity.Services.Authentication;

namespace d4160.UGS.Lobbies
{
    [CreateAssetMenu(menuName = "d4160/UGS/Lobbies/Lobby")]
    public class LobbySO : ScriptableObject
    {
        public StringReference playerNameKey;

        public Lobby Lobby { get; internal set; }

        internal void PrintPlayers(Lobby lobby)
        {
            Debug.Log($"Players in Lobby {lobby.Name} {lobby.Players.Count}");
            for (int i = 0; i < lobby.Players.Count; i++)
            {
                Player p = lobby.Players[i];
                Debug.Log($"{p.Id} {p.Data[playerNameKey]}");
            }
        }

        public async void SendHeartbeatPingAsync()
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

        public async void GetLobbyAsync()
        {
            if (Lobby != null)
            {
                try
                {
                    Lobby = await LobbyService.Instance.GetLobbyAsync(Lobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public async void LeaveLobbyAsync()
        {
            if (Lobby != null)
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(Lobby.Id, AuthenticationService.Instance.PlayerId);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public async void RemovePlayerAsync(string playerId)
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

        public async void MigrateHostAsync(string playerId)
        {
            if (Lobby != null)
            {
                try
                {
                    Lobby = await LobbyService.Instance.UpdateLobbyAsync(Lobby.Id, 
                        new UpdateLobbyOptions() { 
                            HostId = playerId
                        });
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public async void DeleteLobbyAsync()
        {
            if (Lobby != null)
            {
                try
                {
                    await LobbyService.Instance.DeleteLobbyAsync(Lobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }

    [System.Serializable]
    public struct LobbyData
    {
        public StringReference key;
        public VisibilityOptions visibility;
        public StringReference value;
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
                    lData.Add(l.key, new(l.visibility, l.value, l.index));
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
        public StringReference value;

        public static Dictionary<string, PlayerDataObject> GetPlayerData(LobbyPlayerData[] playerData)
        {
            Dictionary<string, PlayerDataObject> pData = null;
            if (playerData != null && playerData.Length > 0)
            {
                pData = new(playerData.Length);
                for (int i = 0; i < playerData.Length; i++)
                {
                    LobbyPlayerData p = playerData[i];
                    pData.Add(p.key, new(p.visibility, p.value));
                }
            }

            return pData;
        }
    }
}
