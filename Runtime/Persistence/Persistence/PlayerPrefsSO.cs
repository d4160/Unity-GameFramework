using System.Collections.Generic;
using UnityEngine;

namespace d4160.Persistence
{
    [CreateAssetMenu(menuName = "d4160/Persistence/PlayerPrefs")]
    public class PlayerPrefsSO : ScriptableObject, IPersistenceService
    {
        [SerializeField] private List<VariableSOKey> _storeVariables;

        private static PlayerPrefsService _service = PlayerPrefsService.Instance;

        public void Save()
        {
            _service.StoreVariables = _storeVariables;
            _service.Save();
        }

        public void Load()
        {
            _service.StoreVariables = _storeVariables;
            _service.Load();
        }
    }
}
