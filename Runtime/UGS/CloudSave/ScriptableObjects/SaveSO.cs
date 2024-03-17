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

        public void SetInnerValue<T1, T2>(int index, T2 value) where T1 : VariableSOBase<T2>
        {
            if (_variablesToSave.IsValidIndex(index))
            {
                if (_variablesToSave[index] is InnerVariableSOBase<T1, T2> innerVar)
                {
                    innerVar.InnerValue = value;
                }
            }
        }

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

        public T2 GetInnerValue<T1, T2>(int index) where T1 : VariableSOBase<T2>
        {
            if (_variablesToSave.IsValidIndex(index))
            {
                if (_variablesToSave[index] is InnerVariableSOBase<T1, T2> innerVar)
                {
                    return innerVar.InnerValue;
                }
            }

            return default;
        }

        public string GetInnerStringValue(int index)
        {
            if (_variablesToSave.IsValidIndex(index))
            {
                if (_variablesToSave[index] is IInnerVariable innerVar)
                {
                    return innerVar.InnerStringValue;
                }
            }

            return default;
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