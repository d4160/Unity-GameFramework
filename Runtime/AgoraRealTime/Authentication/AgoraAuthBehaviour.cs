#if AGORA
using d4160.MonoBehaviours;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif
using UltEvents;
using UnityEngine;

namespace d4160.Agora_
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

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Login(){
            if(_data) {
                _data.Login();
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Register(){
            if(_data) {
                _data.Register();
            }
        }

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Logout(){
            if(_data) {
                _data.Logout();
            }
        }
    }

}
#endif