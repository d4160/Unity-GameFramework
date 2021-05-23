using NaughtyAttributes;
using UnityEngine;

namespace d4160.MonoBehaviourData
{
    public abstract class MonoBehaviourUnityData<T> : MonoBehaviour
    {
        [Expandable]
        [SerializeField] protected T _data;

        public T Data
        {
            get => _data;
            set => _data = value;
        }
    }

    public abstract class MonoBehaviourUnityData<T1, T2> : MonoBehaviour
    {
        [Expandable]
        [SerializeField] protected T1 _data1;
        [Expandable]
        [SerializeField] protected T2 _data2;

        public T1 Data1
        {
            get => _data1;
            set => _data1 = value;
        }

        public T2 Data2
        {
            get => _data2;
            set => _data2 = value;
        }
    }

    public abstract class MonoBehaviourUnityData<T1, T2, T3> : MonoBehaviour
    {
        [Expandable]
        [SerializeField] protected T1 _data1;
        [Expandable]
        [SerializeField] protected T2 _data2;
        [Expandable]
        [SerializeField] protected T3 _data3;

        public T1 Data1
        {
            get => _data1;
            set => _data1 = value;
        }

        public T2 Data2
        {
            get => _data2;
            set => _data2 = value;
        }

        public T3 Data3
        {
            get => _data3;
            set => _data3 = value;
        }
    }

    public abstract class MonoBehaviourUnityData<T1, T2, T3, T4> : MonoBehaviour
    {
        [Expandable]
        [SerializeField] protected T1 _data1;
        [Expandable]
        [SerializeField] protected T2 _data2;
        [Expandable]
        [SerializeField] protected T3 _data3;
        [Expandable]
        [SerializeField] protected T4 _data4;

        public T1 Data1
        {
            get => _data1;
            set => _data1 = value;
        }

        public T2 Data2
        {
            get => _data2;
            set => _data2 = value;
        }

        public T3 Data3
        {
            get => _data3;
            set => _data3 = value;
        }

        public T4 Data4
        {
            get => _data4;
            set => _data4 = value;
        }
    }

    public abstract class MonoBehaviourData<T> : MonoBehaviour
    {
        [Expandable]
        [SerializeField] protected T _data;

        public T Data
        {
            get => _data;
            set => _data = value;
        }
    }

    public abstract class MonoBehaviourData<T1, T2> : MonoBehaviour
    {
        [Expandable]
        [SerializeField] protected T1 _data1;
        [Expandable]
        [SerializeField] protected T2 _data2;

        public T1 Data1
        {
            get => _data1;
            set => _data1 = value;
        }

        public T2 Data2
        {
            get => _data2;
            set => _data2 = value;
        }
    }

    public abstract class MonoBehaviourData<T1, T2, T3> : MonoBehaviour
    {
        [Expandable]
        [SerializeField] protected T1 _data1;
        [Expandable]
        [SerializeField] protected T2 _data2;
        [Expandable]
        [SerializeField] protected T3 _data3;

        public T1 Data1
        {
            get => _data1;
            set => _data1 = value;
        }

        public T2 Data2
        {
            get => _data2;
            set => _data2 = value;
        }

        public T3 Data3
        {
            get => _data3;
            set => _data3 = value;
        }
    }

    public abstract class MonoBehaviourData<T1, T2, T3, T4> : MonoBehaviour
    {
        [Expandable]
        [SerializeField] protected T1 _data1;
        [Expandable]
        [SerializeField] protected T2 _data2;
        [Expandable]
        [SerializeField] protected T3 _data3;
        [Expandable]
        [SerializeField] protected T4 _data4;

        public T1 Data1
        {
            get => _data1;
            set => _data1 = value;
        }

        public T2 Data2
        {
            get => _data2;
            set => _data2 = value;
        }

        public T3 Data3
        {
            get => _data3;
            set => _data3 = value;
        }

        public T4 Data4
        {
            get => _data4;
            set => _data4 = value;
        }
    }
}
