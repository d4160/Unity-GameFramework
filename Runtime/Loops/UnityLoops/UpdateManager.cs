using System.Collections.Generic;
using d4160.Singleton;
using UnityEngine;

namespace d4160.Loops
{
    public sealed class UpdateManager : Singleton<UpdateManager>
    {
        protected override bool DontDestroyOnLoadProp => true;
        protected override bool HideInHierarchy => true;

        private static readonly List<IUpdateObject> _updates = new();

        public static void AddListener(IUpdateObject updateObj)
        {
            _updates.Add(updateObj);

            if(!Instanced)
            {
                InstanceSingleton();
            }
        }

        public static void RemoveListener(IUpdateObject updateObj)
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