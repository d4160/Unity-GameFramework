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
    [CreateAssetMenu(menuName = "d4160/UGS/CloudSave/Load")]
    public class LoadSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] VariableLibrarySO _variablesToLoad;

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

            Dictionary<string, string> data = await CloudSaveService.Instance.Data.LoadAsync(keys);

            foreach (var item in data)
            {
                for (int i = 0; i < _variablesToLoad.Count; i++)
                {
                    if (_variablesToLoad[i] is IDictionaryItem<string> dicItem)
                    {
                        if (item.Key == dicItem.Key)
                        {
                            dicItem.ParseInnerValue(item.Value);
                        }
                    }
                }
            }
        }
    }
}