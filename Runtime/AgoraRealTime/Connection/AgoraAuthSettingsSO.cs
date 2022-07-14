#if AGORA
using UnityEngine;
namespace d4160.Agora
{
    [CreateAssetMenu(menuName = "d4160/Agora/Settings")]
    public class AgoraAuthSettingsSO : ScriptableObject
    {
        [SerializeField] private string _appID = "your_appid";

        public string AppID => _appID;
    }
}
#endif