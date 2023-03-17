using UnityEngine;
using d4160.Collections;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.AgoraRtc
{
    [CreateAssetMenu(menuName = "d4160/AgoraRtc/Collections/Users")]
    public class UsersRuntimeSetSO : RuntimeSetSOBase<uint>
    {
        [SerializeField] private bool _setStaticVSurfacesOnJoinChannel = false;

        [Header("Data")]
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private VideoSurfaceProviderSO _videoSurfaceProvider;

        [Header("Events")]
        [SerializeField] private JoinChannelEventSO _onJoinChannelSuccess;
        [SerializeField] private JoinChannelEventSO _onRejoinChannelSuccess;
        [SerializeField] private LeaveChannelEventSO _onLeaveChannelSuccess;
        [SerializeField] private UserJoinedEventSO _onUserJoined;
        [SerializeField] private UserOfflineEventSO _onUserOffline;

        public uint LocalUid { get; private set; }

        private JoinChannelEventSO.EventListener _onJoinChannelSuccessListener;
        private JoinChannelEventSO.EventListener _onRejoinChannelSuccessListener;
        private LeaveChannelEventSO.EventListener _onLeaveChannelSuccessListener;
        private UserJoinedEventSO.EventListener _onUserJoinedListener;
        private UserOfflineEventSO.EventListener _onUserOfflineListener;

        public void Setup()
        {
            _onJoinChannelSuccessListener = new ((conn, elapsed) => {
                LocalUid = conn.localUid;

                if (_setStaticVSurfacesOnJoinChannel)
                {
                    _videoSurfaceProvider.EnableStaticVideoSurface();
                }
            });

            _onRejoinChannelSuccessListener = new((conn, elapsed) => {
                LocalUid = conn.localUid;
            });

            _onLeaveChannelSuccessListener = new((conn, stats) =>
            {
                Clear();
                LocalUid = 0;

                if (_setStaticVSurfacesOnJoinChannel)
                {
                    _videoSurfaceProvider.DisableStaticVideoSurface();
                }
                else
                {
                    _videoSurfaceProvider.DisableStaticVideoSurface(0, false);
                }
            });

            _onUserJoinedListener = new((conn, uid, elapsed) => { 
                Add(uid);

                if (_setStaticVSurfacesOnJoinChannel)
                {
                    _videoSurfaceProvider.EnableStaticVideoSurface(uid);
                }
            });

            _onUserOfflineListener = new((conn, uid, reason) => { 
                Remove(uid);

                if (_setStaticVSurfacesOnJoinChannel)
                {
                    _videoSurfaceProvider.DisableStaticVideoSurface(uid);
                }
                else
                {
                    _videoSurfaceProvider.DisableStaticVideoSurface(uid, false);
                }
            });
        }

        public void RegisterEvents()
        {
            //Debug.Log("RegisterEvents");
            _onJoinChannelSuccess.AddListener(_onJoinChannelSuccessListener);
            _onRejoinChannelSuccess.AddListener(_onRejoinChannelSuccessListener);
            _onLeaveChannelSuccess.AddListener(_onLeaveChannelSuccessListener);
            _onUserJoined.AddListener(_onUserJoinedListener);
            _onUserOffline.AddListener(_onUserOfflineListener);
        }

        public void UnregisterEvents()
        {
            _onJoinChannelSuccess.RemoveListener(_onJoinChannelSuccessListener);
            _onRejoinChannelSuccess.RemoveListener(_onRejoinChannelSuccessListener);
            _onLeaveChannelSuccess.RemoveListener(_onLeaveChannelSuccessListener);
            _onUserJoined.RemoveListener(_onUserJoinedListener);
            _onUserOffline.RemoveListener(_onUserOfflineListener);
        }
    }
}