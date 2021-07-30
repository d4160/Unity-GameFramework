using UnityEngine;
using GraphProcessor;
using d4160.NodeGraphProcessor;

namespace d4160.Editor.NodeGraphProcessor
{
    public class CustomGraphWindow : BaseGraphWindow
    {
        BaseGraph tmpGraph;

        //[MenuItem("Window/03 Custom Toolbar")]
        public static BaseGraphWindow OpenWithTmpGraph()
        {
            var graphWindow = CreateWindow<CustomGraphWindow>();

            // When the graph is opened from the window, we don't save the graph to disk
            graphWindow.tmpGraph = ScriptableObject.CreateInstance<BaseGraph>();
            graphWindow.tmpGraph.hideFlags = HideFlags.HideAndDontSave;
            graphWindow.InitializeGraph(graphWindow.tmpGraph);

            graphWindow.Show();

            return graphWindow;
        }

        protected override void OnDestroy()
        {
            graphView?.Dispose();
            DestroyImmediate(tmpGraph);
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent("Custom Toolbar Graph");

            if (graphView == null)
            {
                graphView = new CustomGraphView(this);
                graphView.Add(new CustomToolbarView(graphView));
                graphView.Add(new CustomMiniMapView(graphView, 0, 16));
            }

            rootView.Add(graphView);
        }
    }
}