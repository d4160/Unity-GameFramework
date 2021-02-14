using UnityEngine;
using UnityEngine.UI;
using TMPro;
using d4160.UnityUtils;

namespace d4160.Texts
{
    /// <summary>
    /// Wrapper component for UI Text and TextMeshProUGUI
    /// Available properties: Text and Color
    /// </summary>
    public sealed class TextUIComponentWrapper : MonoBehaviour
    {
        private Text _text;
        private TextMesh _textMesh;
        private TextMeshProUGUI _tmProUGUI;
        private TextMeshPro _tmpPro;

        public Text UText => _text;
        public TextMeshProUGUI TMProUText => _tmProUGUI;
        public TextMesh TMesh => _textMesh;
        public TextMeshPro TMPro => _tmpPro;
        
        public string Text
        {
            get => _tmProUGUI ? _tmProUGUI.text : _text ? _text.text : string.Empty;
            set {
                if(_tmProUGUI) _tmProUGUI.text = value;
                else if(_text) _text.text = value;
            }
        }
        
        public Color Color
        {
            get => _tmProUGUI ? _tmProUGUI.color : _text ? _text.color : Color.black;
            set {
                if(_tmProUGUI) _tmProUGUI.color = value;
                else if(_text) _text.color = value;
            }
        }

        private void Awake()
        {
            _tmProUGUI = this.GetComponent<TextMeshProUGUI>(true, true);

            if (!_tmProUGUI)
                _text = this.GetComponent<Text>(true, true);
        }
    }
}