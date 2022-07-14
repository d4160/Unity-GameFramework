using agora_gaming_rtc;
using d4160.Variables;
using UnityEngine;

namespace d4160.Agora
{
    [CreateAssetMenu(menuName = "d4160/Agora/VideoSurface Variable")]
    public class VideoSurfaceVariableSO : VariableSOBase<VideoSurface>
    {

    }

    [System.Serializable]
    public class VideoSurfacetReference : VariableReference<VideoSurfaceVariableSO, VideoSurface>
    {
    }
}
