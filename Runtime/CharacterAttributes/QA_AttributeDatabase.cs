using System.Collections.Generic;
using UnityEngine;

namespace QuantumTek.QuantumAttributes
{
    /// <summary>
    /// QA_AttributeDatabase stores a list of attribute templates for characters to use.
    /// </summary>
    [CreateAssetMenu(menuName = "Quantum Tek/Quantum Attributes/Attribute Database", fileName = "New Attribute Database")]
    public class QA_AttributeDatabase : ScriptableObject
    {
        public List<QA_Attribute> Attributes = new List<QA_Attribute>();

        /// <summary>
        /// Gets an attribute by name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns></returns>
        public QA_Attribute GetAttribute(string name)
        {
            foreach (var attribute in Attributes)
                if (attribute.Name == name)
                    return attribute;

            return null;
        }
    }
}