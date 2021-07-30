using GraphProcessor;
using UnityEngine;
using d4160.NodeGraphProcessor;

namespace d4160.Editor.NodeGraphProcessor
{
    public class CustomMiniMapView : MiniMapView
    {
        public CustomMiniMapView(BaseGraphView baseGraphView, int x = 0, int y = 0) : base(baseGraphView)
        {
            SetPosition(new Rect(x, y, 100, 100));
        }
    }
}