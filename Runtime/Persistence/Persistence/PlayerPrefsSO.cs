using System.Collections.Generic;
using UnityEngine;

namespace d4160.Persistence
{
    [CreateAssetMenu(menuName = "d4160/Persistence/PlayerPrefs")]
    public class PlayerPrefsSO : ScriptableObject, IPersistenceService
    {
        [SerializeField] private List<VariableSOKey> _storeVariables;

        private static PlayerPrefsService _service = PlayerPrefsService.Instance;

        public List<VariableSOKey> StoreVariables => _storeVariables;

        [ContextMenu("ResetValues")]
        private void ResetValues()
        {
            for (int i = 0; i < _storeVariables.Count; i++)
            {
                _storeVariables[i].ResetValue();
            }
        }

        public void Save()
        {
            _service.StoreVariables = _storeVariables;
            _service.Save();
        }

        public void Save(int index)
        {
            _service.StoreVariables = _storeVariables;
            _service.Save(index);
        }

        public void Load()
        {
            _service.StoreVariables = _storeVariables;
            _service.Load();
        }

        public void Delete()
        {
            _service.StoreVariables = _storeVariables;
            _service.Delete();
        }

        public void Delete(int index)
        {
            _service.StoreVariables = _storeVariables;
            _service.Delete(index);
        }
    }
}
