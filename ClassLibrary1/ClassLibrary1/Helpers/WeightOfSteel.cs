using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Helpers
{
    public class WeightOfSteel
    {
        public static double Convert(double volume)
        {
            // Density of both reinforcement steel and construction steel is 7850 kg/m3 
            return volume * 7850;
        }

    }
}
