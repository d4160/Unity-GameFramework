using Agora.Rtc;
using d4160.Variables;
using UnityEngine;

namespace d4160.Agora_
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
