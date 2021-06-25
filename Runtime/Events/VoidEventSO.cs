using System;
using NaughtyAttributes;
using UnityEngine;

namespace d4160.Events {
    [CreateAssetMenu (menuName = "d4160/Events/Void")]
    public class VoidEventSO : ScriptableObject {
        public event Action OnEvent;

        [Button]
        public void Invoke () {
            OnEvent?.Invoke ();
        }
    }
}