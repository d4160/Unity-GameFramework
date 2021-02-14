using System.Collections.Generic;
using d4160.Singleton;
using UnityEngine;

namespace d4160.Loops
{
    public sealed class LateUpdateManager : Singleton<LateUpdateManager>
    {
        private static List<ILaterUpdateInstance> _updates = new List<ILaterUpdateInstance>();

        public static void AddInstance(ILaterUpdateInstance instance)
        {
            _updates.Add(instance);
        }

        public static void RemoveInstance(ILaterUpdateInstance instance)
        {
            _updates.Remove(instance);
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            for (var i = 0; i < _updates.Count; i++)
            {
                _updates[i].OnUpdate(dt);
            }
        }
    }
}