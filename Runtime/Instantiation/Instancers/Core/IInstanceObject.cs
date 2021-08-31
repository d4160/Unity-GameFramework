
namespace d4160.Instancers
{
    public interface IInstanceObject<T> : IDestroyable where T : class
    {
        /// <summary>
        /// Just call Pool?.Destroy(this); in implementation
        /// </summary>
        IProvider<T> Provider { get; set; }       
    }
}