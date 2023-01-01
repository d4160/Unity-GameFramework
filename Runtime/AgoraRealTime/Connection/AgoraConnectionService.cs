#if AGORA
using System;
using d4160.Core;
using Agora.Rtc;
using UnityEngine;
using M31Logger = d4160.Logging.LoggerM31;

namespace d4160.Agora_
{
    public class AgoraConnectionService
    {
        public static event Action<int, string> OnError;

        public IRtcEngine RtcEngine { get; private set; }
        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;
        public AgoraAuthSettingsSO AgoraSettings { get; private set; }
        public AgoraEventHandler AgoraEventHandler { get; private set; }

        public static AgoraConnectionService Instance => _instance ?? (_instance = new AgoraConnectionService());
        private static AgoraConnectionService _instance;

        private AgoraConnectionService()
        {
            _instance = this;
        }

        private bool CheckErrors(AgoraAuthSettingsSO settings){
            
            if(RtcEngine != null) {
                M31Logger.LogWarning("AGORA: RtcEngine is already loaded.", LogLevel);
                return true;
            }

            if(!Application.isPlaying) {
                M31Logger.LogWarning("AGORA: Cannot use Agora in EditMode.", LogLevel);
                return true;
            }

            if(!settings) {
                M31Logger.LogWarning("AGORA: You need to pass an AgoraAuthSettingsSO asset to AgoraAuthService", LogLevel);
                return true;
            }

            if(settings.AppID.Length < 10) {
                M31Logger.LogWarning("AGORA: You need to specify an AppID in the AgoraAuthSettingsSO", LogLevel);
                return true;
            }

            return false;
        }

        public void InitEngine(AgoraAuthSettingsSO settings)
        {
            if (CheckErrors(settings)) {
                return;
            }

            AgoraSettings = settings;

            RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
            AgoraEventHandler = new AgoraEventHandler(this);
            RtcEngineContext context = new RtcEngineContext(settings.AppID, 0,
                                        CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING,
                                        AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);
            RtcEngine.Initialize(context);
            RtcEngine.InitEventHandler(AgoraEventHandler);

            M31Logger.LogInfo("AGORA: Successfully RTCEngine Initialized", LogLevel);
        }

        // In Application.Quit()
        public void ShutdownEngine() 
        {
            if (RtcEngine == null) return;
            RtcEngine.InitEventHandler(null);
            RtcEngine.LeaveChannel();
            RtcEngine.Dispose();
        }

        public void CallOnError(int err, string msg)
        {
            M31Logger.LogError($"AGORA: OnError:{err}, msg:{msg}", LogLevel);
            OnError?.Invoke(err, msg);
        }

        public void Log(string msg)
        {
            M31Logger.LogInfo($"AGORA: {msg}", LogLevel);
        }
    }

    public class AgoraEventHandler : IRtcEngineEventHandler
    {
        private readonly AgoraConnectionService _agoraConn;

        public AgoraChannelService ChannelService { get; set; }
        public AgoraUserService UserService { get; set; }

        internal AgoraEventHandler(AgoraConnectionService agoraConn)
        {
            _agoraConn = agoraConn;
        }

        public override void OnError(int err, string msg)
        {
            _agoraConn.CallOnError(err, msg);
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            if (ChannelService == null) return;

            ChannelService.CallOnJoinChannelSuccess(connection, elapsed);
        }

        public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            if (ChannelService == null) return;

            ChannelService.CallOnReJoinChannelSuccess(connection, elapsed);
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            if (ChannelService == null) return;

            ChannelService.CallOnLeaveChannel(connection, stats);
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole)
        {
            _agoraConn.Log("OnClientRoleChanged");
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            if (UserService == null) return;

            UserService.CallOnUserJoined(connection, uid, elapsed);
            //JoinChannelVideo.MakeVideoView(uid, _agoraConn.GetChannelName());
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            if (UserService == null) return;

            //JoinChannelVideo.DestroyVideoView(uid);
            UserService.CallOnUserOfflineEvent(connection, uid, reason);
        }

        public override void OnUplinkNetworkInfoUpdated(UplinkNetworkInfo info)
        {
            _agoraConn.Log("OnUplinkNetworkInfoUpdated");
        }

        public override void OnDownlinkNetworkInfoUpdated(DownlinkNetworkInfo info)
        {
            _agoraConn.Log("OnDownlinkNetworkInfoUpdated");
        }
    }

}
#endif