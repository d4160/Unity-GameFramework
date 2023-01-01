using d4160.Logging;
using Fusion;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/Service")]
    public class FusionServiceSO : ScriptableObject
    {
        public bool provideInput = true;
        public FusionRuntimeGamesSO runtimeGamesSO;
        public FusionRuntimePlayersSO runtimePlayersSO;
        public LoggerSO loggerSO;

        [Header("EVENTS")]
        public NetworkRunnerEventSO onConnectedToServerEventSO;
        public ConnectionFailedEventSO onConnectionFailedEventSO;
        public ConnectRequestEventSO onConnectRequestEventSO;
        public CustomAuthenticationResponseEventSO onCustomAuthenticationResponseEventSO;
        public NetworkRunnerEventSO onDisconnectedToServerEventSO;
        public HostMigrationEventSO onHostMigrationEventSO;
        public InputEventSO onInputEventSO;
        public InputMissingEventSO onInputMissingEventSO;
        public NetworkRunnerPlayerEventSO onPlayerJoinedEventSO;
        public NetworkRunnerPlayerEventSO onPlayerLeftEventSO;
        public ReliableDataReceivedEventSO onReliableDataReceivedEventSO;
        public NetworkRunnerEventSO onSceneLoadStartEventSO;
        public NetworkRunnerEventSO onSceneLoadDoneEventSO;
        public SessionListUpdatedEventSO onSessionListUpdatedEventSO;
        public ShutdownEventSO onShutdownEventSO;
        public UserSimulationMessageEventSO onUserSimulationMessageEventSO;

        public NetworkRunner Runner
        {
            get => FusionService.Instance.Runner;
            set
            {
                var service = FusionService.Instance;
                service.ProvideInput = provideInput;
                service.Runner = value;
            }
        }
        public INetworkSceneManager SceneManager
        {
            get => FusionService.Instance.SceneManager;
            set
            {
                var service = FusionService.Instance;
                service.SceneManager = value;
            }
        }
        public LoggerSO LoggerSO
        {
            get => FusionService.Instance.LoggerSO;
            set => FusionService.Instance.LoggerSO = value;
        }

        public void SetLogger()
        {
            LoggerSO = loggerSO;
        }

        public void RegisterEvents()
        {
            var service = FusionService.Instance;
            service.OnConnectedToServerEventSO = onConnectedToServerEventSO;
            service.OnConnectionFailedEventSO = onConnectionFailedEventSO;
            service.OnConnectRequestEventSO = onConnectRequestEventSO;
            service.OnCustomAuthenticationResponseEventSO = onCustomAuthenticationResponseEventSO;
            service.OnDisconnectedToServerEventSO = onDisconnectedToServerEventSO;
            service.OnHostMigrationEventSO = onHostMigrationEventSO;
            service.OnInputEventSO = onInputEventSO;
            service.OnInputMissingEventSO = onInputMissingEventSO;
            service.OnPlayerJoinedEventSO = onPlayerJoinedEventSO;
            service.OnPlayerLeftEventSO = onPlayerLeftEventSO;
            service.OnReliableDataReceivedEventSO = onReliableDataReceivedEventSO;
            service.OnSceneLoadStartEventSO = onSceneLoadStartEventSO;
            service.OnSceneLoadDoneEventSO = onSceneLoadDoneEventSO;
            service.OnSessionListUpdatedEventSO = onSessionListUpdatedEventSO;
            service.OnShutdownEventSO = onShutdownEventSO;
            service.OnUserSimulationMessageEventSO = onUserSimulationMessageEventSO;

            if (runtimeGamesSO) runtimeGamesSO.RegisterEvents();
            if (runtimePlayersSO) runtimePlayersSO.RegisterEvents();
        }

        public void UnregisterEvents()
        {
            var service = FusionService.Instance;
            service.OnConnectedToServerEventSO = null;
            service.OnConnectionFailedEventSO = null;
            service.OnConnectRequestEventSO = null;
            service.OnCustomAuthenticationResponseEventSO = null;
            service.OnDisconnectedToServerEventSO = null;
            service.OnHostMigrationEventSO = null;
            service.OnInputEventSO = null;
            service.OnInputMissingEventSO = null;
            service.OnPlayerJoinedEventSO = null;
            service.OnPlayerLeftEventSO = null;
            service.OnReliableDataReceivedEventSO = null;
            service.OnSceneLoadStartEventSO = null;
            service.OnSceneLoadDoneEventSO = null;
            service.OnSessionListUpdatedEventSO = null;
            service.OnShutdownEventSO = null;
            service.OnUserSimulationMessageEventSO = null;

            if (runtimeGamesSO) runtimeGamesSO.UnregisterEvents();
            if (runtimePlayersSO) runtimePlayersSO.UnregisterEvents();
        }
    }
}
