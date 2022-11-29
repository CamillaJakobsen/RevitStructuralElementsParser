﻿using StructuralElementsExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Models.Containers
{
    public class ExteriorWalls
    {
        public List<object> ExteriorWallsInModel = new List<object>();

        public void AddExteriorWall(ExteriorWall exteriorWall)
        {
            ExteriorWallsInModel.Add(exteriorWall);
        }
    }
}