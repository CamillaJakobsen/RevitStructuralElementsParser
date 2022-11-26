using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StructuralElementsExporter.StructuralAnalysis
{
    [System.Serializable]
    public partial class Materials
    {
        [XmlElement("material", Order = 1)]
        public List<Material> Material = new List<Material>(); // sequence: material_type

    }
}
