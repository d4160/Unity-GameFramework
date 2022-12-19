using d4160.Singleton;

namespace d4160.Coroutines
{
    /// <summary>
    /// MonoBehaviour when need to start a Co-routine outside a MonoBehaviour
    /// </summary>
    public class CoroutineStarter : Singleton<CoroutineStarter>
    {
        protected override bool DontDestroyOnLoadProp => true;
        protected override bool HideInHierarchy => true;
    }
}