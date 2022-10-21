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

namespace ClassLibrary1.Models
{

    public class InnerWall
    {
        // Declaring variables
        public int TypeID { get; set; }
        public string MaterialID { get; set; }
        public double Area { get; set; }
        public double Thickness { get; set; }
        public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();
        // In MaterialProperties is Density, strength, reinforcement ratio etc.


        // Constructor: Starts with lower case letter
        public InnerWall(int typeID, string materialID, double area, double thickness)
        {
            TypeID = typeID;
            MaterialID = materialID;
            Area = area;
            Thickness = thickness;

        }


    }
}