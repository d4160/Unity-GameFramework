using System.Collections;
using d4160.Events;
using d4160.Loops;
using d4160.Singleton;
using NaughtyAttributes;
using Unity.Netcode;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using UnityEngine;

namespace d4160.UGS.Multiplay
{
    public class MultiplayServiceMono : Singleton<MultiplayServiceMono>, IUpdateObject
    {
        [SerializeField] private bool _startServerAtStart;
        [SerializeField] private bool _setReadyServerForPlayersOnServerStarted;

        [Header("Data")]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private MultiplaySO _multiplay;

        private float _autoAllocateTimer = 9999999f;
        private VoidEventSO.EventListener _onServerStartedLtn;

        protected override void Awake()
        {
            base.Awake();

            _onServerStartedLtn = new(async () =>
            {
                if (_setReadyServerForPlayersOnServerStarted)
                {
                    await MultiplayService.Instance.ReadyServerForPlayersAsync();

                    _multiplay.LogInfo($"ReadyServerForPlayers was called");
                }

                Camera.main.enabled = false;
            });
        }

        protected void OnEnable()
        {
            UpdateManager.AddListener(this);

            _multiplay.OnServerStarted.AddListener(_onServerStartedLtn);
        }

        protected void OnDisable()
        {
            UpdateManager.RemoveListener(this);

            _multiplay.OnServerStarted.RemoveListener(_onServerStartedLtn);
        }

        private IEnumerator Start()
        {
            while (UnityServices.State != ServicesInitializationState.Initialized)
            {
                yield return null;
            }

            if (_startServerAtStart)
            {
                _multiplay.SubscribeEvents();
                _ = _multiplay.StartServerAsync();
            }
        }

        void OnDestroy()
        {
            if (_startServerAtStart)
            {
                _multiplay.UnsubscribeEvents();
            }
        }

        void IUpdateObject.OnUpdate(float deltaTime)
        {
            _autoAllocateTimer -= deltaTime;
            if (_autoAllocateTimer <= 0)
            {
                _autoAllocateTimer = 999f;
                Debug.Log("AutoAllocateTimer time out!");
            }

            if (_multiplay.ServerQueryHandler != null)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    _multiplay.ServerQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
                }
                _multiplay.ServerQueryHandler.UpdateServerCheck();
            }
        }
    }
}