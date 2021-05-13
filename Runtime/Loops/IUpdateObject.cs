
namespace d4160.Loops
{
    public interface IUpdateObject
    {
        void OnUpdate(float deltaTime);
    }

    public interface ILaterUpdateInstance
    {
        void OnUpdate(float deltaTime);
    }

    public interface IFixedUpdateInstance
    {
        void OnUpdate(float fixedDeltaTime);
    }
}