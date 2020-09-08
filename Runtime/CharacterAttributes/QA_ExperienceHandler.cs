using UnityEngine;

namespace QuantumTek.QuantumAttributes
{
    /// <summary>
    /// QA_ExperienceMaxType determines how the character's MaxXP increases when leveling up. Constant will increase the MaxXP by a certain number. Percent will increase the MaxXP by a certain percent.
    /// </summary>
    [System.Serializable]
    public enum QA_ExperienceMaxType
    {
        Constant,
        Percent
    }

    /// <summary>
    /// QA_ExperienceLevelType determines how the character's XP increases when leveling up. Carry will keep extra XP when leveling up. Reset will level up once and remove extra XP.
    /// </summary>
    [System.Serializable]
    public enum QA_ExperienceType
    {
        Carry,
        Reset
    }

    /// <summary>
    /// QA_ExperienceHandler handles character level and experience.
    /// </summary>
    [AddComponentMenu("Quantum Tek/Quantum Attributes/Experience Handler")]
    public class QA_ExperienceHandler : MonoBehaviour
    {
        public int Level;
        public int MaxLevel;
        public float XP;
        public float MaxXP;
        public float MaxXPIncrease;
        public QA_ExperienceType XPType;
        public QA_ExperienceMaxType MaxXPType;

        /// <summary>
        /// Adds XP to the handler and returns if it leveled up.
        /// </summary>
        /// <param name="amount">How much XP to add.</param>
        /// <returns></returns>
        public bool AddXP(float amount)
        {
            int oldLevel = Level;
            while (!Mathf.Approximately(amount, 0))
            {
                float space = MaxXP - XP;
                if (space > amount && !Mathf.Approximately(space, amount))
                {
                    XP += amount;
                    amount = 0;
                }
                else
                {
                    Level = Mathf.Clamp(Level + 1, 1, MaxLevel);
                    XP = 0;

                    if (XPType == QA_ExperienceType.Carry)
                        amount -= space;
                    else if (XPType == QA_ExperienceType.Reset)
                        amount = 0;

                    if (oldLevel != Level)
                    {
                        if (MaxXPType == QA_ExperienceMaxType.Constant)
                            MaxXP += MaxXPIncrease;
                        else if (MaxXPType == QA_ExperienceMaxType.Percent)
                            MaxXP *= 1 + MaxXPIncrease / 100;
                    }
                }
            }
            
            return oldLevel != Level;
        }
    }
}