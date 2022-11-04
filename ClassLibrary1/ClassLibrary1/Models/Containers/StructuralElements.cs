using Autodesk.Revit.DB;
using FemDesign.Bars;
using FemDesign.Releases;
using FemDesign.Results;
using StructuralElementsExporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StructuralElementsExporter.Models
{

    public class StructuralElements
    {
        public Dictionary<string, List<object>> structuralElement = new Dictionary<string, List<object>>();

        public StructuralElements()
        {
            
            //public void AddstructuralElement(string, List<object>)
            //{
                
            //    structuralElements.Add(string, List<object>);

            //}
        }
    }

    ////public void AddStructuralElement(StructuralElement, structuralElement)
    ////{
    //public List<Deck> DecksInModel = new List<Deck>();

    //public void AddDeck(Deck deck)
    //{
    //    DecksInModel.Add(deck);
    //}




    //    Dictionary<string, StructuralElement> structuralElements = new Dictionary<string, StructuralElement>();


    //    public Dictionary<string, StructuralElement> structuralelements { get; set; } = new Dictionary<string, StructuralElement>();


}

