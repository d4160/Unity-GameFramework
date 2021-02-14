using System.Collections.Generic;
using d4160.Singleton;
using UnityEngine;

namespace d4160.Loops
{
    public sealed class FixedUpdateManager : Singleton<FixedUpdateManager>
    {
        private static List<IFixedUpdateInstance> _updates = new List<IFixedUpdateInstance>();

        public static void AddInstance(IFixedUpdateInstance instance)
        {
            _updates.Add(instance);
        }

        public static void RemoveInstance(IFixedUpdateInstance instance)
        {
            _updates.Remove(instance);
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;
            for (var i = 0; i < _updates.Count; i++)
            {
                _updates[i].OnUpdate(dt);
            }
        }
    }
}