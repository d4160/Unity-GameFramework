using d4160.Events;
using Fusion;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/RuntimePlayers")]
    public class FusionRuntimePlayersSO : ScriptableObject, IEventListener<NetworkRunner, PlayerRef>
    {
        public FusionPlayerSO playerSO;
        public Vector3 spawnPoint;
        public Vector3 separatorBetween;
        [Header("EVENTS")]
        public NetworkRunnerPlayerEventSO onPlayerJoinedEventSO;
        public NetworkRunnerPlayerEventSO onPlayerLeftEventSO;
        public NetworkRunnerEventSO onInializedEventSO;
        public ShutdownEventSO onShutdownEventSO;

        private readonly Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new();

        private OnPlayerLeftEventListener _onPlayerLeftListener;
        private OnInitializedEventListener _onInitializedListener;
        private OnShutdownEventListener _onShutdownListener;

        public Transform SpawnPointT { get; set; }
        public Dictionary<PlayerRef, NetworkObject> SpawnedCharacters => _spawnedCharacters;

        private static readonly FusionService _service = FusionService.Instance;

        public void RegisterEvents()
        {
            onPlayerJoinedEventSO.AddListener(this);

            _onPlayerLeftListener.runtimePlayers = this;
            _onPlayerLeftListener.spawnedCharacters = _spawnedCharacters;
            onPlayerLeftEventSO.AddListener(_onPlayerLeftListener);

            _onInitializedListener.runtimePlayers = this;
            _onInitializedListener.spawnedCharacters = _spawnedCharacters;
            onInializedEventSO.AddListener(_onInitializedListener);

            _onShutdownListener.runtimePlayers = this;
            _onShutdownListener.spawnedCharacters = _spawnedCharacters;
            onShutdownEventSO.AddListener(_onShutdownListener);
        }

        public void UnregisterEvents()
        {
            onPlayerJoinedEventSO.RemoveListener(this);
            onPlayerLeftEventSO.RemoveListener(_onPlayerLeftListener);
            onInializedEventSO.RemoveListener(_onInitializedListener);
            onShutdownEventSO.RemoveListener(_onShutdownListener);
        }

        void IEventListener<NetworkRunner, PlayerRef>.OnInvoked(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer || (runner.GameMode == GameMode.Shared && player == runner.LocalPlayer))
            {
                Vector3 spawnPosition = SpawnPointT ? SpawnPointT.position : spawnPoint + separatorBetween * _spawnedCharacters.Count * (_spawnedCharacters.Count % 2 == 0 ? 1 : -1);
                NetworkObject networkPlayerObject = runner.Spawn(playerSO.playerPrefab, spawnPosition, SpawnPointT ? SpawnPointT.rotation : Quaternion.identity, player);
               
                runner.SetPlayerObject(player, networkPlayerObject);

                if (!_spawnedCharacters.ContainsKey(player))
                    _spawnedCharacters.Add(player, networkPlayerObject);

                int i = 0;
                foreach (var p in runner.ActivePlayers)
                {
                    LogInfo($"FusionRuntimePlayersSO->onPlayerJoinedEvent()->ActivePlayers; index: {i}; {p}; {runner.GetPlayerObject(p)?.name}");
                    i++;
                }
                
            }
        }

        [Button]
        public void LogActivePlayer()
        {
            int i = 0;
            foreach (var p in _service.Runner.ActivePlayers)
            {
                LogInfo($"FusionRuntimePlayersSO->onPlayerJoinedEvent()->ActivePlayers; index: {i}; {p}; {_service.Runner.GetPlayerObject(p)?.name}");
                i++;
            }
        }

        public void LogInfo(string message)
        {
            _service.LogInfo(message);
        }
    }

    public struct OnPlayerLeftEventListener : IEventListener<NetworkRunner, PlayerRef>
    {
        public FusionRuntimePlayersSO runtimePlayers;
        public Dictionary<PlayerRef, NetworkObject> spawnedCharacters;

        void IEventListener<NetworkRunner, PlayerRef>.OnInvoked(NetworkRunner runner, PlayerRef player)
        {
            if (spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runtimePlayers.LogInfo($"FusionRuntimePlayersSO->OnPlayerLeftEventListener Despawn(NetworkObject); {player};");

                runner.Despawn(networkObject);
                spawnedCharacters.Remove(player);
            }
        }
    }

    public struct OnShutdownEventListener : IEventListener<NetworkRunner, ShutdownReason>
    {
        public FusionRuntimePlayersSO runtimePlayers;
        public Dictionary<PlayerRef, NetworkObject> spawnedCharacters;

        void IEventListener<NetworkRunner, ShutdownReason>.OnInvoked(NetworkRunner runner, ShutdownReason reason)
        {
            if (spawnedCharacters.TryGetValue(runner.LocalPlayer, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);

                runtimePlayers.LogInfo($"FusionRuntimePlayersSO->OnShutdownEventListener Despawn(NetworkObject); {runner.LocalPlayer};");
            }

            runtimePlayers.LogActivePlayer();

            spawnedCharacters.Clear();
        }
    }

    public struct OnInitializedEventListener : IEventListener<NetworkRunner>
    {
        public FusionRuntimePlayersSO runtimePlayers;
        public Dictionary<PlayerRef, NetworkObject> spawnedCharacters;

        public void OnInvoked(NetworkRunner runner)
        {
            var players = runner.ActivePlayers;

            foreach (var player in players)
            {
                runtimePlayers.LogInfo($"OnInitializedEventListener; {player}; NetworkObject: {runner.GetPlayerObject(player)?.name}");
                if (!spawnedCharacters.ContainsKey(player))
                    spawnedCharacters.Add(player, runner.GetPlayerObject(player));
            }
        }
    }
}