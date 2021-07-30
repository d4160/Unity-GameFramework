using UnityEditor;
using GraphProcessor;
using UnityEngine.UIElements;

namespace d4160.Editor.NodeGraphProcessor
{
    [CustomEditor(typeof(BaseGraph), true)]
    public class GraphAssetInspector : GraphInspector
    {
        // protected override void CreateInspector()
        // {
        // }

        protected override void CreateInspector()
        {
            base.CreateInspector();

            root.Add(new Button(() => EditorWindow.GetWindow<CustomGraphWindow>().InitializeGraph(target as BaseGraph))
            {
                text = "Open Graph Window"
            });
        }
    }
}