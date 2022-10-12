using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class Foundation
    {
        // Declaring variables
        public int BIM7AATypeID { get; set; }
        public string MaterialID { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        // Key value pairs indgår i Dictionary
        public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();

        // Constructor: Starts with lower case letter
        public Foundation(int bIM7AATypeID, string materialID, double length, double width, double height)
        {
            BIM7AATypeID = bIM7AATypeID;
            MaterialID = materialID;
            Length = length;
            Width = width;
            Height = height;

        }

        //public double Density { get; set; } Den kan ses i databasen.





    }
}
