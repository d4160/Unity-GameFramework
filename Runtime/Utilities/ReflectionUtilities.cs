namespace d4160.Utilities
{
    using System.Reflection;

    public static class ReflectionUtilities
    {
        public static object GetFieldValue(object obj, string fieldName)
        {
            var objType = obj.GetType();
            var field = objType.GetField(fieldName);

            return field.GetValue(obj);
        }

        public static void SetFieldValue(object obj, string fieldName, object value)
        {
            var objType = obj.GetType();
            var field = objType.GetField(fieldName);

            field.SetValue(obj, value);
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            var objType = obj.GetType();
            var prop = objType.GetProperty(propertyName);

            return prop.GetValue(obj, null);
        }

        public static bool IsSubclassOfRawGeneric(System.Type generic, System.Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
