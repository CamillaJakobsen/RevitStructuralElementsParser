using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Models
{
    public class Column
    {
        // Declaring variables
        // Declaring variables
        public int TypeID { get; set; }
        public string Material { get; set; }

        public string Quality { get; set; }

        public double Volume { get; set; }

        public double Weight { get; set; }

        // Constructor: Starts with lower case letter
        public Column(int typeID, string material, string quality, double volume, double weight)
        {
            TypeID = typeID;
            Material = material;
            Quality = quality;
            Volume = volume;
            Weight = weight;

        }


    }
}