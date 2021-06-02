namespace d4160.Core
{
    public enum UnityLifetimeMethodType : byte
    {
        Awake,
        OnEnable,
        Start,
        Manual
    }

    public enum UnityLoopMethodType : byte
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

    public enum RecicleOptionType : byte
    {
        None,
        Add,
        AddAndHide,
        Destroy,
        ReturnToPool
    }

    public enum AssetManagementType : byte
    {
        Default,
        AssetBundles,
        Addressables
    }

    public enum LogLevelType : byte
    {
        None,
        Critical,
        Error,
        Warning,
        Info,
        Debug
    }

    public enum AuthType : byte {
        Login,
        Register,
        Link,
        Unlink
    }
}
