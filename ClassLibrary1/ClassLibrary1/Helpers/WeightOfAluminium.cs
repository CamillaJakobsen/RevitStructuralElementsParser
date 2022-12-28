using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Helpers
{
    public class WeightOfAluminium
    {
        public static double Convert(double volume)
        {
            // Density of aluminium is 2700 kg/m3 
            return volume * 2700;
        }

    }
}
