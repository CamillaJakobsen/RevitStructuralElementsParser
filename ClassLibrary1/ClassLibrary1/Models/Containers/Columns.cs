using StructuralElementsExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Models.Containers
{
    public class Columns
    {
        public List<Column> ColumnsInModel = new List<Column>();

        public void AddColumn(Column column)
        {
            ColumnsInModel.Add(column);
        }

    }
}
