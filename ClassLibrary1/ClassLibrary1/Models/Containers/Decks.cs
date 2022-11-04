using StructuralElementsExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructuralElementsExporter.Models.Containers
{
    public class Decks
    {
        
        public List<object> DecksInModel = new List<object>();

        public void AddDeck(Deck deck)
        {
            DecksInModel.Add(deck);
        }
    }


    
}
