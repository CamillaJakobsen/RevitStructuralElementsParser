using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models
{
    public class Column
    {
        // Declaring variables
        public int BIM7AATypeID { get; set; }
        public string MaterialID { get; set; }
        public double Length { get; set; }

        public double CrossSectionalArea { get; set; }
        public Dictionary<string, string> MaterialProperties { get; set; } = new Dictionary<string, string>();

        // Constructor: Starts with lower case letter
        public Column(int bIM7AATypeID, string materialID, double length, double crossSectionalArea)
        {
            BIM7AATypeID = bIM7AATypeID;
            MaterialID = materialID;
            Length = length;
            CrossSectionalArea = crossSectionalArea;

        }


    }
}