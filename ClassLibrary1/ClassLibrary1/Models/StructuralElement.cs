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

    
    


    public class StructuralElement
    {

        Dictionary<string, StructuralElement> structuralElements { get; set; } = new Dictionary<string, StructuralElement>();
        public StructuralElement()
        {

        }

        //public void AddStructuralElement(StructuralElement, structuralElement)
        //{

        //}
        
        


    }

    
    //    Dictionary<string, StructuralElement> structuralElements = new Dictionary<string, StructuralElement>();


    //    public Dictionary<string, StructuralElement> structuralelements { get; set; } = new Dictionary<string, StructuralElement>();


    //}

    //public class StructuralElement
    //{
    //    public Beam Beam { get; set; }
    //    public Column column { get; set; }
    //    public Deck deck { get; set; }

    //public static implicit operator StructuralElement(StructuralElementsExporter.Models.Beam v)
    //{
    //    throw new NotImplementedException();
    //}
}







//public object[] structuralElements { beam, column, deck, exteriorWall, interiorWall };



//Beam beam = new Beam();








//StructuralElement beam = new StructuralElement()
//{
//    beam = new StructuralElement()
//};

//StructuralElement column = new StructuralElement()
//{
//    column = new StructuralElement()
//};

//StructuralElement Deck = new StructuralElement()
//{
//    Deck = new StructuralElement()
//};

//StructuralElement ExteriorWall = new StructuralElement()
//{
//    ExteriorWall = new StructuralElement()
//};

//StructuralElement InteriorWall = new StructuralElement()
//{
//    InteriorWall = new StructuralElement()
//};
//StructuralElement Foundation = new StructuralElement()
//{
//    Foundation = new StructuralElement()
//};

//StructuralElements.Add(Beam beam);





//Dictionary = { "Decks": [],
//               "Beams": [],
//               "Columns": []}
