namespace d4160.Loops
{
    using UnityEngine;
    using System;

    public abstract class AnimatorLoop : MonoBehaviour
    {
        public delegate void AnimatorIKDelegate(int layerIndex);

        public static event AnimatorIKDelegate OnAnimatorIKEvent;
        public static event Action OnAnimatorMoveEvent;

        protected virtual void OnAnimatorIK(int layerIndex) 
        {
            OnAnimatorIKEvent?.Invoke(layerIndex);
        }

        protected virtual void OnAnimatorMove()
        {
            OnAnimatorMoveEvent?.Invoke();
        }
    }
}