using d4160.MonoBehaviours;
using UnityEngine;
using d4160.Loops;
using NaughtyAttributes;
using Unity.Services.Authentication;
#if ENABLE_QUANTUM_CONSOLE
using QFSW.QC;
#endif

namespace d4160.UGS.Lobbies
{
    public class LobbyMono : MonoBehaviourUnityData<LobbySO, RuntimeLobbiesSO, UpdateLobbySO, UpdatePlayerSO>, IUpdateObject
    {
        [SerializeField, Range(0, 30)] private float _heartbeatTimerMax = 15f;
        [SerializeField, Range(1.1f, 30)] private float _lobbyUpdateTimerMax = 2.1f;
        [SerializeField, Range(1.1f, 30)] private float _listLobbiesTimerMax = 3.1f;

        [SerializeField] private CreateLobbySO _createLobby;
        [SerializeField] private JoinLobbySO _joinLobby;

        private float _heartbeatTimer, _lobbyUpdateTimer, _listLobbiesTimer;

        private void OnEnable()
        {
            UpdateManager.AddListener(this);
            _lobbyUpdateTimer = _lobbyUpdateTimerMax;
        }

        private void OnDisable()
        {
            UpdateManager.RemoveListener(this);
        }

#if ENABLE_QUANTUM_CONSOLE
        [Command]
#endif
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void CreateLobby()
        {
            _createLobby.CreateLobbyAsync();
        }

#if ENABLE_QUANTUM_CONSOLE
        [Command]
#endif
        public void JoinLobbyByCodeAsync(string code)
        {
            _joinLobby.JoinLobbyByCodeAsync(code);
        }

#if ENABLE_QUANTUM_CONSOLE
        [Command]
#endif
        public void JoinLobbyByIdAsync(string id)
        {
            _joinLobby.JoinLobbyByIdAsync(id);
        }

#if ENABLE_QUANTUM_CONSOLE
        [Command]
#endif
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void QuickJoinLobby()
        {
            _joinLobby.QuickJoinLobbyAsync();
        }

#if ENABLE_QUANTUM_CONSOLE
        [Command]
#endif
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public async void LeaveLobby()
        {
            await _data1.LeaveLobbyAsync();
        }

#if ENABLE_QUANTUM_CONSOLE
        [Command]
#endif
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public async void DeleteLobby()
        {
            await _data1.DeleteLobbyAsync();
        }

#if ENABLE_QUANTUM_CONSOLE
        [Command]
#endif
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void ListLobbies()
        {
            _data2.ListLobbiesAsync();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_data1.Lobby != null)
            {
                _heartbeatTimer -= Time.deltaTime;
                _lobbyUpdateTimer -= Time.deltaTime;

                if (_heartbeatTimer <= 0)
                {
                    _heartbeatTimer = _heartbeatTimerMax;

                    _data1.SendHeartbeatPingAsync();
                }

                if (_lobbyUpdateTimer <= 0)
                {
                    _lobbyUpdateTimer = _lobbyUpdateTimerMax;
                    _data1.GetLobbyAsync();
                }
            }

            if (_data2 && AuthenticationService.Instance.IsSignedIn)
            {
                _listLobbiesTimer -= Time.deltaTime;

                if (_listLobbiesTimer <= 0)
                {
                    _listLobbiesTimer = _listLobbiesTimerMax;
                    _data2.ListLobbiesAsync();
                }
            }
        }
    }
}