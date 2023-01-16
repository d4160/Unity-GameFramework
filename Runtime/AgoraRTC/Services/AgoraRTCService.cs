using Agora.Rtc;
using d4160.Agora_;

namespace d4160.AgoraRTC
{
    public class AgoraRTCService
    {
        public AgoraRTCSettingsSO SettingsSO { get; set; }

        internal IRtcEngine _rtcEngine = null;

        public void InitRtcEngine()
        {
            _rtcEngine = RtcEngine.CreateAgoraRtcEngine();
            UserEventHandler handler = new(this);
            RtcEngineContext context = new(SettingsSO.AppID, SettingsSO.Context,
                                        SettingsSO.ChannelProfileType,
                                        SettingsSO.AudioScenarioType);
            _rtcEngine.Initialize(context);
            _rtcEngine.InitEventHandler(handler);
        }
    }

    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly AgoraRTCService _service;

        internal UserEventHandler(AgoraRTCService service)
        {
            _service = service;
        }


        public override void OnError(int err, string msg)
        {
            _service.Log.UpdateLog(string.Format("OnError err: {0}, msg: {1}", err, msg));
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            int build = 0;
            _service.Log.UpdateLog(string.Format("sdk version: ${0}",
                _service.RtcEngine.GetVersion(ref build)));
            _service.Log.UpdateLog(
                string.Format("OnJoinChannelSuccess channelName: {0}, uid: {1}, elapsed: {2}",
                                connection.channelId, connection.localUid, elapsed));
        }

        public override void OnRejoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            _service.Log.UpdateLog("OnRejoinChannelSuccess");
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            _service.Log.UpdateLog("OnLeaveChannel");
        }

        public override void OnClientRoleChanged(RtcConnection connection, CLIENT_ROLE_TYPE oldRole, CLIENT_ROLE_TYPE newRole)
        {
            _service.Log.UpdateLog("OnClientRoleChanged");
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            _service.Log.UpdateLog(string.Format("OnUserJoined uid: ${0} elapsed: ${1}", uid, elapsed));
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            _service.Log.UpdateLog(string.Format("OnUserOffLine uid: ${0}, reason: ${1}", uid,
                (int)reason));
        }
    }
}