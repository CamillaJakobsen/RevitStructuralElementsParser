using StructuralElementsExporter.Models.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StructuralElementsExporter.Models
{
    public class StructuralElements
    {

        public Dictionary<string, List<object>> CreateDictionary(Beams beams, Columns columns, Decks decks, ExteriorWalls exteriorWalls, InteriorWalls interiorWalls, Foundations foundations)
        {
            Dictionary<string, List<object>> structuralElements = new Dictionary<string, List<object>>();

            structuralElements.Add("Beam", beams.BeamsInModel);
            structuralElements.Add("Column", columns.ColumnsInModel);
            structuralElements.Add("Deck", decks.DecksInModel);
            structuralElements.Add("ExteriorWall", exteriorWalls.ExteriorWallsInModel);
            structuralElements.Add("InteriorWall", interiorWalls.InteriorWallsInModel);
            structuralElements.Add("Foundation", foundations.FoundationsInModel);
            //structuralElements.Add("Reinforcement", reinforcements.ReinforcementsInModel);

            return structuralElements;
        }

    }
}
