namespace d4160.Core
{
    public enum UnityInitMethod : byte 
    {
        Awake,
        OnEnable,
        Start
    }

    public enum UnityInitMethodWithManual : byte
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
        LateUpdate
    }

    public enum UnityLoopMethodWithManual : byte
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
}
