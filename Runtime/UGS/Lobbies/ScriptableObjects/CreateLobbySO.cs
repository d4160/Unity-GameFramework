using NaughtyAttributes;
using Unity.Services.Lobbies;
using UnityEngine;

namespace d4160.UGS.Lobbies
{
    [CreateAssetMenu(menuName = "d4160/UGS/Lobbies/CreateLobby")]
    public class CreateLobbySO : ScriptableObject
    {
        public LobbySO lobby;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public LobbyParamsSO lobbyParams;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public LobbyDataSO lobbyData;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public LobbyPlayerSO player;


#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public async void CreateLobbyAsync()
        {
            CreateLobbyOptions options = new()
            {
                IsPrivate = lobbyParams.isPrivate,
                Data = LobbyData.GetLobbyData(lobbyData.lobbyData),
                // TODO: Custom Id with PlayFab, for example
                Player = player.GetPlayer()
            };

            //Debug.Log($"[Lobby created] PlayerId: {options.Player.Id}");

            try
            {
                lobby.Lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyParams._name, lobbyParams.maxPlayers, options);

                // TODO: Implement LoggerSO
                Debug.Log($"[Lobby created] Name: {lobby.Lobby.Name}; MaxPlayers: {lobby.Lobby.MaxPlayers}; Id: {lobby.Lobby.Id}: Code: {lobby.Lobby.LobbyCode}");

                lobby.PrintPlayers(lobby.Lobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
}