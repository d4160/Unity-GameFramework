using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.ScriptableObjects;
using UnityEngine;

namespace d4160.DoozyUI
{
    [CreateAssetMenu(menuName = "d4160/Doozy UI/UIPopup")]
    public class UIPopupSO : ScriptableObject
    {
        [SerializeField] private UIPopupLink _popupLnk;
        [SerializeField] private bool _autoSelectAfterShow = true;

        private UIPopup _popup;

        public UIPopup Popup 
        { 
            get {
                if (_popup == null) 
                {
                    _popup = UIPopup.Get(_popupLnk.prefabName);
                }
                return _popup; 
            }
        }

        public void Show(float hideDelay = 0, params string[] texts)
        {
            if (hideDelay == 0)
            {
                Popup.AutoHideAfterShow = false;
            }
            else
            {
                Popup.AutoHideAfterShow = true;
                Popup.AutoHideAfterShowDelay = hideDelay;
            }

            Popup.AutoSelectAfterShow = _autoSelectAfterShow;
            if (_autoSelectAfterShow) Popup.AutoSelectTarget = Popup.gameObject;

            Popup.SetTexts(texts);
            Popup.Show();
        }

        public void Hide()
        {
            Popup.Hide();
        }
    }
}