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
        public int TypeID { get; set; }
        public string MaterialID { get; set; }
        public double Length { get; set; }

        public double CrossSectionalArea { get; set; }
        //public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();

        // Constructor: Starts with lower case letter
        public Column(int typeID, string materialID, double length, double crossSectionalArea)
        {
            TypeID = typeID;
            MaterialID = materialID;
            Length = length;
            CrossSectionalArea = crossSectionalArea;

        }


    }
}