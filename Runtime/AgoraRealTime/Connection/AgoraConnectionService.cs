#if AGORA
using System;
using d4160.Core;
using agora_gaming_rtc;
using UnityEngine;
using M31Logger = d4160.Logging.LoggerM31;

namespace d4160.Agora
{
    public class AgoraConnectionService
    {
        public static event Action<int, string> OnEngineError;
        public static event Action<int, string> OnEngineWarning;

        public IRtcEngine RtcEngine { get; private set; }
        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;
        public AgoraAuthSettingsSO AgoraSettings { get; private set; }

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
                M31Logger.LogWarning("You need to pass an AgoraAuthSettingsSO asset to AgoraAuthService", LogLevel);
                return true;
            }

            if(settings.AppID.Length < 10) {
                M31Logger.LogWarning("You need to specify an AppID in the AgoraAuthSettingsSO", LogLevel);
                return true;
            }

            return false;
        }

        public void LoadEngine(AgoraAuthSettingsSO settings)
        {
            if (CheckErrors(settings)) {
                return;
            }

            AgoraSettings = settings;

            RtcEngine = IRtcEngine.GetEngine(settings.AppID);

            RtcEngine.OnError = (code, msg) =>
            {
                OnEngineError?.Invoke(code, msg);
                M31Logger.LogError($"AGORA: RTC Error:{code}, msg:{IRtcEngine.GetErrorDescription(code)}", LogLevel);
            };

            RtcEngine.OnWarning = (code, msg) =>
            {
                OnEngineWarning?.Invoke(code, msg);
                M31Logger.LogWarning($"AGORA: RTC Warning:{code}, msg:{IRtcEngine.GetErrorDescription(code)}", LogLevel);
            };

            RtcEngine.SetLogFilter(GetAgoraLogLevel(LogLevel));

            M31Logger.LogInfo("AGORA: Successfully RTCEngine loaded", LogLevel);
        }

        public void UnloadEngine() {
            if (RtcEngine != null)
            {
                IRtcEngine.Destroy();  // Place this call in ApplicationQuit
                RtcEngine = null;
            }
        }

        private static LOG_FILTER GetAgoraLogLevel(LogLevelType logLevel) {
            switch(logLevel){
                case LogLevelType.Debug:
                    return LOG_FILTER.DEBUG | LOG_FILTER.INFO | LOG_FILTER.WARNING | LOG_FILTER.ERROR | LOG_FILTER.CRITICAL;
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