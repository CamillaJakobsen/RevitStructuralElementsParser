using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace ClassLibrary1.Models
{
    
        public class Walls
        {
            //Get application and document objects
            //UIApplication uiapp = commandData.Application;
            //Document doc = uiapp.ActiveUIDocument.Document;

            //Get parameters from elements
            //Element elem = doc.GetElement()

            public string Id { get; set; }
            // Layer is forexample "Ydervægge" det er en datatype vi selv skaber (Den er ikke defineret endnu
            // Når man skriver { get; set;} betyder det at man kan hente og sætte en værdi det tilsvarer []
            public List<Layer> LayersInConstruction { get; set; } = new List<Layer>();

            // Man laver en ny instance af classen (Man Instantierer)
            public List<string> ContainedInSpaces { get; set; } = new List<string>();
        }
}