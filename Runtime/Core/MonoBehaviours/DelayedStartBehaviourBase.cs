using System.Collections;
using UnityEngine;
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3 || ODIN_INSPECTOR_3_1
using Sirenix.OdinInspector;
#endif

namespace d4160.MonoBehaviours
{
    public abstract class DelayedStartBehaviourBase : MonoBehaviour
    {
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3 || ODIN_INSPECTOR_3_1
        [HideIf("_setOnAwake")]
#endif
        [SerializeField] protected bool _waitEndOfFrame;
#if ODIN_INSPECTOR || ODIN_INSPECTOR_3 || ODIN_INSPECTOR_3_1
        [HideIf("_ShowDelayProperty")]
#endif
        [SerializeField, Range(0, 31f)] protected float _delayInSeconds;

#if ODIN_INSPECTOR || ODIN_INSPECTOR_3 || ODIN_INSPECTOR_3_1
        protected virtual bool _ShowDelayProperty => _waitEndOfFrame;
#endif

        protected virtual IEnumerator Start()
        {
            if (!_waitEndOfFrame)
            {
                if (_delayInSeconds <= 0)
                {
                    yield return null;
                }
                else
                {
                    yield return new WaitForSeconds(_delayInSeconds);
                }
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }

            OnStart();
        }

        protected abstract void OnStart();
    }
}