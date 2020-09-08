#if UNITY_EDITOR
using System;
using UnityEngine;

namespace ManasparkAssets
{
    namespace HierarchyDividers
    {
        [Serializable]
        public class HierarchyDividerValues
        {
            public string DividerID, Label;
            public Color fontColor, bgColor;
            public Color OverrideColor_leftSprite, OverrideColor_rightSprite;
            public bool boldFont, italicFont, hasFontColor, hasBgColor, hasLeftSprite, hasRightSprite;
            public Texture leftSprite, rightSprite;

            public HierarchyDividerValues(string text, Color fontcol, Color bgcol, Color _ovrdCol_left,
                                          Color _ovrdCol_right, bool bold, bool italic,
                                          bool hasFontCol, bool hasBgCol, bool hasLeft, bool hasRight,
                                          bool _hasOvrd_left, bool _hasOvrd_right,
                                          Sprite leftSprite, Sprite rightSprite, string id)
            {
                DividerID = id;
                Label = text;

                fontColor = fontcol;
                bgColor = bgcol;
                boldFont = bold;
                italicFont = italic;
                hasFontColor = hasFontCol;
                hasBgColor = hasBgCol;
                hasLeftSprite = hasLeft;
                hasRightSprite = hasRight;
                if (leftSprite != null)
                    this.leftSprite = leftSprite.texture;
                else
                    hasLeftSprite = false;
                if (rightSprite != null)
                    this.rightSprite = rightSprite.texture;
                else
                    hasRightSprite = false;

                OverrideColor_leftSprite = _ovrdCol_left;
                OverrideColor_rightSprite = _ovrdCol_right;
            }
        }

        public class HierarchyDivider : MonoBehaviour, IHierarchyDivider
        {
            [HideInInspector] public HierarchyDividerValues HDV;

            public void SetDivider(string text, Color fontcol, Color bgcol, Color _ovrdCol_left,
                                   Color _ovrdCol_right, bool bold, bool italic, bool hasFontCol, bool hasBgCol,
                                   bool hasLeft, bool hasRight, bool _hasOvrd_left, bool _hasOvrd_right,
                                   Sprite leftSprite, Sprite rightSprite)
            {
                string id = gameObject.GetInstanceID().ToString() + this.GetInstanceID().ToString();

                gameObject.name = text == "" ? "-- UNNAMED DIVIDER --" : text;
                gameObject.tag = "EditorOnly";

                HDV = new HierarchyDividerValues(text, fontcol, bgcol, _ovrdCol_left, _ovrdCol_right, bold, italic,
                                                 hasFontCol, hasBgCol, hasLeft, hasRight, _hasOvrd_left, _hasOvrd_right,
                                                 leftSprite, rightSprite, id);

                HierarchyDividerDrawing.Dividers.Add(HDV);
            }
            
            public string DividerLabel
            {
                get { return HDV != null ? HDV.Label : "";}
            }

            public Transform GetTransform
            {
                get { return transform; }
            }
        }
    }
}
#endif
