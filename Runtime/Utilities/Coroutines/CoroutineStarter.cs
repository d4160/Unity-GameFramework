﻿using d4160.Singleton;
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

        public void WaitAndExecute(float wait, Action callback)
        {
            StartCoroutine(WaitAndExecuteCo(wait, callback));
        }

        private IEnumerator WaitAndExecuteCo(float wait, Action callback)
        {
            if (wait > 0)
            {
                yield return new WaitForSeconds(wait);
            }

            callback?.Invoke();
        }
    }
}