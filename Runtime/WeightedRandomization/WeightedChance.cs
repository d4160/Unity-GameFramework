using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WeightedRandomization
{
    [Serializable]
    public class WeightedChance<T>
    {
        [SerializeField] protected T _value;
        [Range(0f, 1f)]
        [SerializeField] protected float _weight;

        /// <summary>
        /// Target value of this randomization
        /// </summary>
        public T Value
        {
            get => _value;
            set => _value = value;
        }

        /// <summary>
        /// Weight from 0..1
        /// </summary>
        public float Weight
        {
            get => _weight;
            set => _weight = value;
        }

        /// <summary>
        /// Adjusted weight based on the weights of other items added to the randomizer
        /// </summary>
        public float AdjustedWeight { get; set; }        
    }
}
