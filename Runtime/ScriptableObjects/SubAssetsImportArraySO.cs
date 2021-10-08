using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.ScriptableObjects
{
    [CreateAssetMenu(menuName = "d4160/SubAssets/Import Array")]
    public class SubAssetsImportArraySO : ScriptableObject
    {
        [SerializeField] private List<ImportItem> _items;

        public List<ImportItem> Items { get => _items; set => _items = value; }
    }

    [System.Serializable]
    public struct ImportItem
    {
        public int index;
        public ScriptableObject sObject;
    }
}