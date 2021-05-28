
namespace d4160.Instancers {
    public interface IProvider<T>
    {
        T Instantiate();

        void Destroy(T instance);
    }
}