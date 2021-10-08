using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.ScriptableObjects
{
    [CreateAssetMenu(menuName = "d4160/SubAssets/Export Array")]
    public class SubAssetsExportArraySO : ScriptableObject
    {
        [SerializeField] private List<int> _items;

        public List<int> Items { get => _items; set => _items = value; }
    }
}