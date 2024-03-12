using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudSave;
using d4160.Variables;
using System.Threading.Tasks;
using d4160.Collections;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.UGS.CloudSave
{
    [CreateAssetMenu(menuName = "d4160/UGS/CloudSave/ForceSave")]
    public class SaveSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] VariableLibrarySO _variablesToSave;

        public void SetInnerStringValue(int index, string value)
        {
            if (_variablesToSave.IsValidIndex(index))
            {
                if (_variablesToSave[index] is IDictionaryItem<string> dic)
                {
                    dic.InnerStringValue = value;
                }
            }
        }

        public async void Save()
        {
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            var data = new Dictionary<string, object>();
            //string log = string.Empty;

            for (int i = 0; i < _variablesToSave.Count; i++)
            {
                if (_variablesToSave[i] is IDictionaryItem<string> dicItem)
                {
                    data.Add(dicItem.Key, dicItem.InnerRawValue);
                    //log += $"[i:{i}] Key: {dicItem.Key}, RawValue: {dicItem.InnerRawValue} \n";
                }
            }

            //Debug.Log(log);

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }
    }
}