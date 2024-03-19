using System.Threading.Tasks;
using d4160.Events;
using d4160.Logging;
using NaughtyAttributes;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
#if DEDICATED_SERVER
using Unity.Services.Multiplay;
#endif
using UnityEngine;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/Multiplay")]
    public class MultiplaySO : ScriptableObject
    {
        [SerializeField] private ServerQueryOptionsSO _serverQueryOptions;

#if DEDICATED_SERVER
        [Header("Events")]
        [SerializeField] private MultiplayEventCallbacksSO _eventCallbacks;
#endif
        [SerializeField] private VoidEventSO _onServerStarted;

        [Header("Log")]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] LoggerSO _logger;

#if DEDICATED_SERVER
        public IServerQueryHandler ServerQueryHandler => _serverQueryHandler;
#endif
        public VoidEventSO OnServerStarted => _onServerStarted;
        public LoggerSO Logger => _logger;

#if DEDICATED_SERVER
        private IServerEvents _serverEvents;
#endif
        private bool _alreadyAutoAllocated;
#if DEDICATED_SERVER
        private IServerQueryHandler _serverQueryHandler;
        private MultiplayAllocationEventSO.EventListener _onAllocateLtn;
#endif

        public async void SubscribeEvents()
        {
            await SubscribeEventsAsync();
        }

        public async Task SubscribeEventsAsync()
        {
            LogInfo("Calling SubscribeToServerEventsAsync");

#if DEDICATED_SERVER
            _onAllocateLtn = new(MultiplayEventCallbacks_Allocate);

            _eventCallbacks.OnAllocate.AddListener(_onAllocateLtn);

            // Also requires server.json
            _serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(_eventCallbacks.GetEventCallbacks());

            LogInfo("SubscribeToServerEventsAsync was called");
            Debug.Log(_serverEvents.ToString());
#endif
        }

        public async void UnsubscribeEvents()
        {
#if DEDICATED_SERVER
            await UnsubscribeEventsAsync();
#endif
        }

        public async Task UnsubscribeEventsAsync()
        {
#if DEDICATED_SERVER
            _eventCallbacks.OnAllocate.RemoveListener(_onAllocateLtn);

            if (_serverEvents != null)
            {
                await _serverEvents.UnsubscribeAsync();
            }
#endif
        }

#if DEDICATED_SERVER
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
#endif

#if DEDICATED_SERVER
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
#endif

#if DEDICATED_SERVER
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
#endif


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