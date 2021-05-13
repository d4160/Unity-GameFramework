using System.Collections.Generic;
using d4160.Singleton;
using UnityEngine;

namespace d4160.Loops
{
    public sealed class UpdateManager : Singleton<UpdateManager>
    {
        private static List<IUpdateObject> _updates = new List<IUpdateObject>();

        public static void RegisterObject(IUpdateObject updateObj)
        {
            _updates.Add(updateObj);

            if(!UpdateManager.Instanced)
            {
                UpdateManager.InstanceSingleton();
            }
        }

        public static void UnregisterObject(IUpdateObject updateObj)
        {
            _updates.Remove(updateObj);
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