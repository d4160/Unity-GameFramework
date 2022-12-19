#if AGORA
using System;
using d4160.Core;
using Agora.Rtc;
using d4160.Authentication;
using UnityEngine;
using M31Logger = d4160.Logging.LoggerM31;

namespace d4160.Agora_
{
    public class AgoraAuthService : BaseAuthService
    {
        public static event Action<uint, string> OnLocalUserRegistered;

        private string _userAccount;
        public string UserAccount { get => _userAccount; set => _userAccount = value; }

        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        private readonly AgoraConnectionService _connection = AgoraConnectionService.Instance;

        public static AgoraAuthService Instance => _instance ?? (_instance = new AgoraAuthService());
        private static AgoraAuthService _instance;

        private void CallOnLocalUserRegisteredEvent(uint uid, string userAccount) {
            OnLocalUserRegistered?.Invoke(uid, userAccount);
            M31Logger.LogInfo($"AGORA: RTC Info: LocalUser Registered:{userAccount}, uid:{uid}", LogLevel);
        }

        private AgoraAuthService()
        {
            _instance = this;
        }

        public void RegisterEvents() {

            //_connection.RtcEngine.OnLocalUserRegistered += CallOnLocalUserRegisteredEvent;
        }

        public void UnregisterEvents() {

            //_connection.RtcEngine.OnLocalUserRegistered -= CallOnLocalUserRegisteredEvent;
        }

        public override void Login(UnityEngine.Promise.Completer completer)
        {
            if(CheckErrors()) return;

            completer.Resolve();
        }

        public override void Register(UnityEngine.Promise.Completer completer)
        {
            if(CheckErrors()) return;

            _connection.RtcEngine.RegisterLocalUserAccount(_connection.AgoraSettings.AppID, _userAccount);
            completer.Resolve();
        }

        public override void Logout(UnityEngine.Promise.Completer completer)
        {
            if(CheckErrors()) return;

            completer.Resolve();
        }

        private bool CheckErrors(){
            
            if(!Application.isPlaying) {
                M31Logger.LogWarning("AGORA: Cannot use Agora in EditMode.", LogLevel);
                return true;
            }

            return false;
        }
    }
}
#endif