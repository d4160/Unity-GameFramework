using System.Collections.Generic;
using d4160.Singleton;
using UnityEngine;

namespace d4160.Loops
{
    public sealed class FixedUpdateManager : Singleton<FixedUpdateManager>
    {
        private static List<IFixedUpdateObject> _updates = new List<IFixedUpdateObject>();

        public static void RegisterObject(IFixedUpdateObject updateObj)
        {
            _updates.Add(updateObj);
        }

        public static void UnregisterObject(IFixedUpdateObject updateObj)
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