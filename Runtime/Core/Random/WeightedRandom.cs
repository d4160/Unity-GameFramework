using System.Collections;
using System.Collections.Generic;
using d4160.Logging;
using UnityEngine;

namespace d4160.RandomM31
{
    public class WeightedRandom<T> 
    {
        public List<WeightedItem<T>> WeightedItems { get; set; }
        public LoggerSO LoggerSO { get; set; }

        // Log messages
        private const string _heading = "RANDOM";
        private const string _weightedItemsNullMsg = "WeightedItems is null, an empty list was created.";
        private const string _weightedItemsEmptyMsg = "WeightedItems is empty.";

        private bool _isAdjusted;

        public void Setup() 
        {
            if (WeightedItems == null) {
                WeightedItems = new List<WeightedItem<T>>();

                LogWarning(_weightedItemsNullMsg);
            }
        }

        /// <summary>
        /// Fix the weights as the sum of all is 1
        /// </summary>
        public void CalculateWeights() 
        {
            Setup();
            if (WeightedItems.Count == 0) {
                LogWarning(_weightedItemsEmptyMsg);
                return;
            }

            float sum = 0f;
            for (var i = 0; i < WeightedItems.Count; i++){
                sum += WeightedItems[i].weight;
            }

            float normalizedWeight = 1f / WeightedItems.Count;
            for (var i = 0; i < WeightedItems.Count; i++){
                WeightedItems[i].normalizedWeight = sum > 0 ? WeightedItems[i].weight / sum : normalizedWeight;
                WeightedItems[i].adjustedWeight += WeightedItems[i].normalizedWeight;
            }
        }

        public void Add(T item, float weight) 
        {
            Setup();

            WeightedItems.Add(new WeightedItem<T>() { item = item, weight = weight });

            CalculateWeights();
        }

        public void Remove(T item)
        {
            Setup();

            for (var i = 0; i < WeightedItems.Count; i++)
            {
                if (WeightedItems[i].item.Equals(item)) {
                    WeightedItems.RemoveAt(i);
                    break;
                }
            }

            CalculateWeights();
        }

        public T GetRandomItem() 
        {
            float value = Random.value;
            for (var i = 0; i < WeightedItems.Count; i++){
                if (value < WeightedItems[i].adjustedWeight) {
                    return WeightedItems[i].item;
                }
            }

            return default;
        }

        private void LogWarning(string message) 
        {
            if (LoggerSO) {
                LoggerSO.LogWarning(message);
            }
            else {
                LoggerM31.LogWarning(message, _heading);
            }
        }
    }

    [System.Serializable]
    public class WeightedItem<T>
    {
        public T item;
        /// <summary>
        /// The weight of the item. eg: 200, 100, 200; for 3 items
        /// </summary>
        public float weight;
        /// <summary>
        /// The normalized weight of the item. eg: .25, .25, .50; for 3 items. Total is 1.
        /// </summary>
        public float normalizedWeight;
        /// <summary>
        /// The adjusted weight of the item. eg: .25, .50, 1; for 3 items. 
        /// </summary>
        public float adjustedWeight;
    }
}