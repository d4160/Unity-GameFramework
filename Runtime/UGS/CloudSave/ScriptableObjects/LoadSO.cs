using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudSave;
using d4160.Variables;
using System.Threading.Tasks;
using d4160.Collections;
using Unity.Services.CloudSave.Models;

#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.UGS.CloudSave
{
    [CreateAssetMenu(menuName = "d4160/UGS/CloudSave/Load")]
    public class LoadSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] VariableLibrarySO _variablesToLoad;

        public T2 GetInnerValue<T1, T2>(int index) where T1 : VariableSOBase<T2>
        {
            if (_variablesToLoad.IsValidIndex(index))
            {
                if (_variablesToLoad[index] is InnerVariableSOBase<T1, T2> innerVar)
                {
                    return innerVar.InnerValue;
                }
            }

            return default;
        }

        public string GetInnerStringValue(int index)
        {
            if (_variablesToLoad.IsValidIndex(index))
            {
                if (_variablesToLoad[index] is IInnerVariable innerVar)
                {
                    return innerVar.InnerStringValue;
                }
            }

            return default;
        }

        public void SetInnerValue<T1, T2>(int index, T2 value) where T1 : VariableSOBase<T2>
        {
            if (_variablesToLoad.IsValidIndex(index))
            {
                if (_variablesToLoad[index] is InnerVariableSOBase<T1, T2> innerVar)
                {
                    innerVar.InnerValue = value;
                }
            }
        }

        public void SetInnerStringValue(int index, string value)
        {
            if (_variablesToLoad.IsValidIndex(index))
            {
                if (_variablesToLoad[index] is IDictionaryItem<string> dic)
                {
                    dic.InnerStringValue = value;
                }
            }
        }

        public async void Load()
        {
            await LoadAsync();
        }

        public async Task LoadAsync()
        {
            var keys = new HashSet<string>();
            //string log = string.Empty;

            for (int i = 0; i < _variablesToLoad.Count; i++)
            {
                if (_variablesToLoad[i] is IDictionaryItem<string> dicItem)
                {
                    keys.Add(dicItem.Key);
                    //log += $"[i:{i}] Key: {dicItem.Key} \n";
                }
            }

            //Debug.Log(log);

            //Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(keys);

            try
            {

                Dictionary<string, Item> data = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                foreach (var item in data)
                {
                    for (int i = 0; i < _variablesToLoad.Count; i++)
                    {
                        if (_variablesToLoad[i] is IDictionaryItem<string> dicItem)
                        {
                            if (item.Key == dicItem.Key)
                            {
                                dicItem.ParseInnerValue(item.Value.Value.GetAs<string>());
                            }
                        }
                    }
                }

            }
            catch (System.Exception e)
            {

            }
        }
    }
}