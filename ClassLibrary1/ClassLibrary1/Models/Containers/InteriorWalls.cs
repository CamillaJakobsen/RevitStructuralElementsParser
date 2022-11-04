using StructuralElementsExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Models.Containers
{
    public class InteriorWalls
    {
        public List<InteriorWall> InteriorWallsInModel = new List<InteriorWall>();

        public void AddInteriorWall(InteriorWall interiorWall)
        {
            InteriorWallsInModel.Add(interiorWall);
        }
    }
}
