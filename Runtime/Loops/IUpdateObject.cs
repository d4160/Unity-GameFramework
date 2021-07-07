
namespace d4160.Loops
{
    public interface IUpdateObject
    {
        void OnUpdate(float deltaTime);
    }

    public interface ILateUpdateObject
    {
        void OnLateUpdate(float deltaTime);
    }

    public interface IFixedUpdateObject
    {
        void OnFixedUpdate(float fixedDeltaTime);
    }
}