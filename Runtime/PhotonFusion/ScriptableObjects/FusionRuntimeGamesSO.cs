using d4160.Events;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.Fusion
{
    [CreateAssetMenu(menuName = "d4160/Fusion/RuntimeGames")]
    public class FusionRuntimeGamesSO : ScriptableObject, IEventListener<NetworkRunner, List<SessionInfo>>
    {
        public SessionListUpdatedEventSO _onSessionListUpdatedEventSO;

        public List<SessionInfo> Games { get; private set; } = new();

        public void RegisterEvents()
        {
            _onSessionListUpdatedEventSO.AddListener(this);
        }

        public void UnregisterEvents()
        {
            _onSessionListUpdatedEventSO.RemoveListener(this);
        }

        void IEventListener<NetworkRunner, List<SessionInfo>>.OnInvoked(NetworkRunner runner, List<SessionInfo> games)
        {
            Games = games;
        }
    }
}