using System.Collections;
using System.Collections.Generic;
using System.Linq;
using d4160.Collections;
using UnityEngine;

namespace d4160.UGS.Multiplay
{
    [CreateAssetMenu(menuName = "d4160/UGS/Multiplay/Libs/BuildConfiguration")]
    public class BuildConfigurationLibrarySO : LibrarySOBase<BuildConfigurationData>
    {
        public int GetEnvironmentIndex(int buildConfigId)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].id == buildConfigId)
                {
                    return _items[i].enviroIndex;
                }
            }

            return -1;
        }

        public int GetBuildConfigurationId(int enviroIndex)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i].enviroIndex == enviroIndex)
                {
                    return _items[i].id;
                }
            }

            return -1;
        }
    }

    [System.Serializable]
    public struct BuildConfigurationData
    {
        public string name;
        public int id;
        public int enviroIndex;
    }
}