namespace d4160.Core
{
    public enum UnityInitMethod : byte
    {
        Awake,
        OnEnable,
        Start,
        Manual
    }

    public enum UnityLoopMethod : byte
    {
        FixedUpdate,
        Update,
        LateUpdate,
        Manual
    }

    public enum LoopType : byte
    {
        Random,
        Restart,
        PingPong
    }

    public enum RecicleOption
    {
        None,
        Add,
        Destroy,
        Reuse
    }
}
