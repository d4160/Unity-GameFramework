using UnityEngine;

namespace d4160.PlayFab
{
    public abstract class PlayFabSOBase : ScriptableObject
    {
        protected static readonly PlayFabService _service = PlayFabService.Instance;
    }
}