using System.Collections.Generic;
using UnityEngine;

namespace QuantumTek.QuantumAttributes
{
    /// <summary>
    /// QA_Attribute represents a character attribute, such as strength, agility, etc. It can also be used to represent health, mana, etc.
    /// </summary>
    [System.Serializable]
    public class QA_Attribute
    {
        public string Name;
        public float MinimumValue;
        public float MaximumValue;

        [HideInInspector] public float Value;
        [HideInInspector] public List<QA_AttributeModifier> PermanentModifiers;
        [HideInInspector] public List<QA_AttributeModifier> TemporaryModifiers;

        public QA_Attribute(string name, float minimumValue, float maximumValue, float value, List<QA_AttributeModifier> permanentModifiers, List<QA_AttributeModifier> temporaryModifiers)
        {
            Name = name;
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;

            Value = value;
            PermanentModifiers = new List<QA_AttributeModifier>();
            TemporaryModifiers = new List<QA_AttributeModifier>();

            foreach (var modifier in permanentModifiers)
                PermanentModifiers.Add(new QA_AttributeModifier(modifier));
            foreach (var modifier in temporaryModifiers)
                TemporaryModifiers.Add(new QA_AttributeModifier(modifier));
        }
        public QA_Attribute(QA_Attribute template)
        {
            Name = template.Name;
            MinimumValue = template.MinimumValue;
            MaximumValue = template.MaximumValue;

            Value = template.Value;
            PermanentModifiers = new List<QA_AttributeModifier>(template.PermanentModifiers);
            TemporaryModifiers = new List<QA_AttributeModifier>(template.TemporaryModifiers);

            foreach (var modifier in template.PermanentModifiers)
                PermanentModifiers.Add(new QA_AttributeModifier(modifier));
            foreach (var modifier in template.TemporaryModifiers)
                TemporaryModifiers.Add(new QA_AttributeModifier(modifier));
        }

        /// <summary>
        /// Sets and clamps the value of the attribute.
        /// </summary>
        /// <param name="value">The new value.</param>
        public void SetValue(float value)
        {
            Value = Mathf.Clamp(value, MinimumValue, MaximumValue);
        }

        /// <summary>
        /// Applies each of the temporary modifiers to the attribute. This should be done an update function.
        /// </summary>
        /// <param name="deltaTime">Time.deltaTime or an equivalent, such as fixedTime for FixedUpdate.</param>
        public void ApplyTemporaryModifiers(float deltaTime)
        {
            int modifierCount = TemporaryModifiers.Count;
            for (int i = 0; i < modifierCount; i++)
            {
                var modifier = TemporaryModifiers[i];
                if (Mathf.Approximately(modifier.ModificationInterval, 0))
                    continue;

                if (!Mathf.Approximately(modifier.ModificationTimer, modifier.ModificationInterval))
                {
                    modifier.ModificationTimer = Mathf.Clamp(modifier.ModificationTimer + deltaTime, 0, modifier.ModificationInterval);
                    TemporaryModifiers[i] = modifier;
                    continue;
                }

                if (modifier.ModificationType == QA_AMModificationType.RawValue)
                    SetValue(Value + modifier.Value);
                else if (modifier.ModificationType == QA_AMModificationType.Percent)
                    SetValue(Value + modifier.Value / 100 * MaximumValue);

                modifier.ModificationTimer = 0;
                TemporaryModifiers[i] = modifier;
            }
        }

        /// <summary>
        /// Decays each of the modifiers by the given amount.
        /// </summary>
        /// <param name="amount">How much to decay by.</param>
        public void DecayModifiers(float amount)
        {
            int modifierCount = TemporaryModifiers.Count;
            for (int i = modifierCount - 1; i >= 0; i--)
            {
                var modifier = TemporaryModifiers[i];
                if (modifier.Duration == QA_AMDuration.TemporaryFrames)
                    amount = 1;
                bool decayed = modifier.Decay(amount);
                TemporaryModifiers[i] = modifier;
                if (decayed)
                    RemoveModifier(modifier.Name, modifier.Duration);
            }
        }

        /// <summary>
        /// Adds a modifier to the handler from the given template.
        /// </summary>
        /// <param name="template">The template modifier.</param>
        public void AddModifier(QA_AttributeModifier template)
        {
            if (template.Name.Length == 0)
                return;
            if (template.Duration == QA_AMDuration.Permanent)
            {
                foreach (var modifier in PermanentModifiers)
                    if (modifier.Name == template.Name)
                        return;

                PermanentModifiers.Add(new QA_AttributeModifier(template));
            }
            else
            {
                foreach (var modifier in TemporaryModifiers)
                    if (modifier.Name == template.Name)
                        return;
                QA_AttributeModifier newModifier = new QA_AttributeModifier(template);
                newModifier.LifeTimer = newModifier.Lifetime;
                newModifier.ModificationTimer = newModifier.ModificationInterval;
                TemporaryModifiers.Add(newModifier);
            }

            if (template.Duration == QA_AMDuration.Permanent || Mathf.Approximately(template.ModificationInterval, 0))
            {
                if (template.ModificationType == QA_AMModificationType.RawValue)
                    SetValue(Value + template.Value);
                else if (template.ModificationType == QA_AMModificationType.Percent)
                    SetValue(Value + template.Value / 100 * MaximumValue);
            }
        }

        /// <summary>
        /// Removes a modifier from the handler.
        /// </summary>
        /// <param name="name">The name of the modifier.</param>
        /// <param name="duration">The duration of the modifier, used to remove the modifier from the right list.</param>
        public void RemoveModifier(string name, QA_AMDuration duration)
        {
            if (duration == QA_AMDuration.Permanent)
            {
                int modifierCount = PermanentModifiers.Count;
                for (int i = modifierCount - 1; i >= 0; i--)
                {
                    var modifier = PermanentModifiers[i];
                    if (modifier.Name == name)
                    {
                        if (modifier.ModificationType == QA_AMModificationType.RawValue)
                            SetValue(Value - modifier.Value);
                        else if (modifier.ModificationType == QA_AMModificationType.Percent)
                            SetValue(Value - modifier.Value / 100 * MaximumValue);

                        PermanentModifiers.RemoveAt(i);
                        return;
                    }
                }
            }
            else
            {
                int modifierCount = TemporaryModifiers.Count;
                for (int i = modifierCount - 1; i >= 0; i--)
                {
                    var modifier = TemporaryModifiers[i];
                    if (modifier.Name == name)
                    {
                        if (Mathf.Approximately(modifier.ModificationInterval, 0))
                        {
                            if (modifier.ModificationType == QA_AMModificationType.RawValue)
                                SetValue(Value - modifier.Value);
                            else if (modifier.ModificationType == QA_AMModificationType.Percent)
                                SetValue(Value - modifier.Value / 100 * MaximumValue);
                        }

                        TemporaryModifiers.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }
}