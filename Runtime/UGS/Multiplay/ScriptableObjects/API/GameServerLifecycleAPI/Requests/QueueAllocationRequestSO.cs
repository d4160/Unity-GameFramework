using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.UGS.Multiplay.LifecycleAPI
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/API/Lifecycle/QueueAllocationRequest")]
    public class QueueAllocationRequestSO : ScriptableObject
    {
        [SerializeField] private bool _newUuidForAllocation;
        [SerializeField, HideIf("_newUuidForAllocation")] private string _allocationId;
        [SerializeField] private int _buildConfigurationId;
        [SerializeField] private string _payload;
        [SerializeField] private string _regionId;
        [SerializeField] private bool _restart;

        [Space]

        [SerializeField] private MultiplayLifecycleAllocationsAPI _lifecycleAllocationAPI;

        public int BuildConfigurationId
        {
            get => _buildConfigurationId;
            set => _buildConfigurationId = value;
        }

        public bool NewUuidForAllocation
        {
            get => _newUuidForAllocation;
            set => _newUuidForAllocation = value;
        }

        public QueueAllocationRequest GetRequest()
        {
            return new QueueAllocationRequest()
            {
                allocationId = _newUuidForAllocation ? Guid.NewGuid().ToString() : _allocationId,
                buildConfigurationId = _buildConfigurationId,
                payload = _payload,
                regionId = _regionId,
                restart = _restart
            };
        }

        [Button]
        public void SendRequest()
        {
            SendRequest();
        }

        public void SendRequest(Action<string> onResult, Action<string> onError = null)
        {
            QueueAllocationRequest req = GetRequest();
            _lifecycleAllocationAPI.QueueAllocation(req, onResult, onError);
        }
    }

    [Serializable]
    public class QueueAllocationRequest
    {
        public string allocationId;
        public int buildConfigurationId;
        public string payload;
        public string regionId;
        public bool restart;
    }
}
