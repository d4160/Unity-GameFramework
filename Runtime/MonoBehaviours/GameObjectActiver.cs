namespace d4160.Core
{
    using UnityEngine;
    using d4160.Systems.Common;

    public class GameObjectActiver : MonoBehaviour//, IActiver
    {
        public GameObject[] gameObjectsToActive;
        public bool setActive;
        public UnityInitMethodWithManual activationMethod;

        private void Awake()
        {
            if (activationMethod == UnityInitMethodWithManual.Awake)
                SetObjectsActive(setActive);
        }

        private void OnEnable()
        {
            if (activationMethod == UnityInitMethodWithManual.OnEnable)
                SetObjectsActive(setActive);
        }

        private void Start() 
        {
            if (activationMethod == UnityInitMethodWithManual.Start)
                SetObjectsActive(setActive);
        }

        public void SetObjectsActive(bool active)
        {
            for (int i = 0; i < gameObjectsToActive.Length; i++)
            {
                gameObjectsToActive[i].SetActive(setActive);
            }
        }
    }
}