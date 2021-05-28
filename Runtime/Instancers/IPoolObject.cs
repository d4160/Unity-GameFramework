
namespace d4160.Instancers
{
    public interface IPoolObject<T>
    {
        IObjectPool<T> Pool { get; set; }

        /// <summary>
        /// Just call Pool?.Destroy(this); in implementation
        /// </summary>
        void Destroy();
    }
}