using System.Collections.Generic;
using UnityEngine;

namespace QuantumTek.QuantumAttributes
{
    /// <summary>
    /// QA_AttributeModifierDatabase stores a list of attribute modifier templates for characters to use.
    /// </summary>
    [CreateAssetMenu(menuName = "Quantum Tek/Quantum Attributes/Modifier Database", fileName = "New Attribute Modifier Database")]
    public class QA_AttributeModifierDatabase : ScriptableObject
    {
        public List<QA_AttributeModifier> Modifiers = new List<QA_AttributeModifier>();

        /// <summary>
        /// Gets a modifier by name.
        /// </summary>
        /// <param name="name">The name of the modifier.</param>
        /// <returns></returns>
        public QA_AttributeModifier GetModifier(string name)
        {
            foreach (var modifier in Modifiers)
                if (modifier.Name == name)
                    return modifier;

            return new QA_AttributeModifier { Name = "" };
        }
    }
}