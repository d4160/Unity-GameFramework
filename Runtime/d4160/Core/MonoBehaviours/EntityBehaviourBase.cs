namespace d4160.Core
{
    using UnityEngine;

    public abstract class EntityBehaviourBase : MonoBehaviour, IArchetype
    {
        [SerializeField] protected int m_id;

        public int ID { get => m_id; set => m_id = value; }
    }
}