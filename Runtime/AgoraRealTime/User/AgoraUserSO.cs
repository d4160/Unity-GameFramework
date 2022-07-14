#if AGORA
using System;
using agora_gaming_rtc;
using UnityEngine;
using System.Collections.Generic;
using d4160.Instancers;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.Agora
{
    [CreateAssetMenu(menuName = "d4160/Chat/Agora User")]
    public class AgoraUserSO : ScriptableObject
    {
        [SerializeField] private bool _autoVideoSurface = true;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private ComponentProviderSOBase _provider;
        [SerializeField] AgoraVideoSurfaceType _agoraVideoSurfaceType = AgoraVideoSurfaceType.Renderer;
        [SerializeField] uint _videoFps = 30;
        [SerializeField] bool _enableFlipHorizontal = true;
        [SerializeField] bool _enableFlipVertical = false;
        [SerializeField] float _otherVideoSurfaceScaleMultiplier = .25f;

        public event Action<uint, int> OnUserJoinedEvent;
        public event Action<uint, USER_OFFLINE_REASON> OnUserOfflineEvent;

        private readonly AgoraUserService _userService = AgoraUserService.Instance; 

        public Dictionary<uint, VideoSurface> UserVideoDict => _userService.UserVideoDictionary;

        public ComponentProviderSOBase VideoSurfaceProvider { get => _provider; set => _provider = value; }
        public AgoraVideoSurfaceType AgoraVideoSurfaceType { get => _agoraVideoSurfaceType; set => _agoraVideoSurfaceType = value; }
        public uint VideoFps { get => _videoFps; set => _videoFps = value; }
        public bool EnableFlipHorizontal { get => _enableFlipHorizontal; set => _enableFlipHorizontal = value; }
        public bool EnableFlipVertical { get => _enableFlipVertical; set => _enableFlipVertical = value; }
        public float OtherVideoSurfaceScaleMultiplier { get => _otherVideoSurfaceScaleMultiplier; set => _otherVideoSurfaceScaleMultiplier = value; }

        private void CallOnUserJoined(uint uid, int elapsed) => OnUserJoinedEvent?.Invoke(uid, elapsed);
        private void CallOnUserOfflineEvent(uint uid, USER_OFFLINE_REASON reason) => OnUserOfflineEvent?.Invoke(uid, reason);

        public void RegisterEvents () {
            // if (_provider) _provider.RegisterEvents(); // If want events use the ComponentProviderBehaviour
            _userService.RegisterEvents();
            AgoraUserService.OnUserJoinedEvent += CallOnUserJoined;
            AgoraUserService.OnUserOfflineEvent += CallOnUserOfflineEvent;
        }

        public void UnregisterEvents(){
            // if (_provider) _provider.UnregisterEvents();
            _userService.UnregisterEvents();
            AgoraUserService.OnUserJoinedEvent -= CallOnUserJoined;
            AgoraUserService.OnUserOfflineEvent -= CallOnUserOfflineEvent;
        }   

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Setup() {
            if (_provider) _provider.Setup();

            _userService.AutoVideoSurface = _autoVideoSurface;
            _userService.VideoSurfaceProvider = _provider;
            _userService.AgoraVideoSurfaceType = _agoraVideoSurfaceType;
            _userService.VideoFps = _videoFps;
            _userService.EnableFlipHorizontal = _enableFlipHorizontal;
            _userService.EnableFlipVertical = _enableFlipVertical;
            _userService.OtherVideoSurfaceScaleMultiplier = _otherVideoSurfaceScaleMultiplier;
        }
    }
}
#endif