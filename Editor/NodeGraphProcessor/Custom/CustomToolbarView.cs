using GraphProcessor;
using Status = UnityEngine.UIElements.DropdownMenuAction.Status;
using d4160.NodeGraphProcessor;

namespace d4160.Editor.NodeGraphProcessor
{
    public class CustomToolbarView : ToolbarView
    {
        public CustomToolbarView(BaseGraphView graphView) : base(graphView) { }

        protected override void AddButtons()
        {
            // Add the hello world button on the left of the toolbar
            //AddButton("Hello !", () => Debug.Log("Hello World"), left: false);

            // add the default buttons (center, show processor and show in project)
            base.AddButtons();

            bool conditionalProcessorVisible = graphView.GetPinnedElementStatus<ConditionalProcessorView>() != Status.Hidden;
            AddToggle("Show Conditional Processor", conditionalProcessorVisible, (v) => graphView.ToggleView<ConditionalProcessorView>());

            bool stepProcessorVisible = graphView.GetPinnedElementStatus<StepProcessorView>() != Status.Hidden;
            AddToggle("Show Step Processor", stepProcessorVisible, (v) => graphView.ToggleView<StepProcessorView>());
        }
    }
}