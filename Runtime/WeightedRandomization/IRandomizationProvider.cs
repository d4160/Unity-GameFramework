using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightedRandomization
{
    public interface IRandomizationProvider
    {
        /// <summary>
        /// Return a random double between 0 and 1.0
        /// </summary>
        /// <returns></returns>
        double NextRandomValue(); 
    }
}
