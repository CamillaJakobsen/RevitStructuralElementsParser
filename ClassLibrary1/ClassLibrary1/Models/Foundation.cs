using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Models
{
    public class Foundation
    {
        // Declaring variables
        public int TypeID { get; set; }
        public string Material { get; set; }
        public string Quality { get; set; }
        public double Volume { get; set; }


        // Constructor: Starts with lower case letter
        public Foundation(int typeID, string material, string quality, double volume)
        {
            TypeID = typeID;
            Material = material;
            Quality = quality;
            Volume = volume;
            

        }

    }
}