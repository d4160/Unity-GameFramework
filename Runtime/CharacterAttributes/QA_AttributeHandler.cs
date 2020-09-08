using System.Collections.Generic;
using UnityEngine;

namespace QuantumTek.QuantumAttributes
{
    /// <summary>
    /// QA_AttributeHandler handles any attributes a character has, including decaying and removing them.
    /// </summary>
    [AddComponentMenu("Quantum Tek/Quantum Attributes/Attribute Handler")]
    public class QA_AttributeHandler : MonoBehaviour
    {
        public QA_AttributeDatabase AttributeDatabase;
        public QA_AttributeModifierDatabase ModifierDatabase;
        public List<QA_Attribute> Attributes { get; set; } = new List<QA_Attribute>();

        private void Update()
        {
            foreach (var attribute in Attributes)
            {
                attribute.ApplyTemporaryModifiers(Time.deltaTime);
                attribute.DecayModifiers(Time.deltaTime);
            }
        }

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

        /// <summary>
        /// Gets an attribute value by name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns></returns>
        public float GetAttributeValue(string name)
        {
            foreach (var attribute in Attributes)
                if (attribute.Name == name)
                    return attribute.Value;

            return 0;
        }

        /// <summary>
        /// Sets an attribute value by name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The new value of the attribute.</param>
        /// <returns></returns>
        public void SetAttributeValue(string name, float value)
        {
            int attributeCount = Attributes.Count;
            for (int i = attributeCount - 1; i >= 0; i--)
            {
                var attribute = Attributes[i];
                if (attribute.Name == name)
                {
                    attribute.SetValue(value);
                    Attributes[i] = attribute;
                }
            }
        }

        /// <summary>
        /// Adds a new attribute to the list from the handler's attribute database.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void AddAttribute(string name, float value = 0)
        {
            foreach (var attribute in Attributes)
                if (attribute.Name == name)
                    return;

            QA_Attribute newAttribute = new QA_Attribute(AttributeDatabase.GetAttribute(name));
            newAttribute.SetValue(value);
            if (newAttribute != null)
                Attributes.Add(newAttribute);
        }

        /// <summary>
        /// Removes an attribute from the handler.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        public void RemoveAttribute(string name)
        {
            int attributeCount = Attributes.Count;
            for (int i = attributeCount - 1; i >= 0; i--)
            {
                if (Attributes[i].Name == name)
                {
                    Attributes.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Adds a modifier to an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="modifierName">The name of the modifier.</param>
        public void AddModifier(string attributeName, string modifierName)
        {
            QA_Attribute attribute = GetAttribute(attributeName);
            if (attribute == null)
                return;
            attribute.AddModifier(ModifierDatabase.GetModifier(modifierName));
        }

        /// <summary>
        /// Removes a modifier from an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="modifierName">The name of the modifier.</param>
        public void RemoveModifier(string attributeName, string modifierName)
        {
            QA_Attribute attribute = GetAttribute(attributeName);
            if (attribute == null)
                return;
            attribute.RemoveModifier(modifierName, ModifierDatabase.GetModifier(modifierName).Duration);
        }
    }
}