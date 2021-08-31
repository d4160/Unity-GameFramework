using d4160.MonoBehaviourData;
using d4160.Tick;
using UnityEngine;

namespace d4160.Permissions
{
    public class AndroidPermissionBehaviour : MonoBehaviourUnityData<AndroidPermissionSO>, ITickObject
    {
        [SerializeField] private int requestsBySecond;

        void Start() {
            TranslatePermissions();
            TickManager.RegisterObject(requestsBySecond, this);
        }

        void ITickObject.OnTick(float fixedDeltaTime)
        {
            RequestPermissions();
        }

        /// <summary>
        /// Genarate the list of enum types to usable string list for android
        /// </summary>
        public void TranslatePermissions()
        {
            if(_data) {
                _data.TranslatePermissions();
            }
        }

        public void RequestPermissions() {
            if(_data) {
                _data.RequestPermissions();
            }
        }

        public bool HasPermission(AndroidPermissionType permissionType)
        {
            if(_data) {
                return _data.HasPermission(permissionType);
            }

            return false;
        }
    }
}