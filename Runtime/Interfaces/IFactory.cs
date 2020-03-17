using UnityEngine;

namespace d4160.Core
{
    public interface IFactory<T>
    {
        T Fabricate(Vector3 position, Quaternion rotation);
    }

    public interface IClassFactory<T>
    {
        T Fabricate(int option = 0);
    }
}
