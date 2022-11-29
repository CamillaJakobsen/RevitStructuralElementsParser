using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace StructuralElementsExporter.Models
{

    public class InteriorWall
    {
        // Declaring variables
        public int TypeID { get; set; }
        public string Material { get; set; }
        public string Quality { get; set; }
        public double Area { get; set; }
        public double Thickness { get; set; }


        // Constructor: Starts with lower case letter
        public InteriorWall(int typeID, string material, string quality, double area, double thickness)
        {
            TypeID = typeID;
            Material = material;
            Quality = quality;
            Area = area;
            Thickness = thickness;

        }


    }
}