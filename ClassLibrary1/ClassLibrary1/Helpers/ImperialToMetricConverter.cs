using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Helpers
{
    public class ImperialToMetricConverter
    {
        public static double ConvertFromFeetToMeters(double feet)
        {
            return feet * 0.3048;
        }

        public static double ConvertFromSquaredFeetToSquaredMeters(double feet)
        {
            return feet * 0.09290;
        }

        public static double ConvertFromCubicFeetToCubicMeters(double feet)
        {
            return feet * 0.02832;
        }
    }
}
