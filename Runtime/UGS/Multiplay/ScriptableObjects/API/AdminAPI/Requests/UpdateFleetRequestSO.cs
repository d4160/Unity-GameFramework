using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.UGS.Multiplay.AdminAPI
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/API/Admin/UpdateFleetRequest")]
    public class UpdateFleetRequestSO : ScriptableObject
    {
        [SerializeField] private long _allocationTTL = 3600;
        [Tooltip("Required")]
        [SerializeField] private long[] _buildConfigurations;
        [SerializeField] private long _deleteTTL = 604800;
        [SerializeField] private long _disabledDeleteTTL = 10800;
        [Tooltip("Only for reservations-based fleets, otherwise not include in request")]
        [SerializeField] private bool _graceful;
        [Tooltip("Required")]
        [SerializeField] private string _name;
        //[Tooltip("Required but obsolete")]
        //[SerializeField] private string _osID;
        [SerializeField] private long _shutdownTTL = 600;
        [SerializeField] private UsageSetting[] _usageSettings;

        [Space]

        [SerializeField] private MultiplayAdminFleetsAPI _adminFleetsAPI;

        public UpdateFleetRequest GetRequest()
        {
            return new UpdateFleetRequest()
            {
                allocationTTL = _allocationTTL,
                buildConfigurations = _buildConfigurations,
                deleteTTL = _deleteTTL,
                disabledDeleteTTL = _disabledDeleteTTL,
                //graceful = _graceful,
                name = _name,
                //osID = _osID,
                shutdownTTL = _shutdownTTL,
                ///usageSettings = _usageSettings
            };
        }

        [Button]
        public void SendRequest()
        {
            _adminFleetsAPI.UpdateFleet(GetRequest());
        }
    }

    [Serializable]
    public class UpdateFleetRequest
    {
        public long allocationTTL = 3600;
        public long[] buildConfigurations;
        public long deleteTTL = 604800;
        public long disabledDeleteTTL = 10800;
        //public bool graceful;
        public string name;
        //public string osID;
        public long shutdownTTL = 600;
        //public UsageSetting[] usageSettings;
    }

    [Serializable]
    public class UsageSetting
    {
        [Tooltip("Enum: 'CLOUD' 'METAL'")]
        public string hardwareType;
        [Tooltip("Omit for METAL. In most cases 'GCP-N2' for CLOUD")]
        public string machineType;
        [Tooltip("Omit for CLOUD. Optional for CLOUD")]
        public int maxServersPerCore;
        [Tooltip("Required for CLOUD. Optional for METAL")]
        public int maxServersPerMachine;
        [Tooltip("MB. Omit for CLOUD. Required for METAL (Min 100MB)")]
        public int memory;
        [Tooltip("MHz. Omit for CLOUD. Required for METAL (Min 100MHz)")]
        public int speed;
    }
}