using UnityEngine.UIElements;
using GraphProcessor;
using d4160.NodeGraphProcessor;

namespace d4160.Editor.NodeGraphProcessor
{
    [NodeCustomEditor(typeof(IfNode))]
    public class IfNodeView : BaseNodeView
    {
        public override void Enable()
        {
            hasSettings = true; // or base.Enable();
            var node = nodeTarget as IfNode;

            // Create your fields using node's variables and add them to the controlsContainer

            controlsContainer.Add(new Label($"Last Evaluation: {node.condition}"));
        }
    }
}