using NaughtyAttributes;
using Unity.Services.Lobbies;
using UnityEngine;
using d4160.Variables;

namespace d4160.UGS.Lobbies
{
    [CreateAssetMenu(menuName = "d4160/UGS/Lobbies/JoinLobby")]
    public class JoinLobbySO : ScriptableObject
    {
        public LobbySO lobby;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public LobbyPlayerSO player;
        public StringReference lobbyId;
        public StringReference lobbyCode;
        public QueryFilterData[] quickJoinfilters;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void JoinLobbyByCodeAsync()
        {
            JoinLobbyByCodeAsync(lobbyCode);
        }

        public async void JoinLobbyByCodeAsync(string code)
        {
            JoinLobbyByCodeOptions options = new()
            {
                // TODO: Custom Id with PlayFab, for example
                Player = player.GetPlayer(),
            };

            try
            {
                lobby.Lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, options);

                // TODO: Implement LoggerSO
                Debug.Log($"[JoinLobbyByCode] Name: {lobby.Lobby.Name}; MaxPlayers: {lobby.Lobby.MaxPlayers}; Id: {lobby.Lobby.Id}: Code: {lobby.Lobby.LobbyCode}");

                lobby.PrintPlayers(lobby.Lobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void JoinLobbyByIdAsync()
        {
            JoinLobbyByIdAsync(lobbyId);
        }

        public async void JoinLobbyByIdAsync(string id)
        {
            JoinLobbyByIdOptions options = new()
            {
                // TODO: Custom Id with PlayFab, for example
                Player = player.GetPlayer(),
            };

            try
            {
                lobby.Lobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, options);

                // TODO: Implement LoggerSO
                Debug.Log($"[JoinLobbyById] Name: {lobby.Lobby.Name}; MaxPlayers: {lobby.Lobby.MaxPlayers}; Id: {lobby.Lobby.Id}: Code: {lobby.Lobby.LobbyCode}");

                lobby.PrintPlayers(lobby.Lobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public async void QuickJoinLobbyAsync()
        {
            QuickJoinLobbyOptions options = new()
            {
                // TODO: Custom Id with PlayFab, for example
                Player = player.GetPlayer(),
                Filter = QueryFilterData.GetFilters(quickJoinfilters)
            };

            try
            {
                lobby.Lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);

                // TODO: Implement LoggerSO
                Debug.Log($"[QuickJoinLobbyAsync] Name: {lobby.Lobby.Name}; MaxPlayers: {lobby.Lobby.MaxPlayers}; Id: {lobby.Lobby.Id}: Code: {lobby.Lobby.LobbyCode}");

                lobby.PrintPlayers(lobby.Lobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}