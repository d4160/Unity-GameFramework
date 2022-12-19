using System.Collections;
using UnityEngine;

namespace d4160.MonoBehaviours
{
    public abstract class DelayedStartBehaviourBase : MonoBehaviour
    {
        [SerializeField, Range(0, 31f)] private float _delay;

        protected virtual IEnumerator Start()
        {
            if (_delay <= 0)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(_delay);
            }

            OnStart();
        }

        protected abstract void OnStart();
    }
}