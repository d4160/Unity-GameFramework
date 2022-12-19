using System.Collections.Generic;
using d4160.Singleton;
using UnityEngine;

namespace d4160.Loops
{
    public sealed class FixedUpdateManager : Singleton<FixedUpdateManager>
    {
        protected override bool DontDestroyOnLoadProp => true;
        protected override bool HideInHierarchy => true;

        private static readonly List<IFixedUpdateObject> _updates = new();

        public static void AddListener(IFixedUpdateObject updateObj)
        {
            _updates.Add(updateObj);
        }

        public static void RemoveListener(IFixedUpdateObject updateObj)
        {
            _updates.Remove(updateObj);
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            for (var i = 0; i < _updates.Count; i++)
            {
                _updates[i].OnFixedUpdate(dt);
            }
        }
    }
}