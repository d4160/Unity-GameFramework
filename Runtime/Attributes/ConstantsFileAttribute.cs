namespace d4160.GameFramework
{
    using UnityEngine;

    public class ConstantsFileAttribute : PropertyAttribute
    {
        public readonly string fileName;
        public readonly string totalFieldName;

        public ConstantsFileAttribute(string fileName, string totalFieldName)
        {
            this.fileName = fileName;
            this.totalFieldName = totalFieldName;
        }
    }
}
