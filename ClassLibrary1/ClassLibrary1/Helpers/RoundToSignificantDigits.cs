﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Helpers
{
    static class RoundToSignificantDigits
    {
        public static double RoundDigits(this double d, int digits)
        {
            if (d == 0)
                return 0;
            
            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits);

        }

    }
}