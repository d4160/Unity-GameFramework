using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.CloudSave;
using d4160.Variables;
using System.Threading.Tasks;
using d4160.Collections;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;


#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.UGS.CloudSave
{
    [CreateAssetMenu(menuName = "d4160/UGS/CloudSave/DynamicForceSave")]
    public class DynamicSaveSO : ScriptableObject
    {
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringStringVariableSO _dynamicPairListJson;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] private StringIntVariableSO _autoincrement;

        [SerializeField] private string _prefix;

#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        [SerializeField] StringVariableSO _jsonToSave;

        public DynamicPairList DynamicPairList { get; private set; } = new();

        private string NewSlotKey => $"{_prefix}{_autoincrement.Value + 1}";

        public void SetJsonToSave(string json)
        {
            _jsonToSave.Value = json;
        }

        public async void LoadDynamicData()
        {
            await LoadDynamicDataAsync();
        }

        public async Task LoadDynamicDataAsync()
        {
            var keys = new HashSet<string>
            {
                _dynamicPairListJson.Key,
                _autoincrement.Key
            };

            Dictionary<string, Item> data = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            foreach (var item in data)
            {
                if (item.Key == _dynamicPairListJson.Key)
                {
                    _dynamicPairListJson.Value = item.Value.Value.GetAsString();

                    DynamicPairList = DynamicPairList.FromJson(_dynamicPairListJson.Value);
                }
                else if (item.Key == _autoincrement.Key)
                {
                    _autoincrement.Value = int.Parse(item.Value.Value.GetAsString());
                }
            }
        }

        public async Task<Dictionary<string, Item>> ReadAllDynamicDataAsync()
        {
            if (DynamicPairList.Count > 0)
            {
                HashSet<string> keys = new();
                for (int i = 0; i < DynamicPairList.Count; i++)
                {
                    keys.Add(DynamicPairList[i].slotKey);
                }

                return await CloudSaveService.Instance.Data.Player.LoadAsync(keys);
            }
            return null;
        }

        public async void CreateDynamicPair(string id, bool isOwner = true, bool updateIfFound = true)
        {
            await CreateDynamicPairAsync(id, isOwner, updateIfFound);
        }

        public async Task<bool> CreateDynamicPairAsync(string id, bool isOwner = true, bool updateIfFound = true)
        {
            bool found = false;
            for (int i = 0; i < DynamicPairList.Count; i++)
            {
                if (DynamicPairList[i].id == id)
                {
                    found = true;
                    Debug.Log("Id already saved.");
                    if (updateIfFound) await UpdateDynamicPairAsync(id, isOwner);
                    break;
                }
            }

            if (found) return false;

            var data = new Dictionary<string, object>();
            //string log = string.Empty;

            var dynamicPair = new DynamicPair()
            {
                slotKey = NewSlotKey,
                id = id,
                isOwner = isOwner
            };

            DynamicPairList.Add(dynamicPair);

            _autoincrement.Value++;
            _dynamicPairListJson.Value = DynamicPairList.ToJson();

            data.Add(dynamicPair.slotKey, _jsonToSave.Value);
            data.Add(_autoincrement.Key, _autoincrement.Value);
            data.Add(_dynamicPairListJson.Key, _dynamicPairListJson.Value);

            //Debug.Log(log);

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);

            return true;
        }

        public async Task<string> ReadDynamicPairAsync(string id)
        {
            bool found = false;
            DynamicPair dynamicPair = default;
            for (int i = 0; i < DynamicPairList.Count; i++)
            {
                if (DynamicPairList[i].id == id)
                {
                    found = true;
                    dynamicPair = DynamicPairList[i];
                    break;
                }
            }

            if (!found) return null;

            var keys = new HashSet<string>
            {
                dynamicPair.slotKey
            };

            Dictionary<string, Item> data = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (data.ContainsKey(dynamicPair.slotKey))
            {
                return data[dynamicPair.slotKey].Value.GetAsString();
            }

            return null;
        }

        public async void UpdateDynamicPair(string id, bool isOwner = true)
        {
            await UpdateDynamicPairAsync(id, isOwner);
        }

        public async Task UpdateDynamicPairAsync(string id, bool isOwner = true)
        {
            bool found = false;
            DynamicPair dynamicPair = default;
            int index = -1;
            for (int i = 0; i < DynamicPairList.Count; i++)
            {
                if (DynamicPairList[i].id == id)
                {
                    found = true;
                    dynamicPair = DynamicPairList[i];
                    index = i;
                    break;
                }
            }

            if (!found) return;

            var data = new Dictionary<string, object>();
            //string log = string.Empty;

            dynamicPair.isOwner = isOwner;
            DynamicPairList[index] = dynamicPair;

            _dynamicPairListJson.Value = DynamicPairList.ToJson();

            data.Add(dynamicPair.slotKey, _jsonToSave.Value);
            data.Add(_dynamicPairListJson.Key, _dynamicPairListJson.Value);

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }

        public async void DeleteDynamicPair(string id)
        {
            await DeleteDynamicPairAsync(id);
        }

        public async Task DeleteDynamicPairAsync(string id)
        {
            bool found = false;
            DynamicPair dynamicPair = default;
            for (int i = 0; i < DynamicPairList.Count; i++)
            {
                if (DynamicPairList[i].id == id)
                {
                    found = true;
                    dynamicPair = DynamicPairList[i];

                    DynamicPairList.RemoveAt(i);

                    break;
                }
            }

            if (!found) return;

            _dynamicPairListJson.Value = DynamicPairList.ToJson();

            var data = new Dictionary<string, object>
            {
                { _dynamicPairListJson.Key, _dynamicPairListJson.Value }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            await CloudSaveService.Instance.Data.Player.DeleteAsync(dynamicPair.slotKey);
        }

        public async void DeleteAllDynamicData()
        {
            await DeleteAllDynamicDataAsync();
        }

        public async Task DeleteAllDynamicDataAsync()
        {
            if (DynamicPairList.Count > 0)
            {
                for (int i = 0; i < DynamicPairList.Count; i++)
                {
                    await CloudSaveService.Instance.Data.Player.DeleteAsync(DynamicPairList[i].slotKey);
                }

                DynamicPairList = new DynamicPairList();
                _autoincrement.Value = 0;
                _dynamicPairListJson.Value = string.Empty;
                _jsonToSave.Value = string.Empty;

                await CloudSaveService.Instance.Data.Player.DeleteAsync(_dynamicPairListJson.Key);
                await CloudSaveService.Instance.Data.Player.DeleteAsync(_autoincrement.Key);
            }
        }
    }

    [System.Serializable]
    public struct DynamicPair
    {
        public string slotKey;
        public string id;
        public bool isOwner;
    }

    [System.Serializable]
    public class DynamicPairList
    {
        public List<DynamicPair> dynamicPairs = new();

        public int Count => dynamicPairs.Count;

        public void Add(DynamicPair pair) { dynamicPairs.Add(pair); }
        public void RemoveAt(int i) { dynamicPairs.RemoveAt(i); }

        public DynamicPair this[int index]
        {
            get => dynamicPairs[index];
            set => dynamicPairs[index] = value;
        }

        public static DynamicPairList FromJson(string json)
        {
            return JsonUtility.FromJson<DynamicPairList>(json);
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
}