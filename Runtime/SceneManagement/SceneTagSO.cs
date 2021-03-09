using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace d4160.SceneManagement
{
    [CreateAssetMenu(menuName = "d4160/SceneManagement/Scene Tag")]
    public class SceneTagSO : ScriptableObject
    {
        public string _name;
        [TextArea] public string description;
        public ScenePackLoadType loadType;
        
        private int _hash;

        public int Hash
        {
            get
            {
                if (!Application.isPlaying)
                {
                    _hash = Animator.StringToHash(_name);
                }
                else
                {
                    if (_hash == 0)
                    {
                        _hash = Animator.StringToHash(_name);
                    }
                }

                return _hash;
            }
        }
    }

    public enum ScenePackLoadType
    {
        Normal,
        Persistent
    }
}