using d4160.Variables;
using NaughtyAttributes;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Collections.Generic;

namespace d4160.UGS.Lobbies
{
    [CreateAssetMenu(menuName = "d4160/UGS/Lobbies/UpdateLobby")]
    public class UpdateLobbySO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public LobbySO lobby;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public StringVariableSO _name;
        [Range(1, 100)]
        public int maxPlayers = 10;
        public bool isPrivate = false;
        public bool isLocked = false;
        [ContextMenuItem("ClearIndex", "ClearIndex")]
        public LobbyData[] lobbyData;

#if UNITY_EDITOR
        private void ClearIndex()
        {
            for (int i = 0; i < lobbyData.Length; i++)
            {
                lobbyData[i].index = default;
            }
        }
#endif
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public async void UpdateLobbyAsync()
        {
            UpdateLobbyOptions options = new()
            {
                Name = _name,
                MaxPlayers = maxPlayers,
                IsPrivate = isPrivate,
                IsLocked = isLocked,
                Data = LobbyData.GetLobbyData(lobbyData)
            };

            try
            {
                lobby.Lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Lobby.Id, options);

                // TODO: Implement LoggerSO
                Debug.Log($"[UpdateLobbyAsync] Name: {lobby.Lobby.Name}; MaxPlayers: {lobby.Lobby.MaxPlayers}; Id: {lobby.Lobby.Id}: Code: {lobby.Lobby.LobbyCode}");

                lobby.PrintPlayers(lobby.Lobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}