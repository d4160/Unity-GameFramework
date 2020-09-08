using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Malee;
using UnityEngine;
using Object = System.Object;

namespace WeightedRandomization
{
    [Serializable]
    public class WeightedRandomizer<T1, T2, T3> where T2 : WeightedChance<T1>, new() where T3 : ReorderableArray<T2>, new()
    {
        private bool _adjusted;
        [Reorderable(paginate = true, pageSize = 10)]
        [SerializeField] private T3 _weights; 

        public IRandomizationProvider Provider { get; set; }

        public T3 Weights => _weights;

        public WeightedRandomizer()
        {
            this._weights = new T3();
        }

        public void AddOrUpdateWeight(T1 value, float weight)
        {
            if(weight == 0)
                throw new ArgumentException("weighted value cannot have a 0% chance.");


            WeightedChance<T1> existing = this._weights.FirstOrDefault(x => Object.Equals(x.Value, value));
            if (existing == null)
            {
                var instance = new T2
                {
                    Value = value,
                    Weight = weight
                };
                this._weights.Add(instance);
            }
            else
                existing.Weight = weight;

            this._adjusted = false; 
        }

        public void RemoveWeight(T1 value)
        {
            T2 existing = this._weights.FirstOrDefault(x => Object.Equals(x.Value, value));
            if (existing != null)
            {
                this._weights.Remove(existing);
                this._adjusted = false;
            }
        }

        public void ClearWeights()
        {
            this._weights.Clear();
            this._adjusted = false; 
        }
     
        /// <summary>
        /// Determines the adjusted weights for all items in the collection. This will be called automatically if GetNext is called after there are changes to the weights collection. 
        /// </summary>
        public void CalculateAdjustedWeights()
        {
            var sorted = this._weights.OrderBy(x => x.Weight).ToArray();
            decimal weightSum = 0; 
            for (int i = 0; i < sorted.Length; i++)
            {                
                weightSum += (decimal)sorted[i].Weight;               
                if (i == 0)
                    sorted[i].AdjustedWeight = sorted[i].Weight;
                else
                    sorted[i].AdjustedWeight = sorted[i].Weight + sorted[i - 1].AdjustedWeight;                
            }            
                        
            if (!Mathf.Approximately((float)weightSum, 1.0f))
                throw new InvalidOperationException("The weights of all items must add up to 1.0 ");

            this._weights.CopyFrom(sorted); //this._weights.OrderBy(x => x.AdjustedWeight)

            this._adjusted = true; 
        }

#if UNITY_EDITOR
        public void FixRandomizerWeights()
        {
            decimal accumulate = 0m;

            var sorted = _weights.OrderBy(x => x.Weight).ToList();

            foreach (var weight in sorted)
            {
                accumulate += (decimal)weight.Weight;
            }

            if (accumulate != 1m)
            {
                decimal mod = 0m;
                mod = accumulate < 1m ? 1m - accumulate : accumulate - 1m;
                mod /= sorted.Count;

                for (int i = 0; i < sorted.Count; i++)
                {
                    sorted[i].Weight = accumulate < 1m ? sorted[i].Weight + (float)mod : sorted[i].Weight - (float)mod;
                }
            }

            _weights.CopyFrom(sorted);

            CalculateAdjustedWeights();
        }
#endif

        /// <summary>
        /// Return a value based on the weights provided. 
        /// </summary>
        /// <returns></returns>
        public T1 GetNext()
        {            
            if (this.Provider == null)
                this.Provider = UnityRandomizationProvider.Default;

            if (!_adjusted)
                this.CalculateAdjustedWeights();
                        
            double d = this.Provider.NextRandomValue();            
            var item = this._weights.FirstOrDefault(x => d <= x.AdjustedWeight);
            return item.Value;
        }
    }

    [Serializable]
    public class ReorderableIntWeights : ReorderableArray<IntWeightedChance>
    { }

    [Serializable]
    public class IntWeightedChance : WeightedChance<int>
    {
    }

    [Serializable]
    public class IntWeightedRandomizer : WeightedRandomizer<int, IntWeightedChance, ReorderableIntWeights>
    {
    }
}
