using d4160.Logging;
using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace d4160.Fusion
{
    public class FusionService : INetworkRunnerCallbacks
    {
        private NetworkRunner _runner;

        public LoggerSO LoggerSO { get; set; }
        public NetworkRunner Runner
        {
            get => _runner;
            set
            {
                if (value != null)
                {
                    _runner = value;
                    _runner.ProvideInput = ProvideInput;
                    LoggerSO.LogInfo($"ProvideInput: {_runner.ProvideInput}");
                    _runner.AddCallbacks(this);
                }
                else
                {
                    _runner.RemoveCallbacks(this);
                    _runner = null;
                }
            }
        }
        public INetworkSceneManager SceneManager { get; set; }
        public bool ProvideInput { get; set; }

        public bool IsConnectedToServer => _runner != null && _runner.IsConnectedToServer;
        public bool IsRunning => _runner != null && _runner.IsRunning;
        public bool IsSharedModeMasterClient => _runner != null && _runner.IsSharedModeMasterClient;

        // SOEvents
        public NetworkRunnerEventSO OnConnectedToServerEventSO { get; set; }
        public ConnectionFailedEventSO OnConnectionFailedEventSO { get; set; }
        public ConnectRequestEventSO OnConnectRequestEventSO { get; set; }
        public CustomAuthenticationResponseEventSO OnCustomAuthenticationResponseEventSO { get; set; }
        public NetworkRunnerEventSO OnDisconnectedToServerEventSO { get; set; }
        public HostMigrationEventSO OnHostMigrationEventSO { get; set; }
        public InputEventSO OnInputEventSO { get; set; }
        public InputMissingEventSO OnInputMissingEventSO { get; set; }
        public NetworkRunnerPlayerEventSO OnPlayerJoinedEventSO { get; set; }
        public NetworkRunnerPlayerEventSO OnPlayerLeftEventSO { get; set; }
        public ReliableDataReceivedEventSO OnReliableDataReceivedEventSO { get; set; }
        public NetworkRunnerEventSO OnSceneLoadStartEventSO { get; set; }
        public NetworkRunnerEventSO OnSceneLoadDoneEventSO { get; set; }
        public SessionListUpdatedEventSO OnSessionListUpdatedEventSO { get; set; }
        public ShutdownEventSO OnShutdownEventSO { get; set; }
        public UserSimulationMessageEventSO OnUserSimulationMessageEventSO { get; set; }

        public static FusionService Instance => _instance ??= new FusionService();
        private static FusionService _instance;

        private FusionService()
        {
            _instance = this;
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnConnectedToServer; Tick={runner.Tick.Raw}; Mode={runner.Mode}; GameMode={runner.GameMode}; IsServer={runner.IsServer}; IsClient={runner.IsClient}; IsPlayer={runner.IsPlayer}; IsSharedModeMasterClient={runner.IsSharedModeMasterClient}; IsCloudReady=${runner.IsCloudReady}; IsConnectedToServer=${runner.IsConnectedToServer}; IsRunning={runner.IsRunning}; ActivePlayers={runner.ActivePlayers.Count()}; UserId={runner.UserId}");

            if (OnConnectedToServerEventSO) OnConnectedToServerEventSO.Invoke(runner);
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnConnectFailed; Tick={runner.Tick.Raw}; NetAddress={remoteAddress}; NetConnectFailedReason={reason}");

            if (OnConnectionFailedEventSO) OnConnectionFailedEventSO.Invoke(runner, remoteAddress, reason);
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnConnectRequest; Tick={runner.Tick.Raw}; ConnectRequest={request.RemoteAddress}; tokenLength={token.Length}");

            if (OnConnectRequestEventSO) OnConnectRequestEventSO.Invoke(runner, request, token);
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnCustomAuthenticationResponse; Tick={runner.Tick.Raw}; dataCount={data.Count}");

            if (OnCustomAuthenticationResponseEventSO) OnCustomAuthenticationResponseEventSO.Invoke(runner, data);
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnDisconnectedFromServer; Tick={runner.Tick.Raw}");

            if (OnDisconnectedToServerEventSO) OnDisconnectedToServerEventSO.Invoke(runner);
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnHostMigration; Tick={runner.Tick.Raw}; HostMigrationToken.GameMode={hostMigrationToken.GameMode}");

            if (OnHostMigrationEventSO) OnHostMigrationEventSO.Invoke(runner, hostMigrationToken);
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnInput;");

            if (OnInputEventSO) OnInputEventSO.Invoke(runner, input);
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnInputMissing; Tick= {runner.Tick.Raw} ; PlayerId={player.PlayerId}");

            if (OnInputMissingEventSO) OnInputMissingEventSO.Invoke(runner, player, input);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnPlayerJoined; Tick={runner.Tick.Raw}; Mode={runner.Mode}; GameMode={runner.GameMode}; IsServer={runner.IsServer}; IsClient={runner.IsClient}; IsPlayer={runner.IsPlayer}; IsSharedModeMasterClient={runner.IsSharedModeMasterClient}; Player={player}; ActorId={runner.GetPlayerActorId(player)}; UserId={runner.GetPlayerUserId(player)}; PlayerId={player.PlayerId}; RawEncoded={player.RawEncoded}");

            if (OnPlayerJoinedEventSO) OnPlayerJoinedEventSO.Invoke(runner, player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnPlayerLeft; Tick={runner.Tick.Raw}; PlayerId={player.PlayerId}");

            if (OnPlayerLeftEventSO) OnPlayerLeftEventSO.Invoke(runner, player);
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnReliableDataReceived; Tick={runner.Tick.Raw}; PlayerId={player.PlayerId}; dataCount={data.Count}");

            if (OnReliableDataReceivedEventSO) OnReliableDataReceivedEventSO.Invoke(runner, player, data);
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnSceneLoadDone; Tick={runner.Tick.Raw}");

            if (OnSceneLoadDoneEventSO) OnSceneLoadDoneEventSO.Invoke(runner);
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnSceneLoadStart; Tick={runner.Tick.Raw}");

            if (OnSceneLoadStartEventSO) OnSceneLoadStartEventSO.Invoke(runner);
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnSessionListUpdated; Tick={runner.Tick.Raw}; sessionListCount={sessionList.Count}");

            if (OnSessionListUpdatedEventSO) OnSessionListUpdatedEventSO.Invoke(runner, sessionList);
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnShutdown; Tick={runner.Tick.Raw}; ShutdownReason={shutdownReason}");

            if (OnShutdownEventSO) OnShutdownEventSO.Invoke(runner, shutdownReason);
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            if (LoggerSO) LoggerSO.LogInfo($"OnUserSimulationMessage; Tick={runner.Tick.Raw}");

            if (OnUserSimulationMessageEventSO) OnUserSimulationMessageEventSO.Invoke(runner, message);
        }
    }
}