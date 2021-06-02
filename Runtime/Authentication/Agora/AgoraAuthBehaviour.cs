#if AGORA
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using UltEvents;
using UnityEngine;

namespace d4160.Auth.Agora
{
    public class AgoraAuthBehaviour : MonoBehaviourUnityData<AgoraAuthSO>
    {
        [SerializeField] private UltEvent<int, string> _onAuthError;
        [SerializeField] private UltEvent<int, string> _onAuthWarning;

        public void RegisterEvents(){
            if (_data)
            {
                _data.RegisterEvents();
                _data.OnAuthError += _onAuthError.Invoke;
                _data.OnAuthWarning += _onAuthWarning.Invoke;
            }
        }

        public void UnregisterEvents(){
            if (_data)
            {
                _data.UnregisterEvents();
                _data.OnAuthError -= _onAuthError.Invoke;
                _data.OnAuthWarning -= _onAuthWarning.Invoke;
            }
        }

        [Button]
        public void Login(){
            if(_data) {
                _data.Login();
            }
        }

        [Button]
        public void Register(){
            if(_data) {
                _data.Register();
            }
        }

        [Button]
        public void Logout(){
            if(_data) {
                _data.Logout();
            }
        }
    }

}
#endif