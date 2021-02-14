using System.Collections.Generic;
using d4160.Singleton;
using UnityEngine;

namespace d4160.Loops
{
    public sealed class UpdateManager : Singleton<UpdateManager>
    {
        private static List<IUpdateInstance> _updates = new List<IUpdateInstance>();

        public static void AddInstance(IUpdateInstance instance)
        {
            _updates.Add(instance);
        }

        public static void RemoveInstance(IUpdateInstance instance)
        {
            _updates.Remove(instance);
        }

        private void Update()
        {
            float dt = Time.deltaTime;
            for (var i = 0; i < _updates.Count; i++)
            {
                _updates[i].OnUpdate(dt);
            }
        }
    }
}