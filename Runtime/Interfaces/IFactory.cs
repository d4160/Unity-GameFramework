namespace d4160.Core
{
    public interface IFactory<T>
    {
        T Create(int option = 0);
    }
}
