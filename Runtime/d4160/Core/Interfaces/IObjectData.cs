using UnityEngine;

namespace d4160.Core
{
    public interface IObjectData
    {
        object GetData();

        T GetDataAs<T>();

        void SetData(object data);
    }

    public interface IObjectData<T>
    {
        T GetData();

        void SetData(T data);
    }
}

