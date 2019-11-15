namespace d4160.Core.Attributes
{
    using UnityEngine;

    public class InspectorNoteAttribute : PropertyAttribute
    {
        public readonly string header;
        public readonly string message;

        public InspectorNoteAttribute(string header, string message = "")
        {
            this.header = header;
            this.message = message;
        }
    }

    public class InspectorCommentAttribute : PropertyAttribute
    {
        public readonly string message;

        public InspectorCommentAttribute(string message = "")
        {
            this.message = message;
        }
    }
}