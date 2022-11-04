using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Models.Containers
{
    public class Beams
    {
        public List<object> BeamsInModel = new List<object>();

        public void AddBeam(Beam beam)
        {
            BeamsInModel.Add(beam);
        }

    }
}
