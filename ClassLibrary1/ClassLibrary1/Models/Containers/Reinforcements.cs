using StructuralElementsExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1.Models.Containers
{
    public class Reinforcements
    {
        public List<object> ReinforcementsInModel = new List<object>();

        public void AddReinforcement(Reinforcement reinforcement)
        {
            ReinforcementsInModel.Add(reinforcement);
        }
    }
}
