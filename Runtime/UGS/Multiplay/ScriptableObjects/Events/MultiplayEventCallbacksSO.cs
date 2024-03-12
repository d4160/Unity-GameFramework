using System.Collections;
using System.Collections.Generic;
using d4160.Logging;
using NaughtyAttributes;
using Unity.Services.Multiplay;
using UnityEngine;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/Events/MultiplayCallbacks")]
    public class MultiplayEventCallbacksSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] LoggerSO _logger;

        [Header("Events")]
        [SerializeField] private MultiplayAllocationEventSO _onAllocate;
        [SerializeField] private MultiplayDeallocationEventSO _onDeallocate;
        [SerializeField] private MultiplayServerSubscriptionStateEventSO _onSubscriptionStateChanged;
        [SerializeField] private MultiplayErrorEventSO _onError;

        public MultiplayAllocationEventSO OnAllocate => _onAllocate;

        public MultiplayEventCallbacks GetEventCallbacks()
        {
            var callbacks = new MultiplayEventCallbacks();
            callbacks.Allocate += (allocation) =>
            {
                if (_onAllocate)
                {
                    _onAllocate.Invoke(allocation);
                }

                if (_logger) _logger.LogInfo($"[Allocate] AllocationId: {allocation.AllocationId}, ServerId:{allocation.ServerId}, EventId:{allocation.EventId}");
            };

            callbacks.Deallocate += (deallocation) =>
            {
                if (_onDeallocate)
                {
                    _onDeallocate.Invoke(deallocation);
                }

                if (_logger) _logger.LogInfo($"[Deallocate] AllocationId: {deallocation.AllocationId}, ServerId:{deallocation.ServerId}, EventId:{deallocation.EventId}");
            };

            callbacks.Error += (error) =>
            {
                if (_onError)
                {
                    _onError.Invoke(error);
                }

                if (_logger) _logger.LogInfo($"[Error] Reason: {error.Reason}, Detail: {error.Detail}");
            };

            callbacks.SubscriptionStateChanged += (state) =>
            {
                if (_onSubscriptionStateChanged)
                {
                    _onSubscriptionStateChanged.Invoke(state);
                }

                if (_logger) _logger.LogInfo($"[SubscriptionStateChanged] State: {state}");
            };

            return callbacks;
        }
    }
}