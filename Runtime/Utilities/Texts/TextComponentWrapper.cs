using UnityEngine;
using TMPro;
using d4160.UnityUtils;

namespace d4160.Texts
{
    /// <summary>
    /// Wrapper component for TextMesh and TextMeshPro
    /// Available properties: Text and Color
    /// </summary>
    public sealed class TextComponentWrapper : MonoBehaviour
    {
        private TextMesh _textMesh;
        private TextMeshPro _tmpPro;

        public TextMesh TMesh => _textMesh;
        public TextMeshPro TMPro => _tmpPro;

        public string Text
        {
            get => _tmpPro ? _tmpPro.text : _textMesh ? _textMesh.text : string.Empty;
            set
            {
                if (_tmpPro) _tmpPro.text = value;
                else if (_textMesh) _textMesh.text = value;
            }
        }

        public Color Color
        {
            get => _tmpPro ? _tmpPro.color : _textMesh ? _textMesh.color : Color.black;
            set
            {
                if (_tmpPro) _tmpPro.color = value;
                else if (_textMesh) _textMesh.color = value;
            }
        }

        private void Awake()
        {
            _tmpPro = this.GetComponent<TextMeshPro>(true, true);

            if (!_tmpPro)
                _textMesh = this.GetComponent<TextMesh>(true, true);
        }
    }
}