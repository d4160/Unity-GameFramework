using System.Collections.Generic;
using d4160.Singleton;
using UnityEngine;

namespace d4160.Loops
{
    public sealed class LateUpdateManager : Singleton<LateUpdateManager>
    {
        protected override bool DontDestroyOnLoadProp => true;
        protected override bool HideInHierarchy => true;

        private static readonly List<ILateUpdateObject> _updates = new();

        public static void AddListener(ILateUpdateObject updateObj)
        {
            _updates.Add(updateObj);
        }

        public static void RemoveListener(ILateUpdateObject updateObj)
        {
            _updates.Remove(updateObj);
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            for (var i = 0; i < _updates.Count; i++)
            {
                _updates[i].OnLateUpdate(dt);
            }
        }
    }
}