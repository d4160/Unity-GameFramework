using UnityEngine;

namespace QuantumTek.QuantumAttributes
{
    /// <summary>
    /// QA_AMModificationType determines how a modifier will change a character attribute.
    /// </summary>
    [System.Serializable]
    public enum QA_AMModificationType
    {
        RawValue,
        Percent
    }
    /// <summary>
    /// QA_AMDuration determines how long a modifier will last. Permanent means it lasts until manually removed. Temporary means it lasts for a given number of seconds or frames before removal.
    /// </summary>
    [System.Serializable]
    public enum QA_AMDuration
    {
        Permanent,
        TemporarySeconds,
        TemporaryFrames
    }

    /// <summary>
    /// QA_AttributeModifier represents a modification to a character attribute, such as a permanent strength buff, a temporary poison that decreases health, etc.
    /// </summary>
    [System.Serializable]
    public struct QA_AttributeModifier
    {
        public string Name;
        public QA_AMModificationType ModificationType;
        public float Value;
        public QA_AMDuration Duration;
        public float Lifetime;
        public float ModificationInterval;

        public float LifeTimer { get; set; }
        public float ModificationTimer { get; set; }

        public QA_AttributeModifier(string name, QA_AMModificationType modificationType, float value, QA_AMDuration duration, float lifetime, float modificationInterval, float lifeTimer, float modificationTimer)
        {
            Name = name;
            ModificationType = modificationType;
            Value = value;
            Duration = duration;
            Lifetime = lifetime;
            ModificationInterval = modificationInterval;
            LifeTimer = lifeTimer;
            ModificationTimer = modificationTimer;
        }
        public QA_AttributeModifier(QA_AttributeModifier template)
        {
            Name = template.Name;
            ModificationType = template.ModificationType;
            Value = template.Value;
            Duration = template.Duration;
            Lifetime = template.Lifetime;
            ModificationInterval = template.ModificationInterval;
            LifeTimer = template.LifeTimer;
            ModificationTimer = template.ModificationTimer;
        }

        /// <summary>
        /// Decreases the life timer by a certain amount, if the modifier is temporary. Returns if the modifier has been decayed or not.
        /// </summary>
        /// <param name="amount">How much to decay the modifier.</param>
        /// <returns></returns>
        public bool Decay(float amount)
        {
            if (Duration == QA_AMDuration.Permanent)
                return false;

            LifeTimer = Mathf.Clamp(LifeTimer - amount, 0, Lifetime);
            return Mathf.Approximately(LifeTimer, 0);
        }
    }
}