using System.Collections.Generic;
using d4160.Singleton;
using UnityEngine;

namespace d4160.Loops
{
    public sealed class LateUpdateManager : Singleton<LateUpdateManager>
    {
        private static List<ILateUpdateObject> _updates = new List<ILateUpdateObject>();

        public static void RegisterObject(ILateUpdateObject updateObj)
        {
            _updates.Add(updateObj);
        }

        public static void UnregisterObject(ILateUpdateObject updateObj)
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