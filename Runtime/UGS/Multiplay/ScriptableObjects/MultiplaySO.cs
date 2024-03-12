using System.Threading.Tasks;
using d4160.Events;
using d4160.Logging;
using NaughtyAttributes;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Multiplay;
using UnityEngine;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/Multiplay")]
    public class MultiplaySO : ScriptableObject
    {
        [SerializeField] private ServerQueryOptionsSO _serverQueryOptions;

        [Header("Events")]
        [SerializeField] private MultiplayEventCallbacksSO _eventCallbacks;
        [SerializeField] private VoidEventSO _onServerStarted;

        [Header("Log")]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] LoggerSO _logger;

        public IServerQueryHandler ServerQueryHandler => _serverQueryHandler;
        public VoidEventSO OnServerStarted => _onServerStarted;
        public LoggerSO Logger => _logger;

        private IServerEvents _serverEvents;
        private bool _alreadyAutoAllocated;
        private IServerQueryHandler _serverQueryHandler;
        private MultiplayAllocationEventSO.EventListener _onAllocateLtn;

        public async void SubscribeEvents()
        {
            await SubscribeEventsAsync();
        }

        public async Task SubscribeEventsAsync()
        {
            LogInfo("Calling SubscribeToServerEventsAsync");

            _onAllocateLtn = new(MultiplayEventCallbacks_Allocate);

            _eventCallbacks.OnAllocate.AddListener(_onAllocateLtn);

            // Also requires server.json
            _serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(_eventCallbacks.GetEventCallbacks());

            LogInfo("SubscribeToServerEventsAsync was called");
            Debug.Log(_serverEvents.ToString());
        }

        public async void UnsubscribeEvents()
        {
            await UnsubscribeEventsAsync();
        }

        public async Task UnsubscribeEventsAsync()
        {
            _eventCallbacks.OnAllocate.RemoveListener(_onAllocateLtn);

            if (_serverEvents != null)
            {
                await _serverEvents.UnsubscribeAsync();
            }
        }

        public async Task StartServerAsync()
        {
            _serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(_serverQueryOptions.MaxPlayers, _serverQueryOptions.ServerName, _serverQueryOptions.GameType, _serverQueryOptions.BuildId, _serverQueryOptions.Map);

            ServerConfig serverConfig = MultiplayService.Instance.ServerConfig;

            if (!string.IsNullOrEmpty(serverConfig.AllocationId))
            {
                MultiplayEventCallbacks_Allocate(new("", serverConfig.ServerId, serverConfig.AllocationId));
            }
            else
            {
                LogWarning("ServerConfig.AllocationId is empty. Waiting for Allocate event...");
            }
        }

        public void LogServerConfig()
        {
            ServerConfig serverConfig = MultiplayService.Instance.ServerConfig;
            LogInfo("ServerConfig Properties:");
            Debug.Log($"ServerId: {serverConfig.ServerId}");
            Debug.Log($"AllocationId: {serverConfig.AllocationId}");
            Debug.Log($"IpAddress: {serverConfig.IpAddress}");
            Debug.Log($"Port: {serverConfig.Port}");
            Debug.Log($"QueryPort: {serverConfig.QueryPort}");
            Debug.Log($"LogDirectory: {serverConfig.ServerLogDirectory}");
        }

        private void MultiplayEventCallbacks_Allocate(MultiplayAllocation allocation)
        {
            if (_alreadyAutoAllocated)
            {
                LogInfo("Already Auto Allocated");
                return;
            }

            _alreadyAutoAllocated = true;

            LogServerConfig();

            ServerConfig serverConfig = MultiplayService.Instance.ServerConfig;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(serverConfig.IpAddress, serverConfig.Port, "0.0.0.0");

            NetworkManager.Singleton.StartServer();

            if (_onServerStarted) _onServerStarted.Invoke();

            LogInfo($"[Allocated] AllocationId: {allocation.AllocationId}, ServerId:{allocation.ServerId}, EventId:{allocation.EventId}");
        }

        public void LogInfo(string message, GameObject context = null)
        {
            if (_logger) _logger.LogInfo(message, context);
        }

        public void LogWarning(string message, GameObject context = null)
        {
            if (_logger) _logger.LogWarning(message, context);
        }

        public void LogError(string message, GameObject context = null)
        {
            if (_logger) _logger.LogError(message, context);
        }
    }
}