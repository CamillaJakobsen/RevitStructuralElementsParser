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

        Dictionary<string, StructuralElements> structuralElements { get; set; } = new Dictionary<string, StructuralElements>();
        public StructuralElements()
        {
            //public void AddstructuralElement(StructuralElement structuralElement)
            //{
            //    structuralElements.Add(structuralElement);
            //}
        }
        
        //public void AddStructuralElement(StructuralElement, structuralElement)
        //{


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

