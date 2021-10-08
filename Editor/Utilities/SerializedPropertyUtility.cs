using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace InspectInLine
{
    public static class SerializedPropertyUtility
    {
        public static IEnumerable<SerializedProperty>
        EnumerateChildProperties(this SerializedObject serializedObject)
        {
            var iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(enterChildren: true))
            {
                // yield return property; // skip "m_Script"
                while (iterator.NextVisible(enterChildren: false))
                {
                    yield return iterator;
                }
            }
        }

        //public static IEnumerable<SerializedProperty>
        //EnumerateChildProperties(this SerializedObject serializedObject, Dictionary<string, SerializedProperty> dic)
        //{
        //    var iterator = serializedObject.GetIterator();
        //    if (dic != null) dic.Clear();
        //    if (iterator.NextVisible(enterChildren: true))
        //    {
        //        // yield return property; // skip "m_Script"
        //        while (iterator.NextVisible(enterChildren: false))
        //        {
        //            if (dic != null)
        //            {
        //                dic.Add(iterator.name, iterator);
        //            }
        //            yield return iterator;
        //        }
        //    }
        //}

        public static Dictionary<string, SerializedProperty>
        GetChildProperties(this SerializedObject serializedObject)
        {
            var dic = new Dictionary<string, SerializedProperty>();
            var iterator = serializedObject.GetIterator();
            if (iterator.NextVisible(enterChildren: true))
            {
                // yield return property; // skip "m_Script"
                while (iterator.NextVisible(enterChildren: false))
                {
                    dic.Add(iterator.name, iterator);
                }
            }

            return dic;
        }

        public static IEnumerable<SerializedProperty>
        EnumerateChildProperties(this SerializedProperty parentProperty)
        {
            var iterator = parentProperty.Copy();
            var end = iterator.GetEndProperty();
            if (iterator.NextVisible(enterChildren: true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, end))
                        yield break;

                    yield return iterator;
                }
                while (iterator.NextVisible(enterChildren: false));
            }
        }
    }
}