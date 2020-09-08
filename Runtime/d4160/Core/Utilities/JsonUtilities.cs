namespace d4160.Utilities
{
    using System.Collections.Generic;
    using UnityEngine;

    public class JsonUtilities
    {
        //Usage:
        //YouObject[] objects = JsonUtilities.JsonToArray<YouObject> (jsonString);
        public static T[] JsonToArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        //Usage:
        //string jsonString = JsonUtilities.ArrayToJson<YouObject>(objects);
        public static string ArrayToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>() {
                array = array
            };
            
            return JsonUtility.ToJson(wrapper);
        }

        public static List<T> JsonToList<T>(string json)
        {
            ListWrapper<T> wrapper = JsonUtility.FromJson<ListWrapper<T>>(json);
            return wrapper.list;
        }

        public static string ListToJson<T>(List<T> list)
        {
            ListWrapper<T> wrapper = new ListWrapper<T>()
            {
                list = list
            };

            return JsonUtility.ToJson(wrapper);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }

        [System.Serializable]
        private class ListWrapper<T>
        {
            public List<T> list;
        }
    }
}