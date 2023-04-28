using d4160.Utilities.UIs;
using Doozy.Runtime.UIManager.Components;

namespace d4160.DoozyUI
{
    public class TextSliderUIButton : SliderBase<string, UIButton>
    {
        protected override void RegisterEvents()
        {
            _prevBtn.onClickEvent.AddListener(GoPrev);
            _nextBtn.onClickEvent.AddListener(GoNext);
        }
    }
}