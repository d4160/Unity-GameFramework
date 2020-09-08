using UnityEngine;
using UnityExtensions;

namespace d4160.Core.MonoBehaviours
{
    public class PrefabManagerBase<T1, T2> : Singleton<T1> where T1 : MonoBehaviour where T2 : MonoBehaviour
    {
        [SerializeField, InspectInline] protected T2 m_instancedMain;

        public T2 InstancedMain
        {
            get
            {
                if (!m_instancedMain)
                {
                    m_instancedMain = FindObjectOfType<T2>();
                }

                return m_instancedMain;
            }
        }

        public virtual void SetInstanced(T2 instanced)
        {
            if (m_instancedMain && m_instancedMain != instanced)
                Destroy(m_instancedMain.gameObject);

            m_instancedMain = instanced;
        }
    }
}