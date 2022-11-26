using FemDesign.Materials;
using FemDesign;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StructuralElementsExporter.StructuralAnalysis
{
    public partial class Material
    {
        [XmlAttribute("standard")]
        public string Standard { get; set; } // standardtype
        [XmlAttribute("country")]
        public string Country { get; set; } // eurocodetype
        /// <summary>
        /// Name of Material.
        /// </summary>
        /// <value></value>
        [XmlAttribute("name")]
        public string Name { get; set; } // name256
        [XmlElement("timber")]
        public Timber Timber { get; set; }
        [XmlElement("concrete")]
        public Concrete Concrete { get; set; }
        [XmlElement("custom")]
        public Custom Custom { get; set; }
        [XmlElement("steel")]
        public Steel Steel { get; set; }
        [XmlElement("reinforcing_steel")]
        public ReinforcingSteel ReinforcingSteel { get; set; }
        [XmlElement("stratum")]
        public StruSoft.Interop.StruXml.Data.Material_typeStratum Stratum { get; set; }

        [XmlIgnore]
        public string Family
        {
            get
            {
                if (this.Steel != null)
                    return "Steel";
                else if (this.Concrete != null)
                    return "Concrete";
                else if (this.Timber != null)
                    return "Timber";
                else if (this.Stratum != null)
                    return "Stratum";
                else if (this.ReinforcingSteel != null)
                    return "ReinforcingSteel";
                else
                    return "Custom";
            }
        }

        public static Material GetMaterialByNameOrIndex(List<Material> materials, dynamic materialInput)
        {
            Material material;
            var isNumeric = int.TryParse(materialInput.ToString(), out int n);
            if (!isNumeric)
            {
                try
                {
                    material = materials.Where(x => x.Name == materialInput).First();
                }
                catch (Exception ex)
                {
                    throw new Exception($"{materialInput} does not exist!", ex);
                }
            }
            else
            {
                try
                {
                    material = materials[n];
                }
                catch (Exception ex)
                {
                    throw new System.Exception($"Materials List only contains {materials.Count} item. {materialInput} is out of range!", ex);
                }
            }
            return material;
        }

        public override string ToString()
        {
            return $"{this.Name}";
        }

        

    }
}
