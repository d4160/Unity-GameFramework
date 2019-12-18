namespace d4160.Core
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class TextComponentWrapper : MonoBehaviour
    {
        protected Text m_UGUIText;
        protected TextMeshProUGUI m_tmpUGUIText;

        public Text UGUIText => m_UGUIText;
        public TextMeshProUGUI TMPUGUIText => m_tmpUGUIText;

        public virtual string Text
        {
            get {
                return m_UGUIText ? m_UGUIText.text : m_tmpUGUIText?.text;
            }
            set {
                if(m_UGUIText) m_UGUIText.text = value;
                else if(m_tmpUGUIText) m_tmpUGUIText.text = value;
            }
        }

        protected virtual void Awake()
        {
            m_UGUIText = this.GetComponent<Text>(true, true);

            if (!m_UGUIText)
                m_tmpUGUIText = this.GetComponent<TextMeshProUGUI>(true, true);
        }
    }
}