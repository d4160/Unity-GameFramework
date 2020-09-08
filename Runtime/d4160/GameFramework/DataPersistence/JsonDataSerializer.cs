using UnityEngine;
using UnityEngine.GameFoundation.Data;

namespace d4160.GameFramework.DataPersistence
{
    /// <summary>
    /// DataSerializer to serialize GameFoundation's data to and from Json.
    /// </summary>
    public sealed class JsonDataSerializer : IDataSerializer
    {
        /// <inheritdoc />
        public string Serialize(GameFoundationData data)
        {
            var json = JsonUtility.ToJson(data);
            return json;
        }

        /// <inheritdoc />
        public GameFoundationData Deserialize(string serializedData)
        {
            return JsonUtility.FromJson<GameFoundationData>(serializedData);
        }
    }
}
