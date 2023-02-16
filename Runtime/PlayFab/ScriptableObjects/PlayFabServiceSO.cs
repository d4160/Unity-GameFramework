using d4160.Logging;
using UnityEngine;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.PlayFab
{
    [CreateAssetMenu(menuName = "d4160/PlayFab/Service")]
    public class PlayFabServiceSO : PlayFabSOBase
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public LoggerSO logger;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Button]
#endif
        public void Setup()
        {
            _service.Logger = logger;
        }
    }
}
