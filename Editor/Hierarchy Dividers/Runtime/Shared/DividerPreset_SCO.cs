using System;
using System.Collections.Generic;
using UnityEngine;

namespace ManasparkAssets
{
    namespace HierarchyDividers
    {
        [CreateAssetMenu(fileName = "DividerPreset", menuName = "Manaspark/Hierarchy Dividers/Pro Template")]
        public class DividerPreset_SCO : ScriptableObject
        {
            [SerializeField]
            [HideInInspector] public List<Divider> Dividers = new List<Divider>();

            [Serializable]
            public class Divider
            {
                public string Name;
                [HideInInspector] public int ID;
                public string PreSuffix = "-";
                public int PreSuffixCount = 3;
                public string Label;

                public bool BoldFont, ItalicFont, ColoredFont, ColoredBG;
                public bool HasOverrideColor_leftSprite, HasOverrideColor_rightSprite;
                public Color FontColor, BGColor;
                public Color OverrideColor_leftSprite, OverrideColor_rightSprite;
                
                public bool HasSprite_left, HasSprite_right;
                public Sprite LeftSprite, RightSprite;
                public float LeftScale = 1f;
                public float RightScale = 1f;
                
#if UNITY_EDITOR
                public bool foldOut;
#endif

                public void SetLabel()
                {
                    if(Name == "")
                        return;
                   
                    Label = string.Empty;
                    for (int i = 0; i < PreSuffixCount; i++)
                    {
                        Label += PreSuffix;
                    }

                    Label += " " + Name + " ";
                    
                    for (int i = 0; i < PreSuffixCount; i++)
                    {
                        Label += PreSuffix;
                    }
                }
            }
        }

        public interface IHierarchyDivider
        {
            string DividerLabel { get; }
            
            Transform GetTransform { get; }
        }
    }
}
