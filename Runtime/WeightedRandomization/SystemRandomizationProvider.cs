using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedRandomization
{
    public class SystemRandomizationProvider : IRandomizationProvider
    {
        private System.Random rand;

        public static IRandomizationProvider Default { get { return new SystemRandomizationProvider(); } }

        public SystemRandomizationProvider()
        {
            this.rand = new System.Random(); 
        }

        public double NextRandomValue()
        {
            return this.rand.NextDouble(); 
        }
    }
}
