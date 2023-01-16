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

        private readonly FusionService _service = FusionService.Instance;

        public bool IsConnectedToServer => _service.IsConnectedToServer;
        public bool IsRunning => _service.IsRunning;
        public bool IsSharedModeMasterClient => _service.IsSharedModeMasterClient;

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
                _service.ProvideInput = provideInput;
                _service.Runner = value;
            }
        }
        public INetworkSceneManager SceneManager
        {
            get => FusionService.Instance.SceneManager;
            set
            {
                _service.SceneManager = value;
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
            _service.OnConnectedToServerEventSO = onConnectedToServerEventSO;
            _service.OnConnectionFailedEventSO = onConnectionFailedEventSO;
            _service.OnConnectRequestEventSO = onConnectRequestEventSO;
            _service.OnCustomAuthenticationResponseEventSO = onCustomAuthenticationResponseEventSO;
            _service.OnDisconnectedToServerEventSO = onDisconnectedToServerEventSO;
            _service.OnHostMigrationEventSO = onHostMigrationEventSO;
            _service.OnInputEventSO = onInputEventSO;
            _service.OnInputMissingEventSO = onInputMissingEventSO;
            _service.OnPlayerJoinedEventSO = onPlayerJoinedEventSO;
            _service.OnPlayerLeftEventSO = onPlayerLeftEventSO;
            _service.OnReliableDataReceivedEventSO = onReliableDataReceivedEventSO;
            _service.OnSceneLoadStartEventSO = onSceneLoadStartEventSO;
            _service.OnSceneLoadDoneEventSO = onSceneLoadDoneEventSO;
            _service.OnSessionListUpdatedEventSO = onSessionListUpdatedEventSO;
            _service.OnShutdownEventSO = onShutdownEventSO;
            _service.OnUserSimulationMessageEventSO = onUserSimulationMessageEventSO;

            if (runtimeGamesSO) runtimeGamesSO.RegisterEvents();
            if (runtimePlayersSO) runtimePlayersSO.RegisterEvents();
        }

        public void UnregisterEvents()
        {
            _service.OnConnectedToServerEventSO = null;
            _service.OnConnectionFailedEventSO = null;
            _service.OnConnectRequestEventSO = null;
            _service.OnCustomAuthenticationResponseEventSO = null;
            _service.OnDisconnectedToServerEventSO = null;
            _service.OnHostMigrationEventSO = null;
            _service.OnInputEventSO = null;
            _service.OnInputMissingEventSO = null;
            _service.OnPlayerJoinedEventSO = null;
            _service.OnPlayerLeftEventSO = null;
            _service.OnReliableDataReceivedEventSO = null;
            _service.OnSceneLoadStartEventSO = null;
            _service.OnSceneLoadDoneEventSO = null;
            _service.OnSessionListUpdatedEventSO = null;
            _service.OnShutdownEventSO = null;
            _service.OnUserSimulationMessageEventSO = null;

            if (runtimeGamesSO) runtimeGamesSO.UnregisterEvents();
            if (runtimePlayersSO) runtimePlayersSO.UnregisterEvents();
        }
    }
}
