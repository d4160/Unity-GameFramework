using UnityEngine.UIElements;
using UnityEngine;
using GraphProcessor;
using System;
using UnityEditor;
using d4160.NodeGraphProcessor;

namespace d4160.Editor.NodeGraphProcessor
{
    public class CustomGraphView : BaseGraphView
    {
        public CustomGraphView(EditorWindow window) : base(window) { }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendSeparator();

            foreach (var nodeMenuItem in NodeProvider.GetNodeMenuEntries())
            {
                var mousePos = (evt.currentTarget as VisualElement).ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
                Vector2 nodePosition = mousePos;
                evt.menu.AppendAction("Create/" + nodeMenuItem.path,
                    (e) => CreateNodeOfType(nodeMenuItem.type, nodePosition),
                    DropdownMenuAction.AlwaysEnabled
                );
            }

            base.BuildContextualMenu(evt);
        }

        void CreateNodeOfType(Type type, Vector2 position)
        {
            RegisterCompleteObjectUndo("Added " + type + " node");
            AddNode(BaseNode.CreateFromType(type, position));
        }
    }
}