using d4160.Variables;
using System;
using UnityEngine;

namespace d4160.Collections
{
    [CreateAssetMenu(menuName = "d4160/Collections/Variable Library")]
    public class VariableLibrarySO : UnityLibrarySOBase<VariableSOBase>
    {
        public T2 GetValue<T1, T2>(T1 key)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] is IDictionaryItem<T1, T2> dic)
                {
                    if (dic.Key.Equals(key))
                    {
                        return dic.Value;
                    }
                }
            }

            return default;
        }

        public bool IsValidIndex(int index)
        {
            return index >= 0 && index < Items.Length;
        }
    }
}