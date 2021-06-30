#if AGORA
using d4160.MonoBehaviourData;
using NaughtyAttributes;
using UltEvents;
using UnityEngine;

namespace d4160.Auth.Agora
{
    public class AgoraAuthBehaviour : MonoBehaviourUnityData<AgoraAuthSO>
    {
        [SerializeField] private UltEvent<uint, string> _onLocalUserRegistered;

        void OnEnable(){
            if (_data)
            {
                _data.RegisterEvents();
                _data.OnLocalUserRegisteredEvent += _onLocalUserRegistered.Invoke;
            }
        }

        void OnDisable(){
            if (_data)
            {
                _data.UnregisterEvents();
                _data.OnLocalUserRegisteredEvent -= _onLocalUserRegistered.Invoke;
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