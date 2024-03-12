using NaughtyAttributes;
using UnityEngine;

namespace d4160.UGS.Multiplay.LifecycleAPI
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/API/Lifecycle/RemoveAllocationRequest")]
    public class RemoveAllocationRequestSO : ScriptableObject
    {
        [SerializeField] private string _allocationId;

        [Space]

        [SerializeField] private MultiplayLifecycleAllocationsAPI _lifecycleAllocationAPI;

        [Button]
        public void SendRequest()
        {
            _lifecycleAllocationAPI.RemoveAllocation(_allocationId);
        }
    }
}
