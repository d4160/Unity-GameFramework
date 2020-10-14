using UnityEngine;

namespace d4160.Core
{
    public abstract class MonoBehaviourData<T> : MonoBehaviour
    {
        [SerializeField] protected T _data;
    }

    public abstract class MonoBehaviourData<T1, T2> : MonoBehaviour
    {
        [SerializeField] protected T1 _data1;
        [SerializeField] protected T2 _data2;
    }

    public abstract class MonoBehaviourData<T1, T2, T3> : MonoBehaviour
    {
        [SerializeField] protected T1 _data1;
        [SerializeField] protected T2 _data2;
        [SerializeField] protected T3 _data3;
    }

    public abstract class MonoBehaviourData<T1, T2, T3, T4> : MonoBehaviour
    {
        [SerializeField] protected T1 _data1;
        [SerializeField] protected T2 _data2;
        [SerializeField] protected T3 _data3;
        [SerializeField] protected T4 _data4;
    }
}
