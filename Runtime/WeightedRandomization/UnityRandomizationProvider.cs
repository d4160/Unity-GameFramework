using UnityEngine;
using System.Collections;

namespace WeightedRandomization
{
    public class UnityRandomizationProvider : IRandomizationProvider
    {        
        public static IRandomizationProvider Default { get { return new UnityRandomizationProvider(); } }        

        public double NextRandomValue()
        {
            return UnityEngine.Random.Range(0f, 1f); 
        }
    }
}