using Fusion;
using UnityEngine;
using d4160.Events;
using NaughtyAttributes;

namespace d4160.Fusion
{
    public class FusionRoomPropertiesSB : SimulationBehaviour
    {
        [SerializeField] private IntEventSO _onSharedModeMasterClientChanged;

        [Networked] public string[] Properties { get; set; }
        [Networked, ShowNativeProperty] public int SharedModeMasterClient { get; set; }

        public bool IsLocalMasterClient { get; private set; } = false;
        public bool CheckMasterClient { get; set; } = false;

        public IntEventSO OnSharedModeMasterClientChanged => _onSharedModeMasterClientChanged;

        private static FusionRoomPropertiesSB _instance;
        public static FusionRoomPropertiesSB Instance 
        {
            get {
                if (!_instance) 
                {
                    var instances = FindObjectsOfType<FusionRoomPropertiesSB>();
                    if (instances.Length > 1)
                    {
                        for (int i = 1; i < instances.Length; i++)
                        {
                            Destroy(instances[i]);
                        }
                    }
                    _instance = instances[0];
                }

                return _instance;
            }
            private set { _instance = value; }
        }

        private static readonly FusionService _service = FusionService.Instance;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else if (Instance && Instance != this) 
            { 
                Destroy(Instance);
            }
        }

        public static void Rpc_UpdateRoomProperties(string[] properties)
        {
            Rpc_UpdateRoomProperties(_service.Runner, properties);
        }

        [Rpc]
        private static void Rpc_UpdateRoomProperties(NetworkRunner runner, string[] properties, RpcInfo info = default)
        {
            Instance.Properties = properties;

            _service.LogInfo($"Rpc_UpdateRoomProperties(NetworkRunner, string[], RpcInfo); ActivePlayers: {runner.ActivePlayers}; Properties: {properties.Length}; Info: Source= {info.Source}, IsInvokeLocal= {info.IsInvokeLocal}, Channel= {info.Channel}");
        }

        [Rpc]
        public static void Rpc_UpdateSharedModeMasterClient(NetworkRunner runner, PlayerRef newMasterClient, RpcInfo info = default)
        {
            Instance.CheckMasterClient = false;

            if (runner.LocalPlayer == newMasterClient)
            {
                Instance.IsLocalMasterClient = true;
            }
            
            if (Instance.SharedModeMasterClient != newMasterClient)
            {
                Instance.SharedModeMasterClient = newMasterClient;
                _service.LogInfo($"Rpc_UpdateSharedModeMasterClient->newMasterClient; NewMasterClient: {newMasterClient}");
                if (Instance.OnSharedModeMasterClientChanged) Instance.OnSharedModeMasterClientChanged.Invoke(newMasterClient);
            }

            _service.LogInfo($"Rpc_UpdateSharedModeMasterClient(NetworkRunner, string[], RpcInfo); ActivePlayers: {runner.ActivePlayers}; MasterClient: {newMasterClient}; Info: Source= {info.Source}, IsInvokeLocal= {info.IsInvokeLocal}, Channel= {info.Channel}");
        }

        [Rpc]
        public static void Rpc_GetPropertiesRequest(NetworkRunner runner, PlayerRef sender, RpcInfo info = default)
        {
            if (!info.IsInvokeLocal)
            {
                if (runner.LocalPlayer == Instance.SharedModeMasterClient)
                {
                    Rpc_GetPropertiesResponse(runner, sender, runner.LocalPlayer, Instance.Properties);
                }
            }

            _service.LogInfo($"Rpc_GetPropertiesRequest(NetworkRunner, PlayerRef, RpcInfo); ActivePlayers: {runner.ActivePlayers}; Sender: {sender}; IsLocalMasterClient: {Instance.IsLocalMasterClient}; Info: Source= {info.Source}, IsInvokeLocal= {info.IsInvokeLocal}, Channel= {info.Channel}");
        }

        [Rpc]
        public static void Rpc_GetPropertiesResponse(NetworkRunner runner, [RpcTarget] PlayerRef target, PlayerRef masterClient, string[] roomProperties, RpcInfo info = default)
        {
            Instance.SharedModeMasterClient = masterClient;
            Instance.Properties = roomProperties;

            _service.LogInfo($"Rpc_GetPropertiesResponse(NetworkRunner, [RpcTarget], PlayerRef, RpcInfo); ResponseMasterClient: {masterClient}; Info: Source= {info.Source}, IsInvokeLocal= {info.IsInvokeLocal}, Channel= {info.Channel}");
        }

        [Button, ContextMenu("ShowPropertiesInConsole")]
        public void ShowSharedModeMasterClientInConsole() {
            Debug.Log($"MasterClient: {SharedModeMasterClient}");
            Debug.Log($"Properties: {Properties.Length}; {Properties[0]}");
            Debug.Log($"From RpcSendTestMono: {RpcSendTestMono.Properties.Length}; {RpcSendTestMono.Properties[0]}");
        }
    }
}