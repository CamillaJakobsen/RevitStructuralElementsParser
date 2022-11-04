using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Models.Containers
{
    public class Foundations
    {
        public List<object> FoundationsInModel = new List<object>();

        public void AddFoundation(Foundation foundation)
        {
            FoundationsInModel.Add(foundation);
        }
    }
}
