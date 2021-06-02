using System.Collections.Generic;
using UnityEngine;
#if (UNITY_2018_3_OR_NEWER)
using UnityEngine.Android;
#endif

namespace d4160.Permissions
{
    [CreateAssetMenu(menuName = "d4160/Permission/Android")]
    public class AndroidPermissionSO : ScriptableObject
    {
        [SerializeField] private AndroidPermissionType[] _permissions;
#if (UNITY_2018_3_OR_NEWER)
        private List<string> _permissionList = new List<string>();
#endif

        /// <summary>
        /// Genarate the list of enum types to usable string list for android
        /// </summary>
        public void TranslatePermissions()
        {
#if (UNITY_2018_3_OR_NEWER)
            for (var i = 0; i < _permissions.Length; i++)
            {
                string androidPermission = GetAndroidPermission(_permissions[i]);
                if(!_permissionList.Contains(androidPermission))
                    _permissionList.Add(androidPermission);
            }
#endif
        }

        public void RequestPermissions() {
#if (UNITY_2018_3_OR_NEWER)
            for (var i = 0; i < _permissionList.Count; i++)
            {
                if (!Permission.HasUserAuthorizedPermission(_permissionList[i]))
                {                 
                    Permission.RequestUserPermission(_permissionList[i]);
                }
            }
#endif
        }

        public bool HasPermission(AndroidPermissionType permissionType)
        {
#if (UNITY_2018_3_OR_NEWER)
            string androidPermission = GetAndroidPermission(permissionType);
            return Permission.HasUserAuthorizedPermission(androidPermission);
#else
            return false;
#endif
        }

        private string GetAndroidPermission(AndroidPermissionType permissionType){
#if (UNITY_2018_3_OR_NEWER)
            switch (permissionType)
            {
                case AndroidPermissionType.Camera:
                    return Permission.Camera;
                case AndroidPermissionType.Microphone:
                    return Permission.Microphone;
                case AndroidPermissionType.FineLocation:
                    return Permission.FineLocation;
                case AndroidPermissionType.CoarseLocation:
                    return Permission.CoarseLocation;
                case AndroidPermissionType.ExternalStorageRead:
                    return Permission.ExternalStorageRead;
                case AndroidPermissionType.ExternalStorageWrite:
                    return Permission.ExternalStorageWrite;
                default:
                    return string.Empty;
            }
#else
            return string.Empty;
#endif
        }
    }

    public enum AndroidPermissionType {
        Camera,
        Microphone,
        FineLocation,
        CoarseLocation,
        ExternalStorageRead,
        ExternalStorageWrite
    }
}
