namespace d4160.Core.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class ResourcesPathAttribute : Attribute
    {
        public readonly string path;

        public ResourcesPathAttribute(string path)
        {
            this.path = path;
        }
    }
}