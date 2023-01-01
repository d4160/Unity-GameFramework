using d4160.Logging;
using Fusion;
using UnityEngine;

[CreateAssetMenu(menuName = "d4160/PhotonFusion/PhotonFusionService")]
public class PhotonFusionServiceSO : ScriptableObject
{
    public bool provideInput = true;
    public PhotonFusionRuntimeGamesSO runtimeGamesSO;
    public PhotonFusionRuntimePlayersSO runtimePlayersSO;
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
        get => PhotonFusionService.Instance.Runner;
        set { 
            var service = PhotonFusionService.Instance;
            service.ProvideInput = provideInput;
            service.Runner = value;
        }
    }
    public INetworkSceneManager SceneManager 
    {
        get => PhotonFusionService.Instance.SceneManager;
        set
        {
            var service = PhotonFusionService.Instance;
            service.SceneManager = value;
        }
    }
    public LoggerSO LoggerSO 
    {
        get => PhotonFusionService.Instance.LoggerSO;
        set => PhotonFusionService.Instance.LoggerSO = value;
    }

    public void SetLogger()
    {
        LoggerSO = loggerSO;
    }

    public void RegisterEvents()
    {
        var service = PhotonFusionService.Instance;
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
        var service = PhotonFusionService.Instance;
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
