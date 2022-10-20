using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;
using System.Windows.Forms;
using Autodesk.Revit.Creation;
using System.Xml.Linq;
using Document = Autodesk.Revit.DB.Document;
using Application = Autodesk.Revit.ApplicationServices.Application;
using ClassLibrary1.Models;

namespace ClassLibrary1
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SelectElements : IExternalCommand
    {
        
        //Find parameter using the Parameter's definition type.
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)

        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            FilteredElementCollector collection = new FilteredElementCollector(doc);

            ElementCategoryFilter allWalls = new ElementCategoryFilter(BuiltInCategory.OST_Walls);

            List<Wall> listOfAllWalls = collection.WherePasses(allWalls).WhereElementIsNotElementType().Cast<Wall>().ToList();

            List<Wall> exteriorWalls = new List<Wall>();

            
            // Creates a Lists of all exterior walls 
            foreach (Wall element in listOfAllWalls)
            {
                var test = element.WallType.Function;
                if (test == WallFunction.Exterior)
                {
                    exteriorWalls.Add(element);
                }
            }
           
            // Assigns the revit parameters to the Outerwall constructor
            foreach (Element element in exteriorWalls)
            {
                // Creates the TypeId
                WallType walltype = doc.GetElement(element.GetTypeId()) as WallType;
                // Change from var to int
                int typeID = walltype.Id.IntegerValue;

               
                //Hvordan får man Structural material??
                //FamilyInstance familyInstance = doc.GetElement(element.GetTypeId()) as FamilyInstance;
                //var materialID = familyInstance.StructuralMaterialType;              
                string materialID = "Concrete";

                // Creates the area of the wall
                double area = element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED).AsDouble();
                double area = ImperialMetricConverter.ConvertFromFeetToMeters
                // Skal lavet om til m2
                
                // Remember this is not in metrics.
                WallType wallType = doc.GetElement(element.GetTypeId()) as WallType;
                double thickness = wallType.Width;
                //Skal laves om til meter

                OuterWall outerWall = new OuterWall(typeID, materialID, area, thickness);

            }


            return Result.Succeeded;
        }


       



    }
}

