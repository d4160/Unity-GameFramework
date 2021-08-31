using System;

namespace d4160.ResourceUtils
{
    /// <summary>
    /// Attribute to tell about the address of an asset inside the 'Resources' folder.
    /// </summary>
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