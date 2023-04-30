using d4160.Collections;
using System.Collections.Generic;
using UMA;
using UnityEngine;
using UMA.CharacterSystem;

namespace d4160.UMA
{
    [CreateAssetMenu(menuName = "d4160/UMA/Collections/Refs Group")]
    public class UmaRefsGroupLibrarySO : LibrarySOBase<UMARefGroup>
    {
        public void SetDNA(int groupIdx, Dictionary<string, DnaSetter> dna, float value, DynamicCharacterAvatar avatar = null)
        {
            var group = this[groupIdx];
            if (group == null) return;

            group.SetDNA(dna, value, avatar);
        }

        public float GetDNA(int groupIdx, Dictionary<string, DnaSetter> dna)
        {
            var group = this[groupIdx];
            if (group == null) return 0;

            return group.GetDNA(dna);
        }

        public void SetColor(int groupIdx, DynamicCharacterAvatar avatar, Color color)
        {
            var group = this[groupIdx];
            if (group == null) return;

            try
            {
                group.SetColor(avatar, color);
            }
            catch { }
        }

        public Color GetColor(int groupIdx, DynamicCharacterAvatar avatar)
        {
            var group = this[groupIdx];
            if (group == null) return Color.white;

            return group.GetColor(avatar);
        }
    }

    [System.Serializable]
    public class UMARefGroup
    {
        [SerializeField] private string _groupName;
        [SerializeField] private string[] _refs;

        public void SetDNA(Dictionary<string, DnaSetter> dna, float value, DynamicCharacterAvatar avatar = null)
        {
            for (int i = 0; i < _refs.Length; i++)
            {
                dna[_refs[i]].Set(value);
            }

            if (avatar) avatar.BuildCharacter();
        }

        public float GetDNA(Dictionary<string, DnaSetter> dna) 
        {
            if (_refs.Length == 0) return 0;

            return dna[_refs[0]].Get();
        }

        public void SetColor(DynamicCharacterAvatar avatar, Color color)
        {
            for (int i = 0; i < _refs.Length; i++)
            {
                avatar.SetColor(_refs[i], color);
            }

            avatar.UpdateColors(true);
        }

        public Color GetColor(DynamicCharacterAvatar avatar)
        {
            if (_refs.Length == 0) return Color.white;

            try
            {
                return avatar.GetColor(_refs[0]).color;
            }
            catch
            {
                return Color.white;
            }
        }
    }
}