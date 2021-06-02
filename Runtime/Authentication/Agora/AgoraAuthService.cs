#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using d4160.Authentication;
using UnityEngine;
using Logger = d4160.Logging.Logger;

namespace d4160.Auth.Agora
{
    public class AgoraAuthService : BaseAuthService
    {
        public static event Action<int, string> OnAuthError;
        public static event Action<int, string> OnAuthWarning;

        private string _userAccount;
        private IRtcEngine _RtcEngine;

        public IRtcEngine RtcEngine => _RtcEngine;
        public string UserAccount { get => _userAccount; set => _userAccount = value; }
        public AgoraAuthSettingsSO AgoraSettings { get; set; } 

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        public static AgoraAuthService Instance => _instance ?? (_instance = new AgoraAuthService());
        private static AgoraAuthService _instance;

        private AgoraAuthService()
        {
            _instance = this;
        }

        public override void Login(UnityEngine.Promise.Completer completer)
        {
            if(CheckErrors()) return;

            LoadEngine();

            completer.Resolve();
        }

        public override void Register(UnityEngine.Promise.Completer completer)
        {
            if(CheckErrors()) return;

            LoadEngine();

            _RtcEngine.RegisterLocalUserAccount(AgoraSettings.AppID, _userAccount);
            completer.Resolve();
        }

        public override void Logout(UnityEngine.Promise.Completer completer)
        {
            UnloadEngine();
            completer.Resolve();
        }

        private bool CheckErrors(){
            if(!AgoraSettings) {
                Logger.LogWarning("You need to pass an AgoraAuthSettingsSO asset to AgoraAuthService", LogLevel);
                return true;
            }

            if(AgoraSettings.AppID.Length < 10) {
                Logger.LogWarning("You need to specify an AppID in the AgoraAuthSettingsSO", LogLevel);
                return true;
            }

            return false;
        }

        private void LoadEngine(){

            if(_RtcEngine != null) return;

            _RtcEngine = IRtcEngine.GetEngine(AgoraSettings.AppID);

            _RtcEngine.OnError = (code, msg) =>
            {
                OnAuthError?.Invoke(code, msg);
                Debug.LogErrorFormat("RTC Error:{0}, msg:{1}", code, IRtcEngine.GetErrorDescription(code));
            };

            _RtcEngine.OnWarning = (code, msg) =>
            {
                OnAuthWarning?.Invoke(code, msg);
                Debug.LogWarningFormat("RTC Warning:{0}, msg:{1}", code, IRtcEngine.GetErrorDescription(code));
            };

            _RtcEngine.OnLocalUserRegistered = (uid, userAccount) => {
                Debug.Log($"RTC Info: LocalUser Registered:{userAccount}, uid:{uid}");
            };

            _RtcEngine.SetLogFilter(GetAgoraLogLevel(LogLevel));
        }

        private void UnloadEngine() {
            if (_RtcEngine != null)
            {
                IRtcEngine.Destroy();  // Place this call in ApplicationQuit
                _RtcEngine = null;
            }
        }

        private static LOG_FILTER GetAgoraLogLevel(LogLevelType logLevel) {
            switch(logLevel){
                case LogLevelType.Debug:
                    return LOG_FILTER.DEBUG;
                case LogLevelType.Info:
                    return LOG_FILTER.INFO;
                case LogLevelType.Warning:
                    return LOG_FILTER.WARNING;
                case LogLevelType.Error:
                    return LOG_FILTER.ERROR;
                case LogLevelType.Critical:
                    return LOG_FILTER.CRITICAL;
                case LogLevelType.None:
                    return LOG_FILTER.OFF;
                default:
                    return LOG_FILTER.OFF;
            }
        }
    }
}
#endif