namespace d4160.Core.Attributes
{
    using UnityEngine;

    public class DropdownAttribute : PropertyAttribute
    {
        public readonly string[] ExceptionNames;

        /// <summary>
        /// The property of the list of names 
        /// </summary>
        public string ValuesProperty { get; set; }

        public bool IncludeNone { get; set; }

        public DropdownAttribute(params string[] exceptions)
        {
            ExceptionNames = exceptions;
        }
    }
}