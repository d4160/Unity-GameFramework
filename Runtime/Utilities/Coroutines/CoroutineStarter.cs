using d4160.Singleton;
using System;
using System.Collections;
using UnityEngine;

namespace d4160.Coroutines
{
    /// <summary>
    /// MonoBehaviour when need to start a Co-routine outside a MonoBehaviour
    /// </summary>
    public class CoroutineStarter : Singleton<CoroutineStarter>
    {
        protected override bool DontDestroyOnLoadProp => true;
        protected override bool HideInHierarchy => true;

        private WaitForEndOfFrame _waitForEndOfFrame;

        public void WaitForEndOfFrameAndExecute(Action callback)
        {
            StartCoroutine(WaitAndExecuteCo(1, callback));
        }

        public void WaitForFramesAndExecute(int frames, Action callback)
        {
            StartCoroutine(WaitAndExecuteCo(frames, callback));
        }

        public void WaitAndExecute(float wait, Action callback)
        {
            StartCoroutine(WaitAndExecuteCo(wait, callback));
        }

        private IEnumerator WaitAndExecuteCo(float wait, Action callback)
        {
            if (wait <= 0)
            {
                yield return _waitForEndOfFrame ??= new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForSeconds(wait);
            }

            callback?.Invoke();
        }

        private IEnumerator WaitAndExecuteCo(int frames, Action callback)
        {
            while (frames > 0)
            {
                yield return _waitForEndOfFrame ??= new WaitForEndOfFrame();
                frames--;
            }

            callback?.Invoke();
        }
    }
}