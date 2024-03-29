
using d4160.Core;
using d4160.Variables;
using System;
using System.Collections.Generic;
using Sabresaurus.PlayerPrefsUtilities;
using UnityEngine;
using d4160.Collections;
#if ENABLE_NAUGHTY_ATTRIBUTES
using NaughtyAttributes;
#endif

namespace d4160.Persistence
{
    public class PlayerPrefsService : IPersistenceService
    {
        public LogLevelType LogLevel { get; set; } = LogLevelType.Debug;

        public static PlayerPrefsService Instance => _instance ??= new PlayerPrefsService();
        private static PlayerPrefsService _instance;

        public List<VariableSOKey> StoreVariables { get; set; }

        public void Load()
        {
            for (int i = 0; i < StoreVariables.Count; i++)
            {
                var storeVar = StoreVariables[i];
                switch (storeVar.variableSO.RawValue)
                {
                    case int val:
                        if (storeVar.encrypt)
                        {
                            storeVar.variableSO.RawValue = PlayerPrefsUtility.GetEncryptedInt(storeVar.key, val);
                        }
                        else
                        {
                            storeVar.variableSO.RawValue = PlayerPrefs.GetInt(storeVar.key, val);
                        }
                        break;
                    case float val:
                        if (storeVar.encrypt)
                        {
                            storeVar.variableSO.RawValue = PlayerPrefsUtility.GetEncryptedFloat(storeVar.key, val);
                        }
                        else
                        {
                            storeVar.variableSO.RawValue = PlayerPrefs.GetFloat(storeVar.key, val);
                        }
                        break;
                    case bool val:
                        if (storeVar.encrypt)
                        {
                            storeVar.variableSO.RawValue = PlayerPrefsUtility.GetEncryptedBool(storeVar.key, val);
                        }
                        else
                        {
                            storeVar.variableSO.RawValue = PlayerPrefsUtility.GetBool(storeVar.key, val);
                        }
                        break;
                    case string val:
                        if (storeVar.encrypt)
                        {
                            storeVar.variableSO.RawValue = PlayerPrefsUtility.GetEncryptedString(storeVar.key, val);
                        }
                        else
                        {
                            storeVar.variableSO.RawValue = PlayerPrefs.GetString(storeVar.key, val);
                        }
                        break;
                    default:
                        //if (storeVar.encrypt)
                        //{
                        //    storeVar.variableSO.RawValue = PlayerPrefsUtility.GetEncryptedString(storeVar.key, storeVar.variableSO.RawValue.ToString());
                        //}
                        //else
                        //{
                        //    //Debug.Log(storeVar.key);
                        //    storeVar.variableSO.RawValue = PlayerPrefs.GetString(storeVar.key, storeVar.variableSO.RawValue?.ToString());
                        //}
                        break;
                }
            }
        }

        public void Save()
        {
            for (int i = 0; i < StoreVariables.Count; i++)
            {
                var storeVar = StoreVariables[i];
                Save(storeVar);
            }
        }

        public void Save(int index)
        {
            if (StoreVariables.IsValidIndex(index))
            {
                Save(StoreVariables[index]);
            }
        }

        public void Save(VariableSOKey storeVar)
        {
            switch (storeVar.variableSO.RawValue)
            {
                case int val:
                    if (storeVar.encrypt)
                    {
                        PlayerPrefsUtility.SetEncryptedInt(storeVar.key, val);
                    }
                    else
                    {
                        PlayerPrefs.SetInt(storeVar.key, val);
                    }
                    break;
                case float val:
                    if (storeVar.encrypt)
                    {
                        PlayerPrefsUtility.SetEncryptedFloat(storeVar.key, val);
                    }
                    else
                    {
                        PlayerPrefs.SetFloat(storeVar.key, val);
                    }
                    break;
                case bool val:
                    if (storeVar.encrypt)
                    {
                        PlayerPrefsUtility.SetEncryptedBool(storeVar.key, val);
                    }
                    else
                    {
                        PlayerPrefsUtility.SetBool(storeVar.key, val);
                    }
                    break;
                case string val:
                    if (storeVar.encrypt)
                    {
                        PlayerPrefsUtility.SetEncryptedString(storeVar.key, val);
                    }
                    else
                    {
                        PlayerPrefs.SetString(storeVar.key, val);
                    }
                    break;
                default:
                    if (storeVar.encrypt)
                    {
                        PlayerPrefsUtility.SetEncryptedString(storeVar.key, storeVar.variableSO.RawValue.ToString());
                    }
                    else
                    {
                        PlayerPrefs.SetString(storeVar.key, storeVar.variableSO.RawValue.ToString());
                    }
                    break;
            }
        }

        public void Delete()
        {
            for (int i = 0; i < StoreVariables.Count; i++)
            {
                var storeVar = StoreVariables[i];
                string encryptedKey = storeVar.encrypt ? GetEncryptedKey(storeVar.key) : storeVar.key;
                PlayerPrefs.DeleteKey(encryptedKey);
                storeVar.ResetValue();
            }
        }

        public void Delete(int index)
        {
            if (StoreVariables.IsValidIndex(index))
            {
                string encryptedKey = StoreVariables[index].encrypt ? GetEncryptedKey(StoreVariables[index].key) : StoreVariables[index].key;
                PlayerPrefs.DeleteKey(encryptedKey);
                StoreVariables[index].ResetValue();
            }
        }

        private string GetEncryptedKey(string key)
        {
            return PlayerPrefsUtility.KEY_PREFIX + SimpleEncryption.EncryptString(key);
        }
    }

    [Serializable]
    public struct VariableSOKey
    {
        public string key;
        public bool encrypt;
#if ENABLE_NAUGHTY_ATTRIBUTES
        [Expandable]
#endif
        public VariableSOBase variableSO;

        public void ResetValue()
        {
            variableSO.ResetValue();
        }

        public T GetAs<T>() where T : VariableSOBase
        {
            return variableSO.GetAs<T>();
        }
    }
}