using UnityEngine;

namespace d4160.PlayFab
{
    public abstract class PlayFabRequestSOBase : PlayFabSOBase
    {
        [Header("EVENTS")]
        [SerializeField] protected PlayFabErrorEventSO _errorEvent;
    }
}