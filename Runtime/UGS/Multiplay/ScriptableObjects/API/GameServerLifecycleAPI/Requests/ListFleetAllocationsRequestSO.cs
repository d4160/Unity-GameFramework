using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.UGS.Multiplay.LifecycleAPI
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/API/Lifecycle/ListFleetAllocationsRequest")]
    public class ListFleetAllocationsRequestSO : ScriptableObject
    {
        [Tooltip("Example: 2h")]
        [SerializeField] private string _age;
        [Tooltip("This must be a value between 1 and 50. Anything above will be hard limited to 50 results.")]
        [SerializeField] private int _limit = 10;
        [Tooltip("Optional parameter used for pagination. Represents the offset at which to start pagination. Example '10' for page 2")]
        [SerializeField] private int _offset = 0;
        [Tooltip("Example: ids=1111a1a1-a11a-11a1-a1a1-1a111aa11111,2222b2b2-b22b-22b2-b2b2-2b222bb22222")]
        [SerializeField] private string _ids;

        [Space]

        [SerializeField] private MultiplayLifecycleAllocationsAPI _lifecycleAllocationAPI;

        public string Ids
        {
            get => _ids;
            set => _ids = value;
        }

        public ListFleetAllocationsRequest GetRequest()
        {
            return new ListFleetAllocationsRequest()
            {
                age = _age,
                limit = _limit,
                offset = _offset,
                ids = _ids
            };
        }

        [Button]
        public void SendRequest()
        {
            SendRequest((result) =>
            {
                Debug.Log($"{result.allocations.Length} {result.allocations[0].ipv4}");
            });
        }

        public void SendRequest(Action<AllocationList> onResult, Action<string> onError = null)
        {
            _lifecycleAllocationAPI.ListFleetAllocations(GetRequest(), onResult, onError);
        }
    }

    [Serializable]
    public class ListFleetAllocationsRequest
    {
        [Tooltip("Example: 2h")]
        public string age;
        [Tooltip("This must be a value between 1 and 50. Anything above will be hard limited to 50 results.")]
        public int limit = 10;
        [Tooltip("Optional parameter used for pagination. Represents the offset at which to start pagination.")]
        public int offset;
        [Tooltip("Example: ids=1111a1a1-a11a-11a1-a1a1-1a111aa11111,2222b2b2-b22b-22b2-b2b2-2b222bb22222")]
        public string ids;

        public string GetQueryParameters()
        {
            if (!string.IsNullOrEmpty(ids))
            {
                return $"?ids={ids}";
            }
            else
            {
                return $"?age={age}&limit={limit}&offset={offset}";
            }
        }
    }

    [Serializable]
    public class AllocationList
    {
        public Allocation[] allocations;
        public Pagination pagination;
    }

    [Serializable]
    public class Allocation
    {
        public string allocationId;
        public int buildConfigurationId;
        public string created;
        public string fleetId;
        public string fulfilled;
        public ushort gamePort;
        public string ipv4;
        public string ipv6;
        public int machineId;
        public bool readiness;
        public string ready;
        public string regionId;
        public string requestId;
        public string requested;
        public int serverId;
    }

    [Serializable]
    public class Pagination
    {
        public int limit;
        public int offset;
    }

    [Serializable]
    public class ActiveAllocations
    {
        public List<ActiveAllocation> allocations;

        public void AddActiveAllocation(string id, string ip, ushort port, bool requester)
        {
            if (allocations == null) allocations = new List<ActiveAllocation>();

            allocations.Add(new ActiveAllocation()
            {
                id = id,
                ipAddress = ip,
                port = port,
                requester = requester
            });
        }

        public void Clear()
        {
            if (allocations != null)
            {
                allocations.Clear();
            }
        }

        public string GetIdsSeparatedByCommas(bool onlyRequested = true)
        {
            if (allocations == null || allocations.Count == 0)
            {
                return string.Empty;
            }

            string ids = string.Empty;
            for (int i = 0; i < allocations.Count; i++)
            {
                if (!onlyRequested || allocations[i].requester)
                {
                    ids += $"{allocations[i].id}{(i != allocations.Count - 1 ? "," : "")}";
                }
            }

            return ids;
        }
    }

    [Serializable]
    public class ActiveAllocation
    {
        public string id;
        public string ipAddress;
        public ushort port;
        public bool requester;
    }
}
